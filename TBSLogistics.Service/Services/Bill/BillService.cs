using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

namespace TBSLogistics.Service.Services.Bill
{
	public class BillService : IBill
	{
		private readonly TMSContext _context;
		private readonly ICommon _common;
		private readonly IPriceTable _priceTable;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public BillService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor, IPriceTable priceTable)
		{
			_priceTable = priceTable;
			_context = context;
			_common = common;
			_httpContextAccessor = httpContextAccessor;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<GetBill> GetBillByCustomerId(string customerId, DateTime datePay)
		{
			try
			{
				var getlistHandling = from dp in _context.DieuPhoi
									  join vd in _context.VanDon
									  on dp.MaVanDon equals vd.MaVanDon
									  where dp.TrangThai == 20
									  select new { vd, dp };

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

				if (customerId.Trim().Substring(0, 3) == "CUS")
				{
					getlistHandling = getlistHandling.Where(x => x.vd.MaKh == customerId);
				}
				if (customerId.Trim().Substring(0, 3) == "SUP")
				{
					getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == customerId);
				}

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime >= new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
				&& x.dp.CreatedTime <= new DateTime(datePay.Year, datePay.Month, getListContractOfCus.NgayThanhToan.Value));

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
					listHandling = getlistHandling.Where(y => y.vd.MaVanDon == z.vd.MaVanDon).OrderBy(x => x.dp.Id).Select(x => new ListHandling()
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
						DonGia = x.dp.DonGiaKh.Value,
						LoaiTienTe = x.dp.LoaiTienTeKh,
						KhoiLuong = x.dp.KhoiLuong,
						TheTich = x.dp.TheTich,
						SoKien = x.dp.SoKien,
						listSubFeeByContract = getListSFOfContract.Where(y => y.sfc.MaDieuPhoi == x.dp.Id && y.sfp.CusType == (customerId.Substring(0, 3) == "CUS" ? "KH" : "NCC")).Select(x => new ListSubFeeByContract()
						{
							ContractId = x.sfp.ContractId,
							ContractName = _context.HopDongVaPhuLuc.Where(c => c.MaHopDong == x.sfp.ContractId).Select(c => c.TenHienThi).FirstOrDefault(),
							sfName = x.sf.SfName,
							goodsType = x.sfp.GoodsType,
							unitPrice = x.sfp.Price,
							priceType = x.sfp.PriceType
						}).ToList(),
						listSubFeeIncurreds = _context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).OrderBy(x => x.Id).Select(x => new ListSubFeeIncurred()
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
						iHandling.GiaQuyDoi = iHandling.DonGia * (decimal)await _priceTable.GetPriceTradeNow(iHandling.LoaiTienTe);
						iHandling.listSubFeeByContract.ForEach(async x =>
						{
							x.priceTransfer = x.unitPrice * (double)await _priceTable.GetPriceTradeNow(x.priceType);
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

		public async Task<GetBill> GetBillByTransportId(string transportId, long? handlingId)
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
						DonGia = x.DonGiaKh.Value,
						LoaiTienTe = x.LoaiTienTeKh,
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
						iHandling.GiaQuyDoi = iHandling.DonGia * (decimal)await _priceTable.GetPriceTradeNow(iHandling.LoaiTienTe);
						iHandling.listSubFeeByContract.ForEach(async x =>
						{
							x.priceTransfer = x.unitPrice * (double)await _priceTable.GetPriceTradeNow(x.priceType);
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

		public async Task<PagedResponseCustom<ListVanDon>> GetListTransportByCustomerId(string customerId, int ky, PaginationFilter filter)
		{
			var checkexists = await GetListKyThanhToan(customerId);

			if (checkexists == null)
			{
				return null;
			}

			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var kyThanhToan = checkexists.Where(x => x.Ky == ky).FirstOrDefault();

			var getlistHandling = from dp in _context.DieuPhoi
								  join vd in _context.VanDon
								  on dp.MaVanDon equals vd.MaVanDon
								  where
								  (vd.MaKh == customerId || dp.DonViVanTai == customerId)
								  && dp.TrangThai == 20
								  && dp.CreatedTime.Date >= kyThanhToan.StartDate.Date
								  && dp.CreatedTime.Date <= kyThanhToan.EndDate.Date
								  select dp;

			var listData = from kh in _context.KhachHang
						   join vd in _context.VanDon
						   on kh.MaKh equals vd.MaKh
						   where getlistHandling.Select(x => x.MaVanDon).Contains(vd.MaVanDon)
						   orderby vd.CreatedTime
						   select new { kh, vd };

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword));
			}

			var totalCount = await listData.CountAsync();

			var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListVanDon()
			{
				MaVanDon = x.vd.MaVanDon,
				MaKh = x.kh.MaKh,
				TenKh = x.kh.TenKh,
				LoaiVanDon = x.vd.LoaiVanDon,
				DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
				DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
				TongTheTich = x.vd.TongTheTich.Value,
				TongKhoiLuong = x.vd.TongKhoiLuong.Value
			}).OrderBy(x => x.MaKh).ToListAsync();

			return new PagedResponseCustom<ListVanDon>()
			{
				dataResponse = pagedData,
				totalCount = totalCount,
				paginationFilter = validFilter
			};
		}

		public async Task<List<KyThanhToan>> GetListKyThanhToan(string customerId)
		{
			var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == customerId && x.MaHopDongCha == null && x.NgayThanhToan != null).FirstOrDefaultAsync();

			if (getContract == null)
			{
				return null;
			}

			var ngayThanhToan = getContract.NgayThanhToan.Value;
			var dateNow = DateTime.Now.Date;
			var timeStart = getContract.ThoiGianBatDau.Date;
			var allDates = new List<DateTime>();

			for (DateTime date = timeStart; date <= dateNow; date = date.AddDays(1))
			{
				allDates.Add(date);
			}

			var listKy = new List<KyThanhToan>();
			int count = 0;
			foreach (var date in allDates)
			{
				if (date.Date.Day == ngayThanhToan)
				{
					count += 1;

					listKy.Add(new KyThanhToan()
					{
						Ky = count,
						StartDate = date.Date.AddDays(1),
						EndDate = new DateTime(date.Date.Year, date.Date.AddMonths(1).Month, ngayThanhToan)
					});
				}
			}

			return listKy;
		}

		public async Task<PagedResponseCustom<ListBillTransportWeb>> GetListBillWeb(PaginationFilter filter)
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

			if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
			{
				getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value && x.dp.CreatedTime.Date <= filter.toDate.Value);
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId
				&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
				&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value));
			}
			else
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDongCha == null && x.MaLoaiHopDong == "SELL").ToListAsync();

				foreach (var item in getListContractOfCus)
				{
					var getDataFilter = await getlistHandling.Where(x =>
					x.vd.MaKh == item.MaKh
					&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
					&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value)).ToListAsync();

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
						sfByHandling += x.sfp.Price * await _priceTable.GetPriceTradeNow(x.sfp.PriceType);
					});

					handling.DonGiaKH = handling.DonGiaKH * (decimal)await _priceTable.GetPriceTradeNow(handling.LoaiTienTeKH);
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

			if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
			{
				getlistHandling = getlistHandling.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value && x.dp.CreatedTime.Date <= filter.toDate.Value);
			}

			if (!string.IsNullOrEmpty(filter.supplierId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.supplierId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				getlistHandling = getlistHandling.Where(x => x.dp.DonViVanTai == filter.supplierId
				&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
				&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value));
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == filter.customerId && x.MaHopDongCha == null).FirstOrDefaultAsync();
				getlistHandling = getlistHandling.Where(x => x.vd.MaKh == filter.customerId
				&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
				&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, getListContractOfCus.NgayThanhToan.Value));
			}

			var listHadling = new List<ListBillHandling>();
			if (!string.IsNullOrEmpty(filter.customerType))
			{
				if (filter.customerType != "KH" && filter.customerType != "NCC")
				{
					return null;
				}

				var getListContractOfCus = await _context.HopDongVaPhuLuc.Where(x =>
				x.MaHopDongCha == null
				&& x.MaLoaiHopDong == (filter.customerType == "KH" ? "SELL" : "BUY")).ToListAsync();

				foreach (var item in getListContractOfCus)
				{
					var getDataFilter = await getlistHandling.Where(x =>
					(filter.customerType == "KH" ? x.vd.MaKh : x.dp.DonViVanTai) == item.MaKh
					&& x.dp.CreatedTime >= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value).AddMonths(-1).AddDays(1)
					&& x.dp.CreatedTime <= new DateTime(filter.date.Year, filter.date.Month, item.NgayThanhToan.Value)).ToListAsync();

					if (getDataFilter.Count() > 0)
					{
						listHadling.AddRange(getDataFilter.Select(x => new ListBillHandling()
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
							ChiPhiHopDong = 0,
							ChiPhiPhatSinh = (decimal)_context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.Price),
						}));
					}
				}
			}

			var totalCount = listHadling.Count();
			var pagedData = listHadling.OrderByDescending(x => x.handlingId).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListBillHandling()
			{
				DiemCuoi = x.DiemCuoi,
				DiemDau = x.DiemDau,
				DiemLayRong = x.DiemLayRong,
				createdTime = x.createdTime,
				HangTau = x.HangTau,
				MaVanDon = "",
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

				getSFbyContract.Where(x => x.sfc.MaDieuPhoi == item.handlingId && x.sfp.CusType == "NCC").ToList().ForEach(async x =>
				{
					totalSf += x.sfp.Price * await _priceTable.GetPriceTradeNow(x.sfp.PriceType);
				});

				item.ChiPhiHopDong = (decimal)totalSf;
				item.DoanhThu = (item.DonGiaKH.Value * (decimal)await _priceTable.GetPriceTradeNow(item.LoaiTienTeKH)) + item.ChiPhiPhatSinh + item.ChiPhiHopDong;
				item.LoiNhuan = (item.DonGiaKH.Value * (decimal)await _priceTable.GetPriceTradeNow(item.LoaiTienTeKH)) - (item.DonGiaNCC.Value * (decimal)await _priceTable.GetPriceTradeNow(item.LoaiTienTeNCC));
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