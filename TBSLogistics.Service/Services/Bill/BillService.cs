using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.CurrencyExchange;

namespace TBSLogistics.Service.Services.Bill
{
	public class BillService : IBill
	{
		private readonly TMSContext _context;
		private readonly ICommon _common;
		private readonly ICurrencyExchange _currencyExchange;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public BillService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor, ICurrencyExchange currencyExchange)
		{
			_currencyExchange = currencyExchange;
			_context = context;
			_common = common;
			_httpContextAccessor = httpContextAccessor;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<GetBill> GetBillByCustomerId(string customerId, DateTime datePay, DateTime dateTime, string bank)
		{
			try
			{
				await StoreDataBill(customerId, datePay, dateTime, bank);

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				var dateBegin = new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);

				var getlistHandling = from dp in _context.DieuPhoi
									  join vd in _context.VanDon
									  on dp.MaVanDon equals vd.MaVanDon
									  join ky in _context.ChotSanLuongTheoKy
									  on dp.Id equals ky.MaDieuPhoi
									  where dp.TrangThai == 20 && ky.MaKh == customerId
									  && ky.KyChot == dateBegin.Month
									  select new { vd, dp, ky };

				var getDataTransport = from kh in _context.KhachHang
									   join vd in _context.VanDon
									   on kh.MaKh equals vd.MaKh
									   orderby vd.CreatedTime
									   select new { kh, vd };

				var getSubFeeContract = from sf in _context.SubFee
										join sfp in _context.SubFeePrice
										on sf.SubFeeId equals sfp.SfId
										select new { sf, sfp };

				var getListTransport = await getDataTransport.Where(x => getlistHandling.Select(s => s.vd.MaVanDon).Contains(x.vd.MaVanDon)).OrderBy(x => x.vd.MaVanDonKh).Select(z => new ListVanDon()
				{
					MaVanDonKH = z.vd.MaVanDonKh,
					DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
					DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
					MaVanDon = z.vd.MaVanDon,
					MaKh = z.kh.MaKh,
					TenKh = z.kh.TenKh,
					AccountName = z.vd.MaAccount == null ? _context.KhachHang.Where(y => y.MaKh == z.vd.MaKh).Select(y => y.TenKh).FirstOrDefault() : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
					LoaiVanDon = z.vd.LoaiVanDon == "xuat" ? "Xuất" : "Nhập",
					TongTheTich = z.vd.TongTheTich,
					TongKhoiLuong = z.vd.TongKhoiLuong,
					TongSoKien = z.vd.TongSoKien,
					listHandling = getlistHandling.Where(y => y.vd.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.dp.Id).Select(x => new Model.Model.BillModel.ListHandling()
					{
						MaSoXe = x.dp.MaSoXe,
						DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						MaRomooc = x.dp.MaRomooc,
						TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
						LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
						LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.dp.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
						DonViTinh = _context.DonViTinh.Where(y => y.MaDvt == x.dp.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
						DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(x => x.TenKh).FirstOrDefault(),
						DonGiaKh = x.ky.DonGia,
						DonGiaNcc = x.ky.DonGia,
						LoaiTienTeKh = x.ky.MaLoaiTienTe,
						LoaiTienTeNcc = x.ky.MaLoaiTienTe,
						KhoiLuong = x.dp.KhoiLuong,
						TheTich = x.dp.TheTich,
						SoKien = x.dp.SoKien,
						listSubFeeByContract = getSubFeeContract.Where(b => _context.PhuPhiTheoKy.Where(v => v.IdsanLuongKy == x.ky.Id).Select(v => v.IdphuPhiHopDong).Contains(b.sfp.PriceId)).Select(x => new ListSubFeeByContract()
						{
							//ContractId = x.sfp.ContractId,
							//ContractName = _context.HopDongVaPhuLuc.Where(c => c.MaHopDong == x.sfp.ContractId).Select(c => c.TenHienThi).FirstOrDefault(),
							sfName = x.sf.SfName,
							goodsType = x.sfp.GoodsType,
							unitPrice = x.sfp.Price,
							priceType = x.sfp.PriceType
						}).ToList(),
						listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => _context.PhuPhiTheoKy.Where(v => v.IdphuPhiPhatSinh != null && v.IdphuPhiHopDong == null && v.IdsanLuongKy == x.ky.Id).Select(y => y.IdphuPhiPhatSinh).Contains(y.Id)).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
						{
							Note = x.Note,
							Price = x.Price,
							sfName = _context.SubFee.Where(y => y.SubFeeId == x.SfId).Select(x => x.SfName).FirstOrDefault()
						}).ToList(),
					}).ToList(),
				}).ToListAsync();

				foreach (var item in getListTransport)
				{
					foreach (var iHandling in item.listHandling)
					{
						iHandling.GiaQuyDoiKh = iHandling.DonGiaKh * (decimal)await _currencyExchange.GetPriceTradeNow(iHandling.LoaiTienTeKh, dateTime, bank);
						iHandling.GiaQuyDoiNcc = iHandling.DonGiaNcc * (decimal)await _currencyExchange.GetPriceTradeNow(iHandling.LoaiTienTeNcc, dateTime, bank);
						iHandling.listSubFeeByContract.ForEach(async x =>
						{
							x.priceTransfer = x.unitPrice * (double)await _currencyExchange.GetPriceTradeNow(x.priceType, dateTime, bank);
						});
					}
				}

				return new GetBill()
				{
					BillReuslt = getListTransport
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		private async Task<BoolActionResult> StoreDataBill(string customerId, DateTime datePay, DateTime dateTime, string bank)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var getlistHandling = from dp in _context.DieuPhoi
									  join vd in _context.VanDon
									  on dp.MaVanDon equals vd.MaVanDon
									  where dp.TrangThai == 20
									  select new { vd, dp };

				var getListSFOfContract = from sfp in _context.SubFeePrice
										  join sfc in _context.SubFeeByContract
										  on sfp.PriceId equals sfc.PriceId
										  join sf in _context.SubFee
										  on sfp.SfId equals sf.SubFeeId
										  select new { sfp, sfc, sf };

				if (customerId.Trim().Substring(0, 3) == "CUS")
				{
					getlistHandling = getlistHandling.Where(x => x.vd.MaKh == customerId);
				}
				if (customerId.Trim().Substring(0, 3) == "SUP")
				{
					getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == customerId);
				}

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				var dateBegin = new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);

