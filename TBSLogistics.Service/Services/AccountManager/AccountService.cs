using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.AccountModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.AccountManager
{
	public class AccountService : IAccount
	{
		private readonly ICommon _common;
		private readonly TMSContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public AccountService(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
		{
			_common = common;
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<BoolActionResult> CreateAccount(CreateOrUpdateAccount request)
		{
			try
			{
				if (request.ListCustomer.Count == 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn Khách Hàng Cho Account" };
				}

				var checkAccount = await _context.AccountOfCustomer.Where(x => x.TenAccount == request.AccountName.Trim()).FirstOrDefaultAsync();

				if (checkAccount != null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Account này đã tồn tại" };
				}

				var getAccountMax = await _context.AccountOfCustomer.OrderByDescending(x => x.MaAccount).Select(x => x.MaAccount).FirstOrDefaultAsync();

				string AccountId = "ACC";

				if (string.IsNullOrEmpty(getAccountMax))
				{
					AccountId = AccountId + "00001";
				}
				else
				{
					AccountId = AccountId + (int.Parse(getAccountMax.Substring(3, getAccountMax.Length - 3)) + 1).ToString("00000");
				}

				var checkListKh = await _context.KhachHang.Where(x => request.ListCustomer.Contains(x.MaKh)).ToListAsync();
				if (checkListKh.Count != request.ListCustomer.Count)
				{
					return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
				}

				await _context.AccountOfCustomer.AddAsync(new AccountOfCustomer()
				{
					MaAccount = AccountId,
					TenAccount = request.AccountName,
					TrangThai = 1,
					CreatedTime = DateTime.Now,
					Creator = tempData.UserName,
				});

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					foreach (var item in checkListKh)
					{
						await _context.KhachHangAccount.AddAsync(new KhachHangAccount()
						{
							MaAccount = AccountId,
							MaKh = item.MaKh,
							CreatedTime = DateTime.Now,
							Creator = tempData.UserName,
						});

						await _context.UserHasCustomer.AddAsync(new UserHasCustomer()
						{
							UserId = tempData.UserID,
							CustomerId = item.MaKh,
							AccountId = AccountId
						});
					}

					var add = await _context.SaveChangesAsync();
					if (add > 0)
					{
						await _common.LogTimeUsedOfUser(tempData.Token);

						return new BoolActionResult { isSuccess = true, Message = "Tạo Account thành công" };
					}
					else
					{
						return new BoolActionResult { isSuccess = false, Message = "Tạo Account không thành công" };
					}
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Tạo Account không thành công" };
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<GetAccountById> GetAccountById(string accountId)
		{
			var getByid = await _context.AccountOfCustomer.Where(x => x.MaAccount == accountId).FirstOrDefaultAsync();
			var getListCus = await _context.KhachHangAccount.Where(x => x.MaAccount == accountId).ToListAsync();

			return new GetAccountById()
			{
				AccountId = getByid.MaAccount,
				AccountName = getByid.TenAccount,
				ListCustomer = getListCus.Select(x => x.MaKh).ToList(),
				StatusId = getByid.TrangThai
			};
		}

		public async Task<BoolActionResult> UpdateAccount(string accountId, CreateOrUpdateAccount request)
		{
			try
			{
				if (request.ListCustomer.Count == 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn Khách Hàng Cho Account" };
				}

				var getByid = await _context.AccountOfCustomer.Where(x => x.MaAccount == accountId).FirstOrDefaultAsync();
				if (getByid == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Account không tồn tại" };
				}

				getByid.Updater = tempData.UserName;
				getByid.UpdatedTime = DateTime.Now;

				var getListCus = await _context.KhachHangAccount.Where(x => x.MaAccount == accountId).ToListAsync();

				foreach (var item in getListCus.Select(x => x.MaKh).Where(x => !request.ListCustomer.Contains(x)))
				{
					var getListConTract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == item).Select(x => x.MaHopDong).ToListAsync();

					var checkPriceTable = await _context.BangGia.Where(x => x.MaAccount == accountId && getListConTract.Contains(x.MaHopDong)).FirstOrDefaultAsync();

					if (checkPriceTable != null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Account đã tồn tại trong bảng giá, không thể bỏ" };
					}
					else
					{
						_context.RemoveRange(getListCus.Where(x => x.MaKh == item && x.MaAccount == accountId));
					}
				}

				foreach (var item in request.ListCustomer.Where(x => !getListCus.Select(x => x.MaKh).Contains(x)))
				{
					var checkCus = await _context.KhachHang.Where(x => x.MaKh == item.Trim()).FirstOrDefaultAsync();
					if (checkCus == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Khách hàng thêm mới không tồn tại" };
					}
					else
					{
						await _context.KhachHangAccount.AddAsync(new KhachHangAccount()
						{
							MaAccount = accountId,
							MaKh = item,
							CreatedTime = DateTime.Now,
							Creator = tempData.UserName,
						});
					}
				}

				var result = await _context.SaveChangesAsync();
				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật Account thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<List<GetAccountById>> GetListAccountSelectByCus(string cusId)
		{
			var getListAccount = from acc in _context.AccountOfCustomer
								 join khacc in _context.KhachHangAccount
								 on acc.MaAccount equals khacc.MaAccount
								 select new { acc, khacc };

			var getListAccOfUser = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID && x.AccountId != null).ToListAsync();

			var data = await getListAccount.Where(x =>
			getListAccOfUser.Select(y => y.CustomerId).Contains(x.khacc.MaKh) &&
			getListAccOfUser.Select(y => y.AccountId).Contains(x.khacc.MaAccount)
			).ToListAsync();

			if (!string.IsNullOrEmpty(cusId))
			{
				data = data.Where(x => x.khacc.MaKh == cusId).ToList();
			}

			return data.GroupBy(x => new { x.acc.MaAccount, x.acc.TenAccount }).Select(x => new { AccountId = x.Key.MaAccount, AccountName = x.Key.TenAccount }).Select(x => new GetAccountById()
			{
				AccountId = x.AccountId,
				AccountName = x.AccountName,
			}).ToList();
		}
	}
}