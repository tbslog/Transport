using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;

namespace TBSLogistics.Service.Repository.Common
{
    public class CommonService : ICommon
    {
        private IHostingEnvironment _environment;
        private TMSContext _context;
        private readonly string _userContentFolder;
        private const string USER_CONTENT_FOLDER_NAME = "Attachments";


        public CommonService(IHostingEnvironment environment, TMSContext context)
        {
            _environment = environment;
            _context = context;
            _userContentFolder = Path.Combine(Directory.GetCurrentDirectory(), USER_CONTENT_FOLDER_NAME);
        }

        public async Task Log(string FileName, string LogMessage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
                {
                    _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "Log");
                }

                string dirPath = _environment.WebRootPath;
                string fileName = Path.Combine(dirPath, FileName + " - " + DateTime.Now.ToString("yyyy-MM-dd") + ".txt");

                await LogDB(fileName, LogMessage);
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
            using (StreamWriter outputFile = new StreamWriter(Path.Combine(dir, file), true))
            {
                await outputFile.WriteLineAsync(string.Format("Logged on: {1} \r\n at: {2}{0}Message: {3} at {4}{0}--------------------{0}",
                          Environment.NewLine, DateTime.Now.ToLongDateString(),
                          DateTime.Now.ToLongTimeString(), content, DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss.fff")));
            }
        }

        public string GetFileUrl(string fileName, string fileFolder)
        {
            return $"/{USER_CONTENT_FOLDER_NAME}/{fileFolder}/{fileName}";
        }

        public async Task SaveFileAsync(Stream mediaBinaryStream, string fileName, string fileFolder)
        {
            if (!Directory.Exists(_userContentFolder + $"{fileFolder}"))
                Directory.CreateDirectory(_userContentFolder + $"{fileFolder}");

            var filePath = Path.Combine(_userContentFolder + $"{fileFolder}", fileName);
            using var output = new FileStream(filePath, FileMode.Create);
            await mediaBinaryStream.CopyToAsync(output);
        }

        public async Task DeleteFileAsync(string fileName, string fileFolder)
        {
            var filePath = Path.Combine(_userContentFolder + $"{fileFolder}", fileName);
            if (File.Exists(filePath))
            {
                await Task.Run(() => File.Delete(filePath));
            }
        }

        public async Task<BoolActionResult> AddAttachment(Attachment attachment)
        {
            var checkExists = await _context.Attachment.Where(x => x.FileName == attachment.FileName).FirstOrDefaultAsync();

            if(checkExists == null)
            {
                await _context.Attachment.AddAsync(attachment);
            }
            else
            {
                checkExists.FilePath = attachment.FilePath;
                checkExists.FileSize = attachment.FileSize;
                checkExists.FileType = attachment.FileType;
                checkExists.FolderName = attachment.FolderName;

                _context.Attachment.Update(checkExists);
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new BoolActionResult { isSuccess = true };
            }
            else
            {
                return new BoolActionResult { isSuccess = false };
            }
        }
    }
}