				var dateEnd = new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime >= dateBegin && x.dp.CreatedTime <= dateEnd);

				int ky = dateBegin.Month;

				var getDataBill = await _context.ChotSanLuongTheoKy.Where(x => x.KyChot == ky && x.MaKh == customerId).ToListAsync();

				if (getDataBill.Where(x => x.TrangThai == 2).Count() > 0)
				{
					return new BoolActionResult { isSuccess = false };
				}

				var dataStore = await getlistHandling.Where(x => !getDataBill.Select(y => y.MaDieuPhoi).Contains(x.dp.Id)).ToListAsync();
				var listData = new List<ChotSanLuongTheoKy>();

				foreach (var item in dataStore)
				{
					decimal price = 0;

					if (customerId.Trim().Substring(0, 3) == "CUS")
					{
						price = item.dp.DonGiaKh.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh, dateTime, bank);
					}
					else
					{
						price = item.dp.DonGiaNcc.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc, dateTime, bank);
					}

					listData.Add(new ChotSanLuongTheoKy()
					{
						MaKh = customerId,
						MaLoaiKh = customerId.Trim().Substring(0, 3) == "CUS" ? "KH" : "NCC",
						MaAccount = customerId.Trim().Substring(0, 3) == "CUS" ? item.vd.MaAccount : null,
						MaDieuPhoi = item.dp.Id,
						DonGia = customerId.Trim().Substring(0, 3) == "CUS" ? item.dp.DonGiaKh.Value : item.dp.DonGiaNcc.Value,
						MaLoaiTienTe = customerId.Trim().Substring(0, 3) == "CUS" ? item.dp.LoaiTienTeKh : item.dp.LoaiTienTeNcc,
						DonGiaQuyDoi = price,
						KyChot = ky,
						TrangThai = 1,
						CreatedTime = DateTime.Now,
						Creator = tempData.UserName,
					});
				}

				await _context.ChotSanLuongTheoKy.AddRangeAsync(listData);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					var listDataSubfee = new List<PhuPhiTheoKy>();

					foreach (var item in listData)
					{
						var getListSf = await getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == item.MaDieuPhoi && y.sfp.CusType == (customerId.Substring(0, 3) == "CUS" ? "KH" : "NCC")).ToListAsync();
						foreach (var x in getListSf)
						{
							listDataSubfee.Add(new PhuPhiTheoKy()
							{
								IdsanLuongKy = item.Id,
								IdphuPhiHopDong = x.sfc.PriceId,
								DonGia = (decimal)x.sfp.Price,
								MaLoaiTienTe = x.sfp.PriceType,
								DonGiaQuyDoi = (decimal)x.sfp.Price * (decimal)await _currencyExchange.GetPriceTradeNow(x.sfp.PriceType, dateTime, bank),
								CreatedTime = DateTime.Now,
								Creator = tempData.UserName,
							});
						}

						var listSfI = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.MaDieuPhoi && y.ApproveStatus == 14).ToListAsync();
						foreach (var x in listSfI)
						{
							listDataSubfee.Add(new PhuPhiTheoKy()
							{
								IdsanLuongKy = item.Id,
								IdphuPhiPhatSinh = x.Id,
								DonGia = (decimal)x.Price,
								MaLoaiTienTe = "VND",
								DonGiaQuyDoi = (decimal)x.Price,
								CreatedTime = DateTime.Now,
								Creator = tempData.UserName,
							});
						}
					}
					await _context.PhuPhiTheoKy.AddRangeAsync(listDataSubfee);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = ky.ToString() };
				}
				else
				{
					return new BoolActionResult { isSuccess = false };
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<BoolActionResult> StoreDataHandlingToBill(StoreDataHandling request)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var checkData = await _context.ChotSanLuongTheoKy.Where(x => x.MaKh == request.cusId && request.ids.Contains(x.MaDieuPhoi)).ToListAsync();
				if (checkData.Count > 0)
				{
					return new BoolActionResult { isSuccess = false, Message = string.Join(',', checkData.Select(x => x.MaDieuPhoi)) + " đã tồn tại trong kỳ" };
				}

				var getlistHandling = from dp in _context.DieuPhoi
									  join vd in _context.VanDon
									  on dp.MaVanDon equals vd.MaVanDon
									  where dp.TrangThai == 20 && request.ids.Contains(dp.Id)
									  select new { vd, dp };

				var getListSFOfContract = from sfp in _context.SubFeePrice
										  join sfc in _context.SubFeeByContract
										  on sfp.PriceId equals sfc.PriceId
										  join sf in _context.SubFee
										  on sfp.SfId equals sf.SubFeeId
										  select new { sfp, sfc, sf };

				if (request.cusId.Trim().Substring(0, 3) == "CUS")
				{
					getlistHandling = getlistHandling.Where(x => x.vd.MaKh == request.cusId);
				}
				if (request.cusId.Trim().Substring(0, 3) == "SUP")
				{
					getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == request.cusId);
				}

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == request.cusId && x.MaHopDongCha == null).FirstOrDefaultAsync();

				var dateBegin = new DateTime(request.dateBlock.Year, request.dateBlock.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);

				var dateEnd = new DateTime(request.dateBlock.Year, request.dateBlock.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				int ky = dateBegin.Month;

				var getDataBill = await _context.ChotSanLuongTheoKy.Where(x => x.KyChot == ky && x.MaKh == request.cusId).ToListAsync();

				if (getDataBill.Where(x => x.TrangThai == 2).Count() > 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Kỳ số " + ky.ToString() + " đã chốt, không thể thêm chuyến" };
				}

				var dataStore = await getlistHandling.Where(x => !getDataBill.Select(y => y.MaDieuPhoi).Contains(x.dp.Id)).ToListAsync();

				var listData = new List<ChotSanLuongTheoKy>();

				foreach (var item in dataStore)
				{
					decimal price = 0;

					if (request.cusId.Trim().Substring(0, 3) == "CUS")
					{
						price = item.dp.DonGiaKh.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeKh);
					}
					else
					{
						price = item.dp.DonGiaNcc.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.dp.LoaiTienTeNcc);
					}

					listData.Add(new ChotSanLuongTheoKy()
					{
						MaKh = request.cusId,
						MaLoaiKh = request.cusId.Trim().Substring(0, 3) == "CUS" ? "KH" : "NCC",
						MaAccount = request.cusId.Trim().Substring(0, 3) == "CUS" ? item.vd.MaAccount : null,
						MaDieuPhoi = item.dp.Id,
						DonGia = request.cusId.Trim().Substring(0, 3) == "CUS" ? item.dp.DonGiaKh.Value : item.dp.DonGiaNcc.Value,
						MaLoaiTienTe = request.cusId.Trim().Substring(0, 3) == "CUS" ? item.dp.LoaiTienTeKh : item.dp.LoaiTienTeNcc,
						DonGiaQuyDoi = price,
						KyChot = ky,
						TrangThai = 1,
						CreatedTime = DateTime.Now,
						Creator = tempData.UserName,
					});
				}

				await _context.ChotSanLuongTheoKy.AddRangeAsync(listData);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					var listDataSubfee = new List<PhuPhiTheoKy>();

					foreach (var item in listData)
					{
						var getListSf = await getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == item.MaDieuPhoi && y.sfp.CusType == (request.cusId.Substring(0, 3) == "CUS" ? "KH" : "NCC")).ToListAsync();
						foreach (var x in getListSf)
						{
							listDataSubfee.Add(new PhuPhiTheoKy()
							{
								IdsanLuongKy = item.Id,
								IdphuPhiHopDong = x.sfc.PriceId,
								DonGia = (decimal)x.sfp.Price,
								MaLoaiTienTe = x.sfp.PriceType,
								DonGiaQuyDoi = (decimal)x.sfp.Price * (decimal)await _currencyExchange.GetPriceTradeNow(x.sfp.PriceType),
								CreatedTime = DateTime.Now,
								Creator = tempData.UserName,
							});
						}

						var listSfI = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.MaDieuPhoi && y.ApproveStatus == 14).ToListAsync();
						foreach (var x in listSfI)
						{
							listDataSubfee.Add(new PhuPhiTheoKy()
							{
								IdsanLuongKy = item.Id,
								IdphuPhiPhatSinh = x.Id,
								DonGia = (decimal)x.Price,
								MaLoaiTienTe = "VND",
								DonGiaQuyDoi = (decimal)x.Price,
								CreatedTime = DateTime.Now,
								Creator = tempData.UserName,
							});
						}
					}
					await _context.PhuPhiTheoKy.AddRangeAsync(listDataSubfee);
					await _context.SaveChangesAsync();
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = "Thêm chuyến vào kỳ thành công!" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Thêm chuyến vào kỳ thất bại!" };
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<GetBill> GetBillByTransportId(string transportId, long? handlingId, DateTime dateTime, string bank)
		{
			try
			{
				var getlistHandling = from dp in _context.DieuPhoi
									  join vd in _context.VanDon
									  on dp.MaVanDon equals vd.MaVanDon
									  where dp.TrangThai == 20
									  select dp;

				var getDataTransport = from kh in _context.KhachHang
									   join vd in _context.VanDon
									   on kh.MaKh equals vd.MaKh
									   orderby vd.CreatedTime
									   select new { kh, vd };

				var getListSFOfContract = from sfp in _context.SubFeePrice
										  join sfc in _context.SubFeeByContract
										  on sfp.PriceId equals sfc.PriceId
										  join sf in _context.SubFee
										  on sfp.SfId equals sf.SubFeeId
										  select new { sfp, sfc, sf };

				if (handlingId != null)
				{
					getlistHandling = getlistHandling.Where(x => x.Id == handlingId);
				}

				if (!string.IsNullOrEmpty(transportId))
				{
					getDataTransport = getDataTransport.Where(x => x.vd.MaVanDon == transportId);
				}
				else
				{
					getDataTransport = getDataTransport.Where(x => x.vd.MaVanDon == getlistHandling.Select(y => y.MaVanDon).FirstOrDefault());
				}

				var getListTransport = await getDataTransport.Select(z => new ListVanDon()
				{
					MaVanDonKH = z.vd.MaVanDonKh,
					DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
					DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == z.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
					MaVanDon = z.vd.MaVanDon,
					MaKh = z.kh.MaKh,
					TenKh = z.kh.TenKh,
					AccountName = z.vd.MaAccount == null ? _context.KhachHang.Where(y => y.MaKh == z.vd.MaKh).Select(y => y.TenKh).FirstOrDefault() : _context.AccountOfCustomer.Where(y => y.MaAccount == z.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
					LoaiVanDon = z.vd.LoaiVanDon == "xuat" ? "Xuất" : "Nhập",
					TongTheTich = z.vd.TongTheTich,
					TongKhoiLuong = z.vd.TongKhoiLuong,
					TongSoKien = z.vd.TongSoKien,
					listHandling = getlistHandling.Where(y => y.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.Id).Select(x => new Model.Model.BillModel.ListHandling()
					{
						MaSoXe = x.MaSoXe,
						DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						MaRomooc = x.MaRomooc,
						TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
						LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
						LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
						DonViTinh = _context.DonViTinh.Where(y => y.MaDvt == x.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
						DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.DonViVanTai).Select(x => x.TenKh).FirstOrDefault(),
						DonGiaKh = x.DonGiaKh.Value,
						DonGiaNcc = x.DonGiaNcc.Value,
						LoaiTienTeKh = x.LoaiTienTeKh,
						LoaiTienTeNcc = x.LoaiTienTeNcc,
						KhoiLuong = x.KhoiLuong,
						TheTich = x.TheTich,
						SoKien = x.SoKien,
						listSubFeeByContract = getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == x.Id && y.sfp.CusType == z.kh.MaLoaiKh).Select(x => new ListSubFeeByContract()
						{
							ContractId = x.sfp.ContractId,
							ContractName = _context.HopDongVaPhuLuc.Where(c => c.MaHopDong == x.sfp.ContractId).Select(c => c.TenHienThi).FirstOrDefault(),
							sfName = x.sf.SfName,
							goodsType = x.sfp.GoodsType,
							unitPrice = x.sfp.Price,
							priceType = x.sfp.PriceType
						}).ToList(),
						listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
						{
							Note = x.Note,
							Price = x.Price,
							sfName = _context.SubFee.Where(y => y.SubFeeId == x.SfId).Select(x => x.SfName).FirstOrDefault()
						}).ToList(),
					}).ToList(),
				}).ToListAsync();

				foreach (var item in getListTransport)
				{
					foreach (var iHandling in item.listHandling)
					{
						iHandling.GiaQuyDoiKh = iHandling.DonGiaKh * (decimal)await _currencyExchange.GetPriceTradeNow(iHandling.LoaiTienTeKh, dateTime, bank);
						iHandling.GiaQuyDoiNcc = iHandling.DonGiaNcc * (decimal)await _currencyExchange.GetPriceTradeNow(iHandling.LoaiTienTeNcc, dateTime, bank);
						iHandling.listSubFeeByContract.ForEach(async x =>
						{
							x.priceTransfer = x.unitPrice * (double)await _currencyExchange.GetPriceTradeNow(x.priceType, dateTime, bank);
						});
					}
				}

				return new GetBill()
				{
					BillReuslt = getListTransport
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<BoolActionResult> BlockDataBillByKy(string customerId, DateTime datePay, DateTime dateTime, string bank)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				var dateBegin = new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);

				var dateEnd = new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.Date.DayOfWeek == DayOfWeek.Saturday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				var getDataBill = await _context.ChotSanLuongTheoKy.Where(x => x.MaKh == customerId && x.KyChot == dateBegin.Month).ToListAsync();

				if (getDataBill.Where(x => x.TrangThai == 2).Count() > 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Sản lượng kỳ " + dateBegin.Month.ToString() + " đã được chốt trước đó" };
				}

				foreach (var item in getDataBill)
				{
					var exchangeRate = (decimal)await _currencyExchange.GetPriceTradeNow(item.MaLoaiTienTe, dateTime, bank);
					if (exchangeRate == 0)
					{
						return new BoolActionResult { isSuccess = false, Message = "Ngày " + dateTime.ToString("dd-mm-yyyy") + " không có dữ liệu tỉ giá " + item.MaLoaiTienTe + " nên không thể quy đổi" };
					}

					item.DonGiaQuyDoi = item.DonGia * exchangeRate;
					item.TimeBlock = DateTime.Now;
					item.Blocker = tempData.UserName;
					item.TrangThai = 2;
				}

				_context.UpdateRange(getDataBill);
				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = "Đã chốt kỳ " + dateBegin.Month + " thành công!" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Chốt kỳ không thành công!" };
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<PagedResponseCustom<ListBillHandling>> GetListHandlingToPick(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getlistHandling = from dp in _context.DieuPhoi
								  join vd in _context.VanDon
								  on dp.MaVanDon equals vd.MaVanDon
								  where dp.TrangThai == 20
								  select new { dp, vd };

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getlistHandling = getlistHandling.Where(x => x.vd.MaVanDonKh.Contains(filter.Keyword));
			}

			if (filter.customerId.Substring(0, 3) == "SUP")
			{
				getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.customerId);
			}

			if (filter.customerId.Substring(0, 3) == "CUS")
			{
				getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId);
			}

			if (!string.IsNullOrEmpty(filter.vehicleType))
			{
				getlistHandling = getlistHandling.Where(x => x.dp.MaLoaiPhuongTien == filter.vehicleType);
			}

			getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value && x.dp.CreatedTime.Date <= filter.toDate.Value);

			var totalRow = await getlistHandling.CountAsync();

			var pagedData = await getlistHandling.OrderByDescending(x => x.dp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
			{
				HangTau = x.vd.HangTau,
				handlingId = x.dp.Id,
				ContNo = x.dp.ContNo,
				ThoiGianHoanThanh = x.dp.ThoiGianHoanThanh,
				MaChuyen = x.dp.MaChuyen,
				AccountName = x.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
				CutOffDate = x.vd.ThoiGianHaCang,
				DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
				DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
				DiemLayRong = x.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
				DiemTraRong = x.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
				MaPTVC = x.vd.MaPtvc,
				MaVanDonKH = x.vd.MaVanDonKh,
				MaVanDon = x.dp.MaVanDon,
				LoaiVanDon = x.vd.LoaiVanDon,
				LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
				LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
				MaNCC = x.dp.DonViVanTai,
				MaKH = x.vd.MaKh,
				TenKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
				TenNCC = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
				DonGiaKH = x.dp.DonGiaKh,
				DonGiaNCC = x.dp.DonGiaNcc,
				LoaiTienTeKH = x.dp.LoaiTienTeKh,
				Reuse = x.dp.ReuseCont == true ? "REUSE" : "",
				LoaiTienTeNCC = x.dp.LoaiTienTeNcc,
				createdTime = x.dp.CreatedTime,
			}).ToListAsync();

			return new PagedResponseCustom<ListBillHandling>()
			{
				dataResponse = pagedData,
				totalCount = totalRow,
				paginationFilter = validFilter
			};
		}

		public async Task<PagedResponseCustom<ListBillTransportWeb>> GetListBillCustomer(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getlistHandling = from dp in _context.DieuPhoi
								  join vd in _context.VanDon
								  on dp.MaVanDon equals vd.MaVanDon
								  where dp.TrangThai == 20
								  select new { dp, vd };

			var getListSFOfContract = from sfp in _context.SubFeePrice
									  join sfc in _context.SubFeeByContract
									  on sfp.PriceId equals sfc.PriceId
									  select new { sfp, sfc };

			var listData = new List<ListBillHandlingWeb>();

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getlistHandling = getlistHandling.Where(x => x.vd.MaVanDonKh.Contains(filter.Keyword));
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();

				var dateBegin = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);
				var dateEnd = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				var getlistDataBill = await _context.ChotSanLuongTheoKy.Where(x => x.KyChot == dateBegin.Month && x.MaKh == filter.customerId).Select(x => x.MaDieuPhoi).ToListAsync();
				if (getlistDataBill.Count > 0)
				{
					getlistHandling = getlistHandling.Where(x => getlistDataBill.Contains(x.dp.Id));
				}
				else
				{
					getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId && x.dp.CreatedTime >= dateBegin && x.dp.CreatedTime <= dateEnd);
				}

				var getData = await getlistHandling.ToListAsync();

				if (getData.Count() > 0)
				{
					foreach (var item in getData)
					{
						listData.Add(new ListBillHandlingWeb()
						{
							MaVanDon = item.dp.MaVanDon,
							handlingId = item.dp.Id,
							DonViVanTai = _context.KhachHang.Where(y => y.MaKh == item.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
							LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == item.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
							LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == item.dp.MaLoaiPhuongTien).Select(y => y.TenLoaiPhuongTien).FirstOrDefault(),
							MaSoXe = item.dp.MaSoXe,
							TaiXe = item.dp.MaTaiXe,
							DonGiaKH = item.dp.DonGiaKh.Value,
							LoaiTienTeKH = item.dp.LoaiTienTeKh,
							PhuPhiHD = 0,
							PhuPhiPhatSinh = _context.SfeeByTcommand.Where(y => y.IdTcommand == item.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
							ContNo = item.dp.ContNo,
							SealNP = item.dp.SealNp,
							SealHQ = item.dp.SealHq,
							DiemLayRong = item.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == item.dp.DiemLayRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
							DiemTraRong = item.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == item.dp.DiemTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
						});
					}
				}
			}
			else
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDongCha == null && x.MaLoaiHopDong == "SELL").ToListAsync();

				foreach (var item in getListContractOfCus)
				{
					var dateBegin = new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value).AddMonths(-1).AddDays(1);
					var dateEnd = new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value);

					if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
					{
						dateEnd = dateEnd.AddDays(1);
					}

					var getDataFilter = await getlistHandling.Where(x => x.vd.MaKh == item.MaKh && x.dp.CreatedTime >= dateBegin && x.dp.CreatedTime <= dateEnd).ToListAsync();

					if (getDataFilter.Count > 0)
					{
						listData.AddRange(getDataFilter.Select(z => new ListBillHandlingWeb()
						{
							MaVanDon = z.dp.MaVanDon,
							handlingId = z.dp.Id,
							DonViVanTai = _context.KhachHang.Where(y => y.MaKh == z.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
							LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == z.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
							LoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == z.dp.MaLoaiPhuongTien).Select(y => y.TenLoaiPhuongTien).FirstOrDefault(),
							MaSoXe = z.dp.MaSoXe,
							TaiXe = z.dp.MaTaiXe,
							DonGiaKH = z.dp.DonGiaKh.Value,
							LoaiTienTeKH = z.dp.LoaiTienTeKh,
							PhuPhiHD = 0,
							PhuPhiPhatSinh = _context.SfeeByTcommand.Where(y => y.IdTcommand == z.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
							ContNo = z.dp.ContNo,
							SealNP = z.dp.SealNp,
							SealHQ = z.dp.SealHq,
							DiemLayRong = z.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == z.dp.DiemLayRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
							DiemTraRong = z.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == z.dp.DiemTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
						}));
					}
				}
			}

			var totalCount = listData.Count();
			var pagedData = listData.OrderBy(x => x.MaVanDon).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize);
			var getlistTransport = await _context.VanDon.Where(x => pagedData.Select(y => y.MaVanDon).Contains(x.MaVanDon)).OrderByDescending(x => x.MaVanDon).ToListAsync();

			var data = getlistTransport.Select(x => new ListBillTransportWeb()
			{
				MaVanDon = x.MaVanDon,
				BookingNo = x.MaVanDonKh,
				HangTau = x.HangTau == null ? "" : _context.ShippingInfomation.Where(y => y.ShippingCode == x.HangTau).Select(y => y.ShippingLineName).FirstOrDefault(),
				TenKH = _context.KhachHang.Where(y => y.MaKh == x.MaKh).Select(y => y.TenKh).FirstOrDefault(),
				Account = x.MaAccount == null ? "" : _context.AccountOfCustomer.Where(y => y.MaAccount == x.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
				LoaiVanDon = x.LoaiVanDon.Trim() == "xuat" ? "Xuất" : "Nhập",
				MaPTVC = x.MaPtvc,
				listBillHandlingWebs = pagedData.Where(y => y.MaVanDon == x.MaVanDon).Select(z => new ListBillHandlingWeb()
				{
					DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
					DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
					MaVanDon = z.MaVanDon,
					handlingId = z.handlingId,
					DonViVanTai = z.DonViVanTai,
					LoaiHangHoa = z.LoaiHangHoa,
					LoaiPhuongTien = z.LoaiPhuongTien,
					MaSoXe = z.MaSoXe,
					TaiXe = z.TaiXe,
					DonGiaKH = z.DonGiaKH,
					LoaiTienTeKH = z.LoaiTienTeKH,
					PhuPhiHD = 0,
					PhuPhiPhatSinh = z.PhuPhiPhatSinh,
					ContNo = z.ContNo,
					SealNP = z.SealNP,
					SealHQ = z.SealHQ,
					DiemLayRong = z.DiemLayRong,
					DiemTraRong = z.DiemTraRong,
				}).ToList(),
			}).ToList();

			foreach (var item in data)
			{
				decimal totalMoney = 0;
				double totalSf = 0;
				foreach (var handling in item.listBillHandlingWebs)
				{
					double sfByHandling = 0;
					getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == handling.handlingId).ToList().ForEach(async x =>
					{
						sfByHandling += x.sfp.Price * await _currencyExchange.GetPriceTradeNow(x.sfp.PriceType);
					});

					handling.DonGiaKH = handling.DonGiaKH * (decimal)await _currencyExchange.GetPriceTradeNow(handling.LoaiTienTeKH);
					totalMoney += handling.DonGiaKH;
					handling.PhuPhiHD = sfByHandling;
					totalSf += (double)sfByHandling;
				}

				var getListIdHandling = pagedData.Where(c => c.MaVanDon == item.MaVanDon).Select(c => c.handlingId).ToList();

				item.TongTien = totalMoney;
				item.TongPhuPhi = totalSf +
				await _context.SfeeByTcommand.Where(y => getListIdHandling.Contains(y.IdTcommand) && y.ApproveStatus == 14).SumAsync(y => y.Price);
			}

			return new PagedResponseCustom<ListBillTransportWeb>()
			{
				dataResponse = data,
				totalCount = totalCount,
				paginationFilter = validFilter
			};
		}

		public async Task<PagedResponseCustom<ListBillHandling>> GetListBillHandling(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getlistHandling = from dp in _context.DieuPhoi
								  join vd in _context.VanDon
								  on dp.MaVanDon equals vd.MaVanDon
								  where dp.TrangThai == 20
								  select new { dp, vd };

			var getSFbyContract = from sfc in _context.SubFeeByContract
								  join sfp in _context.SubFeePrice
								  on sfc.PriceId equals sfp.PriceId
								  select new { sfc, sfp };

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getlistHandling = getlistHandling.Where(x => x.vd.MaVanDonKh.Contains(filter.Keyword) || x.dp.MaChuyen.Contains(filter.Keyword));
			}

			var listHandling = new List<ListBillHandling>();

			if (!string.IsNullOrEmpty(filter.supplierId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.supplierId && x.MaHopDongCha == null && x.TrangThai == 24).FirstOrDefaultAsync();
				if (getListContractOfCus == null)
				{
					return new PagedResponseCustom<ListBillHandling>()
					{
						dataResponse = new List<ListBillHandling>(),
						totalCount = 0,
						paginationFilter = validFilter
					};
				}

				var dateBegin = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);
				var dateEnd = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				var checkDataKy = await _context.ChotSanLuongTheoKy.Where(x => x.KyChot == dateBegin.Month && x.MaKh == filter.supplierId).Select(x => x.MaDieuPhoi).ToListAsync();
				if (checkDataKy.Count > 0)
				{
					getlistHandling = getlistHandling.Where(x => checkDataKy.Contains(x.dp.Id));
				}
				else
				{
					getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.supplierId && x.dp.CreatedTime >= dateBegin && x.dp.CreatedTime <= dateEnd);
				}

				var getDataFilter = await getlistHandling.ToListAsync();

				if (getDataFilter.Count() > 0)
				{
					listHandling.AddRange(getDataFilter.Select(x => new ListBillHandling()
					{
						HangTau = x.vd.HangTau,
						handlingId = x.dp.Id,
						ContNo = x.dp.ContNo,
						ThoiGianHoanThanh = x.dp.ThoiGianHoanThanh,
						MaChuyen = x.dp.MaChuyen,
						AccountName = x.vd.MaAccount == null ? "" : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
						CutOffDate = x.vd.ThoiGianHaCang,
						DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemLayRong = x.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemTraRong = x.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						MaPTVC = x.vd.MaPtvc,
						MaVanDonKH = x.vd.MaVanDonKh,
						MaVanDon = x.dp.MaVanDon,
						LoaiVanDon = x.vd.LoaiVanDon,
						LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
						LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
						MaNCC = x.dp.DonViVanTai,
						MaKH = x.vd.MaKh,
						TenKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
						TenNCC = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
						DonGiaKH = x.dp.DonGiaKh,
						DonGiaNCC = x.dp.DonGiaNcc,
						LoaiTienTeKH = x.dp.LoaiTienTeKh,
						LoaiTienTeNCC = x.dp.LoaiTienTeNcc,
						Reuse = x.dp.ReuseCont == true ? "REUSE" : "",
						createdTime = x.dp.CreatedTime,
						ChiPhiHopDong = 0,
						ChiPhiPhatSinh = 0,
					}));
				}
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.customerId && x.MaHopDongCha == null && x.TrangThai == 24).FirstOrDefaultAsync();
				if (getListContractOfCus == null)
				{
					return new PagedResponseCustom<ListBillHandling>()
					{
						dataResponse = new List<ListBillHandling>(),
						totalCount = 0,
						paginationFilter = validFilter
					};
				}

				var dateBegin = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1);
				var dateEnd = new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value);
				if (dateEnd.DayOfWeek == DayOfWeek.Sunday)
				{
					dateEnd = dateEnd.AddDays(1);
				}

				var checkDataKy = await _context.ChotSanLuongTheoKy.Where(x => x.KyChot == dateBegin.Month && x.MaKh == filter.customerId).Select(x => x.MaDieuPhoi).ToListAsync();
				if (checkDataKy.Count > 0)
				{
					getlistHandling = getlistHandling.Where(x => checkDataKy.Contains(x.dp.Id));
				}
				else
				{
					getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId && x.dp.CreatedTime >= dateBegin && x.dp.CreatedTime <= dateEnd);
				}

				var getDataFilter = await getlistHandling.ToListAsync();

				if (getDataFilter.Count() > 0)
				{
					listHandling.AddRange(getDataFilter.Select(x => new ListBillHandling()
					{
						HangTau = x.vd.HangTau,
						handlingId = x.dp.Id,
						ContNo = x.dp.ContNo,
						ThoiGianHoanThanh = x.dp.ThoiGianHoanThanh,
						MaChuyen = x.dp.MaChuyen,
						AccountName = x.vd.MaAccount == null ? "" : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
						CutOffDate = x.vd.ThoiGianHaCang,
						DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemLayRong = x.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						DiemTraRong = x.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
						MaPTVC = x.vd.MaPtvc,
						MaVanDonKH = x.vd.MaVanDonKh,
						MaVanDon = x.dp.MaVanDon,
						LoaiVanDon = x.vd.LoaiVanDon,
						LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
						LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
						MaNCC = x.dp.DonViVanTai,
						MaKH = x.vd.MaKh,
						TenKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
						TenNCC = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
						DonGiaKH = x.dp.DonGiaKh,
						DonGiaNCC = x.dp.DonGiaNcc,
						LoaiTienTeKH = x.dp.LoaiTienTeKh,
						LoaiTienTeNCC = x.dp.LoaiTienTeNcc,
						Reuse = x.dp.ReuseCont == true ? "REUSE" : "",
						createdTime = x.dp.CreatedTime,
						ChiPhiHopDong = 0,
						ChiPhiPhatSinh = 0,
					}));
				}
			}

			if (!string.IsNullOrEmpty(filter.customerType))
			{
				if (filter.customerType != "KH" && filter.customerType != "NCC")
				{
					return new PagedResponseCustom<ListBillHandling>()
					{
						dataResponse = new List<ListBillHandling>(),
						totalCount = 0,
						paginationFilter = validFilter
					};
				}

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x =>
				x.MaHopDongCha == null
				&& x.TrangThai == 24
				&& x.MaLoaiHopDong == (filter.customerType == "KH" ? "SELL" : "BUY")).ToListAsync();

				foreach (var item in getListContractOfCus)
				{
					var getDataFilter = await getlistHandling.Where(x =>
					((filter.customerType == "KH" ? x.vd.MaKh : x.dp.DonViVanTai) == item.MaKh)
					&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
					&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value)).ToListAsync();

					if (getDataFilter.Count() > 0)
					{
						listHandling.AddRange(getDataFilter.Select(x => new ListBillHandling()
						{
							HangTau = x.vd.HangTau,
							handlingId = x.dp.Id,
							ContNo = x.dp.ContNo,
							ThoiGianHoanThanh = x.dp.ThoiGianHoanThanh,
							MaChuyen = x.dp.MaChuyen,
							AccountName = x.vd.MaAccount == null ? "" : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
							CutOffDate = x.vd.ThoiGianHaCang,
							DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
							DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
							DiemLayRong = x.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
							DiemTraRong = x.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
							MaPTVC = x.vd.MaPtvc,
							MaVanDonKH = x.vd.MaVanDonKh,
							MaVanDon = x.dp.MaVanDon,
							LoaiVanDon = x.vd.LoaiVanDon,
							LoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.dp.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
							LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
							MaNCC = x.dp.DonViVanTai,
							MaKH = x.vd.MaKh,
							TenKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
							TenNCC = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
							DonGiaKH = x.dp.DonGiaKh,
							DonGiaNCC = x.dp.DonGiaNcc,
							LoaiTienTeKH = x.dp.LoaiTienTeKh,
							LoaiTienTeNCC = x.dp.LoaiTienTeNcc,
							Reuse = x.dp.ReuseCont == true ? "REUSE" : "",
							createdTime = x.dp.CreatedTime,
							ChiPhiHopDong = 0,
							ChiPhiPhatSinh = 0,
						}));
					}
				}
			}

			var totalCount = listHandling.Count();
			var pagedData = listHandling.OrderByDescending(x => x.handlingId).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
			{
				DiemCuoi = x.DiemCuoi,
				DiemDau = x.DiemDau,
				DiemLayRong = x.DiemLayRong,
				createdTime = x.createdTime,
				HangTau = x.HangTau,
				MaVanDon = x.MaVanDon,
				handlingId = x.handlingId,
				ContNo = x.ContNo,
				AccountName = x.AccountName == null ? x.TenKH : x.AccountName,
				DiemTraRong = x.DiemTraRong,
				MaPTVC = x.MaPTVC,
				LoaiVanDon = x.LoaiVanDon,
				MaVanDonKH = x.MaVanDonKH,
				MaChuyen = x.MaChuyen,
				LoaiHangHoa = x.LoaiHangHoa,
				LoaiPhuongTien = x.LoaiPhuongTien,
				MaNCC = x.MaNCC,
				MaKH = x.MaKH,
				Reuse = x.Reuse,
				LoaiTienTeKH = x.LoaiTienTeKH,
				LoaiTienTeNCC = x.LoaiTienTeNCC,
				TenKH = x.TenKH,
				TenNCC = x.TenNCC,
				DonGiaKH = x.DonGiaKH,
				DonGiaNCC = x.DonGiaNCC,
				CutOffDate = x.CutOffDate,
				ThoiGianHoanThanh = x.ThoiGianHoanThanh,
				ChiPhiPhatSinh = x.ChiPhiPhatSinh,
			}).ToList();

			foreach (var item in pagedData)
			{
				double totalSf = 0;
				double totalSfI = 0;

				getSFbyContract.Where(x => x.sfc.MaDieuPhoi == item.handlingId && x.sfp.CusType == filter.customerType).ToList().ForEach(async x =>
				{
					var price = x.sfp.Price * await _currencyExchange.GetPriceTradeNow(x.sfp.PriceType);
					totalSf += price;

					if (filter.maptvc == "GetAllSubfeeExportExcel")
					{
						item.ListSubfeeContract += await _context.SubFee.Where(x => x.SubFeeId == x.SubFeeId).Select(x => x.SfName).FirstOrDefaultAsync() + ": " + price + "\r\n";
					}
				});

				var getListSfI = await _context.SfeeByTcommand.Where(y => y.IdTcommand == item.handlingId && y.ApproveStatus == 14).ToListAsync();

				foreach (var sfi in getListSfI)
				{
					var price = sfi.Price;
					totalSfI += price;

					if (filter.maptvc == "GetAllSubfeeExportExcel")
					{
						item.ListSubfeeIncurred += await _context.SubFee.Where(x => x.SubFeeId == sfi.SfId).Select(x => x.SfName).FirstOrDefaultAsync() + ": " + price + "\r\n";
					}
				}

				item.ChiPhiPhatSinh = (decimal)totalSfI;
				item.ChiPhiHopDong = (decimal)totalSf;
				item.DoanhThu = (item.DonGiaKH.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.LoaiTienTeKH)) + item.ChiPhiPhatSinh + item.ChiPhiHopDong;
				item.LoiNhuan = (item.DonGiaKH.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.LoaiTienTeKH)) - (item.DonGiaNCC.Value * (decimal)await _currencyExchange.GetPriceTradeNow(item.LoaiTienTeNCC));
			}

			return new PagedResponseCustom<ListBillHandling>()
			{
				dataResponse = pagedData,
				totalCount = totalCount,
				paginationFilter = validFilter
			};
		}
	}
}