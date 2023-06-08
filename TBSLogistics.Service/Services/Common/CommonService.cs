using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Model.TempModel;

namespace TBSLogistics.Service.Services.Common
{
	public class CommonService : ICommon
	{
		private IHostingEnvironment _environment;
		private TMSContext _context;
		private readonly MailSettings _mailSettings;
		private readonly ILogger<CommonService> _logger;
		private readonly string _userContentFolder;
		private const string USER_CONTENT_FOLDER_NAME = "Attachments";
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CommonService(IHostingEnvironment environment, TMSContext context, IHttpContextAccessor httpContextAccessor, IOptions<MailSettings> mailSettings, ILogger<CommonService> logger)
		{
			_environment = environment;
			_context = context;
			_userContentFolder = Path.Combine(environment.WebRootPath, USER_CONTENT_FOLDER_NAME);
			_httpContextAccessor = httpContextAccessor;
			_mailSettings = mailSettings.Value;
			_logger = logger;
		}

		public async Task Log(string FileName, string LogMessage)
		{
			try
			{
				string dirPath = Path.Combine(_environment.WebRootPath, "Log");
				string fileName = Path.Combine(dirPath, FileName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

				await LogDB(FileName + " - " + DateTime.Now.ToString("yyyy-MM-dd"), LogMessage);
				await WriteToLog(dirPath, fileName, LogMessage);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		private async Task LogDB(string Name, string LogMessage)
		{
			await _context.Log.AddAsync(new Log()
			{
				ModuleName = Name,
				Message = LogMessage,
				LogTime = DateTime.Now
			});
			await _context.SaveChangesAsync();
		}

		public async Task WriteToLog(string dir, string file, string content)
		{
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file), true))
			{
				await outputFile.WriteLineAsync(string.Format("Logged on: {1} \r\n at: {2}{0}Message: {3} at {4}{0}--------------------{0}",
						  Environment.NewLine, DateTime.Now.ToLongDateString(),
						  DateTime.Now.ToLongTimeString(), content, DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss.fff")));
			}
		}

		public string GetFileUrl(string fileName, string fileFolder)
		{
			return $"/{fileFolder}/{fileName}";
		}

		public string GetFile(string fileFolder)
		{
			return $"{_userContentFolder}/{fileFolder}";
		}

		public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName, string fileFolder)
		{
			if (!Directory.Exists(_userContentFolder + $"/{fileFolder}"))
				Directory.CreateDirectory(_userContentFolder + $"/{fileFolder}");

			var filePath = Path.Combine(_userContentFolder + $"/{fileFolder}", fileName);
			using var output = new FileStream(filePath, FileMode.Create);
			await mediaBinaryStream.CopyToAsync(output);
		}

		public async Task DeleteFileAsync(string fileName, string IfilePath)
		{
			var filePath = Path.Combine(_userContentFolder, IfilePath);
			if (File.Exists(filePath))
			{
				await Task.Run(() => File.Delete(filePath));
			}
		}

		public async Task<BoolActionResult> AddAttachment(Attachment attachment)
		{
			try
			{
				var add = await _context.Attachment.AddAsync(attachment);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					return new BoolActionResult { isSuccess = true, DataReturn = add.Entity.Id.ToString() };
				}
				else
				{
					return new BoolActionResult { isSuccess = false };
				}
			}
			catch (Exception ex)
			{
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		public async Task<Attachment> GetAttachmentById(int id)
		{
			var getAttachment = await _context.Attachment.Where(x => x.Id == id).FirstOrDefaultAsync();

			if (getAttachment == null)
			{
				return null;
			}

			return getAttachment;
		}

		public async Task<BoolActionResult> CheckPermission(string permissionId)
		{
			var tempData = DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));

			var user = await _context.NguoiDung.Where(x => x.Id == tempData.UserID).FirstOrDefaultAsync();

			if (user.TrangThai == 2)
			{
				return new BoolActionResult { isSuccess = false, Message = "Tài khoản đã bị khóa" };
			}

			var checkPermission = from per in _context.Permission
								  join rolehasper in _context.RoleHasPermission
								  on per.Id equals rolehasper.PermissionId
								  where rolehasper.RoleId == user.RoleId
								  && per.Mid == permissionId
								  select per;

			if (checkPermission.ToList().Count > 0)
			{
				return new BoolActionResult { isSuccess = true };
			}
			else
			{
				return new BoolActionResult { isSuccess = false, Message = "Bạn không có quyền hạn này" };
			}
		}

		public TempData DecodeToken(string token)
		{
			if (!string.IsNullOrEmpty(token))
			{
				var handler = new JwtSecurityTokenHandler();
				var jwtSecurityToken = handler.ReadJwtToken(token);

				int userId = int.Parse(jwtSecurityToken.Claims.First(x => x.Type == "UserId").Value);

				var checkUser = _context.NguoiDung.Where(x => x.Id == userId).FirstOrDefault();

				if (checkUser == null)
				{
					return new TempData()
					{
						Token = "",
						UserID = 0,
						UserName = "",
						AccType = "",
					};
				}

				return new TempData()
				{
					Token = token,
					UserID = checkUser.Id,
					UserName = jwtSecurityToken.Claims.First(x => x.Type == "UserName").Value,
					AccType = checkUser.AccountType,
				};
			}
			return new TempData();
		}

		public async Task SendMail(MailContent mailContent)
		{
			var email = new MimeMessage();
			email.Sender = new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail);
			email.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
			email.To.Add(MailboxAddress.Parse(mailContent.To));
			email.Cc.Add(MailboxAddress.Parse("hongthai.pham@tbslogistics.com"));
			email.Cc.Add(MailboxAddress.Parse("hai.le@tbslogistics.com"));
			email.Subject = mailContent.Subject;

			var builder = new BodyBuilder();
			builder.HtmlBody = mailContent.Body;
			email.Body = builder.ToMessageBody();

			// dùng SmtpClient của MailKit
			using var smtp = new MailKit.Net.Smtp.SmtpClient();

			try
			{
				smtp.Connect(_mailSettings.Host, _mailSettings.Port);
				smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
				await smtp.SendAsync(email);
			}
			catch (Exception ex)
			{
				// Gửi mail thất bại, nội dung email sẽ lưu vào thư mục mailssave
				System.IO.Directory.CreateDirectory("mailssave");
				var emailsavefile = string.Format(@"mailssave/{0}.eml", Guid.NewGuid());
				await email.WriteToAsync(emailsavefile);

				_logger.LogInformation("Lỗi gửi mail, lưu tại - " + emailsavefile);
				_logger.LogError(ex.Message);
			}

			smtp.Disconnect(true);

			_logger.LogInformation("send mail to " + mailContent.To);
		}

		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			await SendMail(new MailContent()
			{
				To = email,
				Subject = subject,
				Body = htmlMessage
			});
		}

		public async Task LogTimeUsedOfUser(string token)
		{
			var checkToken = await _context.LogTimeUsedOfUsers.Where(x => x.Token == token).FirstOrDefaultAsync();
			if (checkToken != null)
			{
				checkToken.LastTimeRequest = DateTime.Now;
				_context.Update(checkToken);
			}
			else
			{
				var handler = new JwtSecurityTokenHandler();
				var jwtSecurityToken = handler.ReadJwtToken(token);

				await _context.LogTimeUsedOfUsers.AddAsync(new LogTimeUsedOfUsers()
				{
					Token = token,
					UserName = jwtSecurityToken.Claims.First(x => x.Type == "UserName").Value,
					TimeLogin = DateTime.Now,
					LastTimeRequest = null,
				});
			}

			await _context.SaveChangesAsync();
		}
	}
}