using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

namespace TBSLogistics.Service.Services.PriceTableManage
{
	public class PriceTableService : IPriceTable
	{
		private readonly TMSContext _context;
		private readonly ICommon _common;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public PriceTableService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_common = common;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
		{
			try
			{
				string ErrorValidate = "";
				int row = 0;
				foreach (var item in request)
				{
					row++;
					if (!string.IsNullOrEmpty(item.NgayHetHieuLuc.ToString()))
					{
						if (item.NgayHetHieuLuc.Value.Date <= DateTime.Now.Date)
						{
							ErrorValidate += "Dòng " + row + ": Ngày hết hiệu lực không được nhỏ hôm nay";
						}
					}

					if (item.MaPtvc == "FCL" || item.MaPtvc == "LCL")
					{
						if (item.MaLoaiPhuongTien.Contains("TRUCK"))
						{
							ErrorValidate += "Dòng " + row + ": Mã phương tiện không đúng với phương thức vận chuyển";
						}

						if (item.DiemLayTraRong == null)
						{
							ErrorValidate += "Dòng " + row + ": Vui lòng chọn điểm lấy/trả rỗng";
						}
					}

					if (item.MaPtvc == "FTL" || item.MaPtvc == "LTL")
					{
						if (item.MaLoaiPhuongTien.Contains("CONT"))
						{
							ErrorValidate += "Dòng " + row + ": Mã phương tiện không đúng với phương thức vận chuyển";
						}
						item.DiemLayTraRong = null;
					}

					if (!string.IsNullOrEmpty(item.AccountId))
					{
						var checkAccount = await _context.KhachHangAccount.Where(x => x.MaAccount == item.AccountId && x.MaKh == item.MaKH).FirstOrDefaultAsync();
						if (checkAccount == null)
						{
							ErrorValidate += "Dòng " + row + ": Account " + item.AccountId + " không tồn tại";
						}
					}
				}

				int rows = 1;
				foreach (var item in request)
				{
					rows++;

					var checkDuplicate = request.Where(x =>
					x.DiemLayTraRong == item.DiemLayTraRong
					&& x.AccountId == item.AccountId
					&& x.DiemDau == item.DiemDau
					&& x.DiemCuoi == item.DiemCuoi
					&& x.MaHopDong == item.MaHopDong
					&& x.MaKH == item.MaKH
					&& x.MaPtvc == item.MaPtvc
					&& x.MaLoaiPhuongTien == item.MaLoaiPhuongTien
					&& x.MaDvt == item.MaDvt
					&& x.MaLoaiHangHoa == item.MaLoaiHangHoa
					).ToList();

					if (checkDuplicate.Count > 1)
					{
						ErrorValidate += "Dòng " + rows + ": Bị trùng lặp, vui lòng kiểm tra lại! ";
					}

					if (item.DiemDau == item.DiemCuoi)
					{
						ErrorValidate += "Điểm đóng hàng không được giống điểm hạ hàng: ";
					}

					var checkPriceType = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == item.LoaiTienTe).FirstOrDefaultAsync();
					if (checkPriceType == null)
					{
						ErrorValidate += "Loại tiền tiện không được hỗ trợ: " + string.Join(",", item.LoaiTienTe);
					}

					var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == item.MaHopDong && x.MaKh == item.MaKH && x.TrangThai == 49).FirstOrDefaultAsync();
					if (checkContract == null)
					{
						ErrorValidate += "Hợp đồng không thể thêm bảng giá: " + string.Join(",", item.MaHopDong);
					}

					//var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == checkContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();
					//if (getNewestContract.MaHopDong != checkContract.MaHopDong)
					//{
					//    ErrorValidate += "Vui Lòng Chọn Hợp Đồng Mới Nhất: " + string.Join(",", getNewestContract.MaHopDong);
					//}

					var firstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemDau).FirstOrDefaultAsync();
					if (firstPlace == null)
					{
						ErrorValidate += "Điểm Đóng Hàng không tồn tại: " + string.Join(",", item.DiemDau);
					}

					var secondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemCuoi).FirstOrDefaultAsync();
					if (secondPlace == null)
					{
						ErrorValidate += "Điểm Hạ Hàng không tồn tại: " + string.Join(",", item.DiemCuoi);
					}

