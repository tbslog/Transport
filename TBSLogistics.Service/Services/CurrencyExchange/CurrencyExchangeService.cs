using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CurrencyExchangeModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.CurrencyExchange
{
	public class CurrencyExchangeService : ICurrencyExchange
	{
		private readonly TMSContext _context;
		private readonly ICommon _common;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public CurrencyExchangeService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_common = common;
			_context = context;
			tempData = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToArray().Length > 0 ? _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", "")) : new TempData();
		}

		public async Task<BoolActionResult> CreateExchangeRate(List<CreateExchangeRateModel> request)
		{
			try
			{
				foreach (var item in request)
				{
					if (request.Where(x => x.CurrencyCode == item.CurrencyCode && x.Bank == item.Bank && x.CreatedTime.Date == item.CreatedTime.Date).Count() > 1)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dữ liệu bị trùng lặp, vui lòng kiểm tra lại" };
					}
				}

				foreach (var item in request)
				{
					var checkExists = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == item.CurrencyCode.Trim()).FirstOrDefaultAsync();
					if (checkExists == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại tiền tệ không tồn tại" };
					}

					if (item.CreatedTime.Date > DateTime.Now.Date)
					{
						return new BoolActionResult { isSuccess = false, Message = "Không được chọn ngày lớn hơn ngày hiện tại" };
					}

					var checkData = await _context.ExchangeRate.Where(x => x.CurrencyCode == item.CurrencyCode && x.Bank == item.Bank && x.DateTransfer.Value == item.CreatedTime.Date).FirstOrDefaultAsync();
					if (checkData != null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại tiền tệ đã tồn tại" };
					}

					var checkCurrency = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == item.CurrencyCode).FirstOrDefaultAsync();
					if (checkCurrency == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại tiền tệ không tồn tại" };
					}

					var checkBanks = await _context.NganHang.Where(x => x.MaNganHang == item.Bank).FirstOrDefaultAsync();
					if (checkBanks == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Ngân hàng không tồn tại" };
					}

					await _context.ExchangeRate.AddAsync(new ExchangeRate()
					{
						CurrencyCode = checkExists.MaLoaiTienTe,
						CurrencyName = checkExists.TenLoaiTienTe,
						PriceBuy = item.PriceBuy,
						PriceSell = item.PriceSell,
						PriceTransfer = item.PriceTransfer,
						Bank = item.Bank.ToUpper(),
						PriceFix = item.PriceFix,
						Note = item.Note,
						Creator = tempData.UserName,
						CreatedTime = DateTime.Now,
						DateTransfer = item.CreatedTime
					});
				}

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					return new BoolActionResult { isSuccess = true, Message = "Thêm mới thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Thêm mới thất bại" };
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<BoolActionResult> UpdateExchangeRate(int id, float priceFix, string note)
		{
			try
			{
				var checkExPrice = await _context.ExchangeRate.Where(x => x.Id == id).FirstOrDefaultAsync();

				if (checkExPrice == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "ID không tồn tại" };
				}

				if (string.IsNullOrEmpty(note))
				{
					return new BoolActionResult { isSuccess = false, Message = "ID vui lòng nhập ghi chú" };
				}

				if (priceFix < 1)
				{
					return new BoolActionResult { isSuccess = false, Message = "Gía quy đổi không hợp lệ" };
				}

				checkExPrice.PriceFix = priceFix;
				checkExPrice.Note = note;
				checkExPrice.Updater = tempData.UserName;
				checkExPrice.UpdatedTime = DateTime.Now;

				_context.Update(checkExPrice);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await _common.Log("update ExchangeRate", "user:" + tempData.UserName + " has update pricefix= " + priceFix + " and note:" + note);
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật giá quy đổi thành công!" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Gía quy đổi không hợp lệ" };
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<PagedResponseCustom<ExchangeRate>> GetListExchangeRate(PaginationFilter filter)
		{
			try
			{
				var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

				var getData = from exR in _context.ExchangeRate
							  select exR;

				if (!string.IsNullOrEmpty(filter.Keyword))
				{
					getData = getData.Where(x => x.CurrencyCode == filter.Keyword);
				}

				if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
				{
					getData = getData.Where(x => x.CreatedTime.Date >= filter.fromDate && x.CreatedTime <= filter.toDate);
				}

				var totalRecords = await getData.CountAsync();

				var pagedData = await getData.OrderByDescending(x => x.CreatedTime).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ExchangeRate()
				{
					Id = x.Id,
					CurrencyCode = x.CurrencyCode,
					CurrencyName = x.CurrencyName,
					PriceBuy = x.PriceBuy,
					PriceSell = x.PriceSell,
					PriceTransfer = x.PriceTransfer,
					Bank = x.Bank,
					PriceFix = x.PriceFix,
					Note = x.Note,
					CreatedTime = x.CreatedTime,
					Creator = x.Creator,
				}).ToListAsync();

				return new PagedResponseCustom<ExchangeRate>()
				{
					paginationFilter = validFilter,
					totalCount = totalRecords,
					dataResponse = pagedData
				};
			}
			catch (Exception ex)
			{
				return new PagedResponseCustom<ExchangeRate>();
			}
		}

		public async Task<double> GetPriceTradeNow(string priceCode, DateTime? dateTime = null, string bank = null)
		{
			if (priceCode == "VND")
			{
				return 1;
			}

			var getPriceTrade = from exP in _context.ExchangeRate select exP;

			if (dateTime.HasValue)
			{
				getPriceTrade = getPriceTrade.Where(x => x.DateTransfer.Value.Date == dateTime.Value.Date && x.CurrencyCode == priceCode);
			}
			else
			{
				getPriceTrade = getPriceTrade.Where(x => x.CurrencyCode == priceCode).OrderByDescending(x => x.CreatedTime);
			}

			if (!string.IsNullOrEmpty(bank))
			{
				getPriceTrade = getPriceTrade.Where(x => x.Bank == bank);
			}

			var ExchangePrice = getPriceTrade.FirstOrDefault();

			if (ExchangePrice == null)
			{
				return 0;
			}

			if (ExchangePrice.PriceFix.HasValue)
			{
				return ExchangePrice.PriceFix.Value;
			}

			return ExchangePrice.PriceSell.Value;
		}

		public async Task<BoolActionResult> GetPriceTrade()
		{
			var checkPriceTrade = await _context.ExchangeRate.Where(x => x.CreatedTime.Date == DateTime.Now.Date).ToListAsync();

			if (checkPriceTrade.Count() > 0)
			{
				return new BoolActionResult { isSuccess = true };
			}

			try
			{
				XmlDocument xmlDcoument = new XmlDocument();
				xmlDcoument.Load("https://portal.vietcombank.com.vn/Usercontrols/TVPortal.TyGia/pXML.aspx");

				XmlNodeList xmlNodeList = xmlDcoument.DocumentElement.SelectNodes("/ExrateList");

				var dataFTrade = new List<ForeignTrade>();

				foreach (XmlNode xmlNode in xmlNodeList)
				{
					foreach (XmlNode item in xmlNode.ChildNodes)
					{
						if (item.Name == "Exrate")
						{
							dataFTrade.Add(new ForeignTrade
							{
								CurrencyCode = item.Attributes.GetNamedItem("CurrencyCode").Value.ToString().Trim(),
								CurrencyName = item.Attributes.GetNamedItem("CurrencyName").Value.ToString().Trim(),
								Buy = item.Attributes.GetNamedItem("Buy").Value.ToString().Trim() == "-" ? null : item.Attributes.GetNamedItem("Buy").Value.ToString().Trim(),
								Transfer = item.Attributes.GetNamedItem("Transfer").Value.ToString().Trim() == "-" ? null : item.Attributes.GetNamedItem("Transfer").Value.ToString().Trim(),
								Sell = item.Attributes.GetNamedItem("Sell").Value.ToString().Trim() == "-" ? null : item.Attributes.GetNamedItem("Sell").Value.ToString().Trim(),
							});
						}
					}
				}

				await _context.ExchangeRate.AddRangeAsync(dataFTrade.Select(x => new ExchangeRate()
				{
					CurrencyCode = x.CurrencyCode,
					CurrencyName = x.CurrencyName,
					PriceBuy = x.Buy == null ? null : double.Parse(x.Buy.Trim(), CultureInfo.InvariantCulture),
					PriceSell = x.Sell == null ? null : double.Parse(x.Sell.Trim(), CultureInfo.InvariantCulture),
					PriceTransfer = x.Transfer == null ? null : double.Parse(x.Transfer.Trim(), CultureInfo.InvariantCulture),
					CreatedTime = DateTime.Now,
					DateTransfer = DateTime.Now,
					Bank = "VCB",
				}));

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
			catch (Exception ex)
			{
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}
	}
}