					if (item.DiemLayTraRong.HasValue)
					{
						var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemLayTraRong).FirstOrDefaultAsync();
						if (firstPlace == null)
						{
							ErrorValidate += "Điểm Lấy/Trả rỗng không tồn tại: " + string.Join(",", item.DiemLayTraRong);
						}
					}

					var checkPtvc = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == item.MaPtvc).FirstOrDefaultAsync();
					if (checkPtvc == null)
					{
						ErrorValidate += "Mã phương thức vận chuyển không tồn tại: " + string.Join(",", item.MaPtvc);
					}

					var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.MaLoaiPhuongTien).FirstOrDefaultAsync();

					if (checkVehicleType == null)
					{
						ErrorValidate += "Mã phương tiện vận chuyển không tồn tại: " + string.Join(",", item.MaLoaiPhuongTien);
					}

					//var checkDVT = await _context.DonViTinh.Where(x => request.Select(y => y.MaDvt).Contains(x.MaDvt)).ToListAsync();
					//var checkExistsDVT = request.Where(x => !checkDVT.Any(y => y.MaDvt == x.MaDvt)).Select(x => x.MaDvt);
					//if (checkExistsDVT.Count() > 0)
					//{
					//    ErrorValidate += "Mã đơn vị tính không tồn tại: " + string.Join(",", checkExistsDVT);
					//}

					var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.MaLoaiHangHoa).FirstOrDefaultAsync();

					if (checkGoodsType == null)
					{
						ErrorValidate += "Mã loại hàng hóa không tồn tại: " + string.Join(",", item.MaLoaiHangHoa);
					}
				}

				if (ErrorValidate != "")
				{
					return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
				}

				await _context.BangGia.AddRangeAsync(request.Select(x => new BangGia
				{
					DiemLayTraRong = x.DiemLayTraRong,
					DiemCuoi = x.DiemCuoi,
					MaAccount = x.AccountId,
					DiemDau = x.DiemDau,
					MaHopDong = x.MaHopDong,
					MaPtvc = x.MaPtvc,
					MaLoaiPhuongTien = x.MaLoaiPhuongTien,
					DonGia = x.DonGia,
					LoaiTienTe = x.LoaiTienTe.ToUpper(),
					MaDvt = x.MaDvt,
					MaLoaiHangHoa = x.MaLoaiHangHoa,
					NgayApDung = _context.HopDongVaPhuLuc.Where(y => y.MaHopDong == x.MaHopDong).Select(x => x.ThoiGianBatDau).FirstOrDefault(),
					NgayHetHieuLuc = x.NgayHetHieuLuc,
					MaLoaiDoiTac = _context.KhachHang.Where(y => y.MaKh == x.MaKH).Select(y => y.MaLoaiKh).FirstOrDefault(),
					TrangThai = 3,
					CreatedTime = DateTime.Now,
					UpdatedTime = DateTime.Now,
					Creator = tempData.UserName,
				}).ToList());

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Create PriceTable with Data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Tạo mới bảng giá thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Tạo mới bảng giá thất bại" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " create new PriceTable with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}

		public async Task<PagedResponseCustom<ListCustomerOfPriceTable>> GetListPriceTable(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getData = from cus in _context.KhachHang
						  select cus;

			var getContract = from hd in _context.HopDongVaPhuLuc
							  select hd;

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getData = getData.Where(x => x.TenKh.Contains(filter.Keyword) || x.MaKh.Contains(filter.Keyword));
			}

			if (!string.IsNullOrEmpty(filter.customerType))
			{
				getData = getData.Where(x => x.MaLoaiKh == filter.customerType);
			}

			var totalRecords = await getData.CountAsync();

			var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListCustomerOfPriceTable()
			{
				TenChuoi = _context.ChuoiKhachHang.Where(c => c.MaChuoi == x.Chuoi).Select(c => c.TenChuoi).FirstOrDefault(),
				MaKH = x.MaKh,
				TenKH = x.TenKh,
				MaSoThue = x.MaSoThue,
				SoDienThoai = x.Sdt,
				listContractOfCustomers = getContract.Where(y => y.MaKh == x.MaKh).Select(y => new ListContractOfCustomer()
				{
					MaKH = y.MaKh,
					MaHopDong = y.MaHopDong,
					TenHopDong = y.TenHienThi,
					LoaiHinhHopTac = y.LoaiHinhHopTac,
					SanPhamDichVu = _context.LoaiSpdv.Where(c => c.MaLoaiSpdv == y.MaLoaiSpdv).Select(c => c.TenLoaiSpdv).FirstOrDefault(),
				}).ToList()
			}).ToListAsync();

			return new PagedResponseCustom<ListCustomerOfPriceTable>()
			{
				paginationFilter = validFilter,
				totalCount = totalRecords,
				dataResponse = pagedData
			};
		}

		public async Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, string onlyContractId, ListFilter listFilter, PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getList = from bg in _context.BangGia
						  join hd in _context.HopDongVaPhuLuc
						  on bg.MaHopDong equals hd.MaHopDong
						  join kh in _context.KhachHang
						  on hd.MaKh equals kh.MaKh
						  where (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
						  && bg.NgayApDung.Date <= DateTime.Now.Date
						  && bg.TrangThai == 4
						  orderby bg.NgayApDung descending
						  select new { bg, hd, kh };

			if (!string.IsNullOrEmpty(onlyContractId))
			{
				getList = getList.Where(x => x.hd.MaHopDong == contractId);
			}
			else
			{
				var checkContractChild = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == contractId).FirstOrDefaultAsync();
				if (checkContractChild == null)
				{
					return null;
				}

				if (checkContractChild.MaHopDongCha == null)
				{
					var listContract = getList.Where(x => x.hd.MaHopDong == contractId || x.hd.MaHopDongCha == contractId).Select(x => x.hd.MaHopDong);
					getList = getList.Where(x => listContract.Contains(x.bg.MaHopDong));
				}
				else
				{
					var listContract = getList.Where(x => x.hd.MaHopDong == checkContractChild.MaHopDongCha || x.hd.MaHopDongCha == checkContractChild.MaHopDongCha).Select(x => x.hd.MaHopDong);
					getList = getList.Where(x => listContract.Contains(x.bg.MaHopDong));
				}
			}

			if (listFilter.listDiemDau.Count > 0)
			{
				getList = getList.Where(x => listFilter.listDiemDau.Contains(x.bg.DiemDau));
			}

			if (listFilter.listDiemCuoi.Count > 0)
			{
				getList = getList.Where(x => listFilter.listDiemCuoi.Contains(x.bg.DiemCuoi));
			}

			if (listFilter.accountIds.Count > 0)
			{
				getList = getList.Where(x => listFilter.accountIds.Contains(x.bg.MaAccount));
			}

			if (listFilter.listDiemLayTraRong.Count > 0)
			{
				getList = getList.Where(x => listFilter.listDiemLayTraRong.Contains(x.bg.DiemLayTraRong));
			}

			if (!string.IsNullOrEmpty(filter.goodsType))
			{
				getList = getList.Where(x => x.bg.MaLoaiHangHoa == filter.goodsType);
			}

			if (!string.IsNullOrEmpty(filter.vehicleType))
			{
				getList = getList.Where(x => x.bg.MaLoaiPhuongTien == filter.vehicleType);
			}

			var totalRecords = await getList.CountAsync();

			var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetPriceListRequest()
			{
				ID = x.bg.Id,
				AccountName = x.bg.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.bg.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
				DiemLayTraRong = x.bg.DiemLayTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemLayTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
				MaKh = x.hd.MaKh,
				TenKH = x.kh.TenKh,
				MaHopDong = x.bg.MaHopDong,
				NgayApDung = x.bg.NgayApDung,
				NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
				LoaiTienTe = x.bg.LoaiTienTe,
				DonGia = x.bg.DonGia,
				MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
				MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
				MaDVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
				MaPTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
				SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
				MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
				TrangThai = x.bg.TrangThai,
			}).OrderByDescending(x => x.NgayApDung).ToListAsync();

			return new PagedResponseCustom<GetPriceListRequest>()
			{
				paginationFilter = validFilter,
				totalCount = totalRecords,
				dataResponse = pagedData
			};
		}

		public async Task<List<GetPriceListRequest>> GetListPriceTableByCustommerId(string MaKH)
		{
			var listPriceTable = from bg in _context.BangGia
								 join hd in _context.HopDongVaPhuLuc
								 on bg.MaHopDong equals hd.MaHopDong
								 where
								 bg.NgayApDung.Date <= DateTime.Now.Date
								 && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
								 && bg.TrangThai == 4
								 && hd.MaKh == MaKH
								 orderby bg.Id descending
								 select new { bg, hd };

			var gr = from t in listPriceTable
					 group t by new { t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac, t.bg.DiemDau, t.bg.DiemCuoi, t.bg.DiemLayTraRong }
					 into g
					 select new
					 {
						 g.Key.DiemCuoi,
						 g.Key.DiemDau,
						 g.Key.DiemLayTraRong,
						 g.Key.MaDvt,
						 g.Key.MaLoaiHangHoa,
						 g.Key.MaLoaiPhuongTien,
						 g.Key.MaPtvc,
						 g.Key.MaLoaiDoiTac,
						 Id = (from t2 in g select t2.bg.Id).Max(),
					 };

			listPriceTable = listPriceTable.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

			return await listPriceTable.Select(x => new GetPriceListRequest()
			{
				ID = x.bg.Id,
				DiemLayTraRong = x.bg.DiemLayTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemLayTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
				MaKh = x.hd.MaKh,
				MaHopDong = x.bg.MaHopDong,
				NgayApDung = x.bg.NgayApDung,
				NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
				DonGia = x.bg.DonGia,
				LoaiTienTe = x.bg.LoaiTienTe,
				MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
				MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
				MaDVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
				MaPTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
				SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
				MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
				TrangThai = x.bg.TrangThai,
			}).ToListAsync();
		}

		public async Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(string contractId, PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getData = from kh in _context.KhachHang
						  join hd in _context.HopDongVaPhuLuc
						  on kh.MaKh equals hd.MaKh
						  join bg in _context.BangGia
						  on hd.MaHopDong equals bg.MaHopDong
						  orderby bg.CreatedTime descending
						  select new { kh, hd, bg };

			if (!string.IsNullOrEmpty(contractId))
			{
				getData = getData.Where(x => x.hd.MaHopDong == contractId.Trim());
			}

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getData = getData.Where(x => x.hd.MaHopDong.Contains(filter.Keyword) || x.hd.MaKh.Contains(filter.Keyword));
			}

			if (filter.AlmostExpired == true)
			{
				getData = getData.Where(x => x.bg.TrangThai == 4 && x.bg.NgayHetHieuLuc != null && x.bg.NgayHetHieuLuc.Value.Date < DateTime.Now.AddDays(7).Date);
			}
			else
			{
				getData = getData.Where(x => x.bg.TrangThai == 3);
			}

			var totalRecords = await getData.CountAsync();

			var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListApprove()
			{
				Id = x.bg.Id,
				AccountName = x.bg.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.bg.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
				DiemLayTraRong = x.bg.DiemLayTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemLayTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
				MaKh = x.kh.MaKh,
				TenKh = x.kh.TenKh,
				DonGia = x.bg.DonGia,
				LoaiTienTe = x.bg.LoaiTienTe,
				MaHopDong = x.hd.MaHopDong,
				TenHopDong = x.hd.TenHienThi,
				PTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
				MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
				DVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
				MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
				MaLoaiDoiTac = _context.LoaiKhachHang.Where(y => y.MaLoaiKh == x.bg.MaLoaiDoiTac).Select(x => x.TenLoaiKh).FirstOrDefault(),
				NgayApDung = x.bg.NgayApDung.ToString("dd-MM-yyyy"),
				NgayHetHieuLuc = x.bg.NgayHetHieuLuc == null ? null : x.bg.NgayHetHieuLuc.Value.ToString("dd-MM-yyyy"),
				ThoiGianTao = x.bg.CreatedTime.ToString("dd-MM-yyyy HH:mm:ss"),
			}).ToListAsync();

			return new PagedResponseCustom<ListApprove>()
			{
				paginationFilter = validFilter,
				totalCount = totalRecords,
				dataResponse = pagedData
			};
		}

		public async Task<BoolActionResult> ApprovePriceTable(ApprovePriceTable request)
		{
			try
			{
				if (request.Result.Count < 1)
				{
					return new BoolActionResult { isSuccess = false, Message = "Không có bảng giá nào được chọn, vui lòng xem lại" };
				}

				foreach (var item in request.Result)
				{
					var checkExists = from ct in _context.HopDongVaPhuLuc
									  join bg in _context.BangGia
									  on ct.MaHopDong equals bg.MaHopDong
									  where bg.Id == item.Id
									  select new { ct, bg };

					if (checkExists == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Tồn tại bảng giá không có trong hệ thống, vui lòng xem lại" };
					}

					var dataPriceTable = await checkExists.FirstOrDefaultAsync();
					var getListContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == dataPriceTable.ct.MaKh).Select(x => x.MaHopDong).ToListAsync();

					if (item.IsAgree == 1)
					{
						dataPriceTable.bg.TrangThai = 5;
						_context.BangGia.Update(dataPriceTable.bg);
					}

					if (item.IsAgree == 0)
					{
						var checkOldPriceTable = await _context.BangGia.Where(x =>
					  getListContract.Contains(x.MaHopDong) &&
					  x.MaAccount == dataPriceTable.bg.MaAccount &&
					  x.DiemDau == dataPriceTable.bg.DiemDau &&
					  x.DiemCuoi == dataPriceTable.bg.DiemCuoi &&
					  x.DiemLayTraRong == dataPriceTable.bg.DiemLayTraRong &&
					  x.MaPtvc == dataPriceTable.bg.MaPtvc &&
					  x.MaLoaiPhuongTien == dataPriceTable.bg.MaLoaiPhuongTien &&
					  x.MaDvt == dataPriceTable.bg.MaDvt &&
					  x.MaLoaiHangHoa == dataPriceTable.bg.MaLoaiHangHoa &&
					  x.MaLoaiDoiTac == dataPriceTable.bg.MaLoaiDoiTac &&
					  x.TrangThai == 4
					  ).ToListAsync();

						if (checkOldPriceTable != null)
						{
							checkOldPriceTable.ForEach(x => { x.TrangThai = 6; x.NgayHetHieuLuc = DateTime.Now; x.Approver = tempData.UserName; });
						}

						dataPriceTable.bg.TrangThai = 4;
						dataPriceTable.bg.Approver = tempData.UserName;
						dataPriceTable.bg.NgayApDung = DateTime.Now;
						_context.BangGia.Update(dataPriceTable.bg);
						await _context.SaveChangesAsync();
					}
				}
				await _common.LogTimeUsedOfUser(tempData.Token);
				await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Approve PriceTable with Data: " + JsonSerializer.Serialize(request));
				return new BoolActionResult { isSuccess = true, Message = "Duyệt bảng giá thành công" };
			}
			catch (Exception ex)
			{
				await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " Approve PriceTable with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		public async Task<GetPriceListById> GetPriceTableById(int id)
		{
			try
			{
				var data = from hd in _context.HopDongVaPhuLuc
						   join bg in _context.BangGia
						   on hd.MaHopDong equals bg.MaHopDong
						   where bg.Id == id
						   select new { hd, bg };

				if (data == null)
				{
					return null;
				}

				var reuslt = await data.Select(x => new GetPriceListById()
				{
					ID = x.bg.Id,
					AccountId = x.bg.MaAccount,
					DiemDau = x.bg.DiemDau,
					DiemCuoi = x.bg.DiemCuoi,
					DiemLayTraRong = x.bg.DiemLayTraRong,
					MaHopDong = x.bg.MaHopDong,
					SoHopDongCha = x.hd.MaHopDongCha,
					MaKh = x.hd.MaKh,
					DonGia = x.bg.DonGia,
					LoaiTienTe = x.bg.LoaiTienTe,
					MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
					MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
					MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
					MaDVT = x.bg.MaDvt,
					MaPTVC = x.bg.MaPtvc,
					NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
				}).FirstOrDefaultAsync();

				return reuslt;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<BoolActionResult> UpdatePriceTable(int id, GetPriceListById request)
		{
			try
			{
				var findById = await _context.BangGia.Where(x => x.Id == id).FirstOrDefaultAsync();

				if (findById == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Bảng giá không tồn tại" };
				}

				var checkValid = await ValidateEdit(request.DiemDau, request.DiemCuoi, request.DiemLayTraRong, request.MaHopDong, request.MaPTVC, request.MaLoaiPhuongTien, request.DonGia, request.MaDVT, request.MaLoaiHangHoa, request.NgayHetHieuLuc);

				if (checkValid != "")
				{
					return new BoolActionResult { isSuccess = false, Message = checkValid };
				}

				if (request.MaPTVC == "FCL" || request.MaPTVC == "LCL")
				{
					if (request.MaLoaiPhuongTien.Contains("TRUCK"))
					{
						return new BoolActionResult { isSuccess = false, Message = " Mã phương tiện không đúng với phương thức vận chuyển" };
					}
				}

				if (request.MaPTVC == "FTL" || request.MaPTVC == "LTL")
				{
					if (request.MaLoaiPhuongTien.Contains("CONT"))
					{
						return new BoolActionResult { isSuccess = false, Message = "Mã phương tiện không đúng với phương thức vận chuyển" };
					}

					request.DiemLayTraRong = null;
				}

				var checkPriceType = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == request.LoaiTienTe).FirstOrDefaultAsync();
				if (checkPriceType == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Loại tiền tiện không được hỗ trợ" };
				}

				if (findById.TrangThai != 3)
				{
					return new BoolActionResult { isSuccess = false, Message = "Không thể chỉnh sửa bảng giá này nữa" };
				}

				if (request.DiemDau == request.DiemCuoi)
				{
					return new BoolActionResult { isSuccess = false, Message = "Điểm đóng hàng không được giống điểm hạ hàng" };
				}

				var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.MaHopDong && x.TrangThai == 49).FirstOrDefaultAsync();
				if (checkContract == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này không thể sửa giá" };
				}

				//var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == checkContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();
				//if (getNewestContract.MaHopDong != checkContract.MaHopDong)
				//{
				//	return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn Phụ lục mới nhất" };
				//}

				var checkPriceTable = await _context.BangGia.Where(x =>
				   x.DiemDau == request.DiemDau &&
				   x.MaAccount == request.AccountId &&
				   x.DiemCuoi == request.DiemCuoi &&
				   x.DiemLayTraRong == request.DiemLayTraRong &&
				   x.MaHopDong == request.MaHopDong &&
				   x.DonGia == request.DonGia &&
				   x.LoaiTienTe == request.LoaiTienTe &&
				   x.MaPtvc == request.MaPTVC &&
				   x.MaLoaiPhuongTien == request.MaLoaiPhuongTien &&
				   x.MaDvt == request.MaDVT &&
				   x.MaLoaiHangHoa == request.MaLoaiHangHoa &&
				   x.TrangThai == 4
				   ).FirstOrDefaultAsync();

				if (checkPriceTable != null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Dữ liệu muốn cập nhật đã tồn tại và đang được áp dụng" };
				}

				findById.MaAccount = request.AccountId;
				findById.DiemCuoi = request.DiemCuoi;
				findById.DiemDau = request.DiemDau;
				findById.DiemLayTraRong = request.DiemLayTraRong;
				findById.MaHopDong = request.MaHopDong;
				findById.DonGia = request.DonGia;
				findById.LoaiTienTe = request.LoaiTienTe;
				findById.MaDvt = request.MaDVT;
				findById.MaPtvc = request.MaPTVC;
				findById.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
				findById.MaLoaiHangHoa = request.MaLoaiHangHoa;
				findById.NgayHetHieuLuc = request.NgayHetHieuLuc;
				findById.Updater = tempData.UserName;
				findById.UpdatedTime = DateTime.Now;

				_context.Update(findById);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Update PriceTable with Data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật bảng giá thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Cập nhật bảng giá thất bại" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " Update PriceTable with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		private async Task<string> ValidateEdit(int DiemDau, int DiemCuoi, int? DiemLayTraRong, string MaHopDong, string MaPTVC, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, DateTime? NgayHetHieuLuc, string ErrorRow = "")
		{
			string ErrorValidate = "";

			var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == MaHopDong).FirstOrDefaultAsync();
			if (checkContract == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã hợp đồng không tồn tại \r\n" + Environment.NewLine;
			}

			var checkFirstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiemDau).FirstOrDefaultAsync();
			if (checkFirstPlace == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Điểm Đóng Hàng không tồn tại \r\n" + Environment.NewLine;
			}

			var checkSecondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiemCuoi).FirstOrDefaultAsync();
			if (checkSecondPlace == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Điểm Hạ Hàng \r\n" + Environment.NewLine;
			}

			if (DiemLayTraRong.HasValue)
			{
				var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == DiemLayTraRong).FirstOrDefaultAsync();
				if (getEmptyPlace == null)
				{
					ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Điểm Lấy Trả rỗng không tồn tại \r\n" + Environment.NewLine;
				}
			}

			var checkMaLoaiPhuongTien = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

			if (checkMaLoaiPhuongTien == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện không tồn tại \r\n" + Environment.NewLine;
			}

			if (DonGia < 0)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đơn giá không được để trong và phải lớn hơn 0  \r\n" + Environment.NewLine;
			}
			//validate MaDVT
			var checkMaDVT = await _context.DonViTinh.Where(x => x.MaDvt == MaDVT).FirstOrDefaultAsync();

			if (checkMaDVT == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Đơn VỊ Tính không tồn tại \r\n" + Environment.NewLine;
			}
			//validate LoaiHangHoa
			var checkMaLoaiHangHoa = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == MaLoaiHangHoa).FirstOrDefaultAsync();

			if (checkMaLoaiHangHoa == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Hàng Hóa không tồn tại \r\n" + Environment.NewLine;
			}

			var checkMaPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == MaPTVC).FirstOrDefaultAsync();

			if (checkMaPTVC == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phương Thức Vận Chuyển không tồn tại \r\n" + Environment.NewLine;
			}

			if (NgayHetHieuLuc != null)
			{
				if (NgayHetHieuLuc.Value.Date <= DateTime.Now.Date)
				{
					ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Ngày hết hiệu lực không được nhỏ hơn hoặc bằng ngày hiện tại \r\n" + Environment.NewLine;
				}
			}
			return ErrorValidate;
		}

		public async Task<BoolActionResult> CreatePriceByExcel(IFormFile formFile, CancellationToken cancellationToken)
		{
			try
			{
				int ErrorRow = 0;
				if (formFile == null || formFile.Length <= 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
				}

				if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
				{
					return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
				}

				var list = new List<CreatePriceListRequest>();

				using var stream = new MemoryStream();
				await formFile.CopyToAsync(stream, cancellationToken);
				using var wbook = new XLWorkbook(stream);
				var ws1 = wbook.Worksheet(1);
				var rowWs1 = ws1.RowsUsed().Count();

				if (rowWs1 == 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
				}

				if (
					ws1.Cell(1, 1).Value.ToString().Trim() != "MaKH" ||
					ws1.Cell(1, 2).Value.ToString().Trim() != "Account" ||
					ws1.Cell(1, 3).Value.ToString().Trim() != "MaHopDong" ||
					ws1.Cell(1, 4).Value.ToString().Trim() != "DiemDau" ||
					ws1.Cell(1, 5).Value.ToString().Trim() != "DiemCuoi" ||
					ws1.Cell(1, 6).Value.ToString().Trim() != "DiemLayTraRong" ||
					ws1.Cell(1, 7).Value.ToString().Trim() != "DonGiaVnd" ||
					ws1.Cell(1, 8).Value.ToString().Trim() != "LoaiTienTe" ||
					ws1.Cell(1, 9).Value.ToString().Trim() != "MaPtvc" ||
					ws1.Cell(1, 10).Value.ToString().Trim() != "MaLoaiPhuongTien" ||
					ws1.Cell(1, 11).Value.ToString().Trim() != "MaLoaiHangHoa")
				{
					return new BoolActionResult { isSuccess = false, Message = "File excel không đúng định dạng chuẩn" };
				}

				for (int row = 3; row <= rowWs1; row++)
				{
					ErrorRow = row;

					string MaKH = ws1.Cell(row, 1).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 1).Value.ToString().Trim().ToUpper();
					string AccountId = ws1.Cell(row, 2).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 2).Value.ToString().Trim().ToUpper();
					string ContractId = ws1.Cell(row, 3).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 3).Value.ToString().Trim().ToUpper();
					string FirstPlace = ws1.Cell(row, 4).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 4).Value.ToString().Trim();
					string SecondPlace = ws1.Cell(row, 5).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 5).Value.ToString().Trim();
					string getEmptyPlace = ws1.Cell(row, 6).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 6).Value.ToString();
					string Price = ws1.Cell(row, 7).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 7).Value.ToString().Trim();
					string priceType = ws1.Cell(row, 8).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 8).Value.ToString().Trim().ToUpper();
					string Ptvc = ws1.Cell(row, 9).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 9).Value.ToString().Trim().ToUpper();
					string VehicleType = ws1.Cell(row, 10).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 10).Value.ToString().Trim().ToUpper();
					string GoodsType = ws1.Cell(row, 11).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 11).Value.ToString().Trim();

					if (MaKH == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ": Không được để trống Mã Khách Hàng" };
					}
					if (ContractId == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Mã Hợp Đồng" };
					}
					if (FirstPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Điểm Đóng Hàng" };
					}
					else
					{
						var checkIsNum = int.TryParse(FirstPlace, out _);

						if (checkIsNum == false)
						{
							return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Sai mã địa điểm" };
						}
					}
					if (SecondPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Điểm Trả Hàng" };
					}
					else
					{
						var checkIsNum = int.TryParse(SecondPlace, out _);

						if (checkIsNum == false)
						{
							return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Sai mã địa điểm" };
						}
					}

					if (getEmptyPlace != null)
					{
						var checkIsNum = int.TryParse(getEmptyPlace, out _);

						if (checkIsNum == false)
						{
							return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Sai mã địa điểm" };
						}
					}

					if (Price == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Đơn Giá VND" };
					}
					else
					{
						var checkIsNum = decimal.TryParse(Price, out _);

						if (checkIsNum == false)
						{
							return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ": sai Đơn Giá VND" };
						}
					}

					if (priceType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Loại Tiền Tệ" };
					}

					//if (Dvt == null)
					//{
					//    return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Đơn Vị Tính" };
					//}
					if (Ptvc == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Phương Thức Vận Chuyển" };
					}
					if (VehicleType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Loại Phương Tiện" };
					}
					if (GoodsType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Loại Hàng Hóa" };
					}

					list.Add(new CreatePriceListRequest()
					{
						DiemLayTraRong = getEmptyPlace == null ? null : int.Parse(getEmptyPlace),
						AccountId = AccountId,
						DiemDau = int.Parse(FirstPlace),
						DiemCuoi = int.Parse(SecondPlace),
						MaHopDong = ContractId,
						MaKH = MaKH,
						MaPtvc = Ptvc,
						MaLoaiPhuongTien = VehicleType,
						DonGia = decimal.Parse(Price),
						LoaiTienTe = priceType,
						MaDvt = "CHUYEN",
						MaLoaiHangHoa = GoodsType,
						MaLoaiDoiTac = "",
						NgayHetHieuLuc = null,
					});
				}

				var addPriceTable = await CreatePriceTable(list);

				if (addPriceTable.isSuccess == true)
				{
					return new BoolActionResult { isSuccess = true, Message = "Tạo bảng giá thành công!" };
				}
				else
				{
					return addPriceTable;
				}
			}
			catch (Exception ex)
			{
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		public async Task<double> GetPriceTradeNow(string priceCode)
		{
			if (priceCode == "VND")
			{
				return 1;
			}

			var getPriceTrade = _context.ExchangeRate.Where(x => x.CurrencyCode == priceCode && x.PriceTransfer != null).OrderByDescending(x => x.CreatedTime).FirstOrDefault();
			return getPriceTrade.PriceSell.Value;
		}

		public async Task<List<GetPriceListRequest>> GetListPriceTableExportExcel(string cusType)
		{
			var getList = from bg in _context.BangGia
						  join hd in _context.HopDongVaPhuLuc
						  on bg.MaHopDong equals hd.MaHopDong
						  join kh in _context.KhachHang
						  on hd.MaKh equals kh.MaKh
						  where (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
						  && bg.NgayApDung.Date <= DateTime.Now.Date
						  && bg.TrangThai == 4
						  && bg.MaLoaiDoiTac == cusType
						  orderby kh.TenKh
						  select new { bg, hd, kh };

			var data = await getList.OrderBy(x => x.kh.TenKh).OrderBy(x => x.bg.MaAccount).Select(x => new GetPriceListRequest()
			{
				AccountName = x.bg.MaAccount == null ? x.kh.TenKh : _context.AccountOfCustomer.Where(y => y.MaAccount == x.bg.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
				DiemLayTraRong = x.bg.DiemLayTraRong == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemLayTraRong).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
				DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.bg.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
				TenKH = x.kh.TenKh,
				MaHopDong = x.bg.MaHopDong,
				NgayApDung = x.bg.NgayApDung,
				DonGia = x.bg.DonGia,
				LoaiTienTe = x.bg.LoaiTienTe,
				MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
				MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
				MaDVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
				MaPTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
				SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
				MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
			}).ToListAsync();

			return data;
		}
	}
}