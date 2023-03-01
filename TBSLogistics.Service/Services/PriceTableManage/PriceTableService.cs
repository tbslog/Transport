using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.PricelistManage;
using TBSLogistics.Service.Services.Common;
using System.Xml;

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
                    }

                    if (item.MaPtvc == "FTL" || item.MaPtvc == "LTL")
                    {
                        if (item.MaLoaiPhuongTien.Contains("CONT"))
                        {
                            ErrorValidate += "Dòng " + row + ": Mã phương tiện không đúng với phương thức vận chuyển";
                        }
                    }
                }

                int rows = 0;
                foreach (var item in request)
                {
                    rows++;

                    var checkDuplicate = request.Where(x =>
                    x.MaKhuVuc == item.MaKhuVuc
                    && x.MaHopDong == item.MaHopDong
                    && x.MaKH == item.MaKH
                    && x.MaPtvc == item.MaPtvc
                    && x.MaCungDuong == item.MaCungDuong
                    && x.MaLoaiPhuongTien == item.MaLoaiPhuongTien
                    && x.MaDvt == item.MaDvt
                    && x.MaLoaiHangHoa == item.MaLoaiHangHoa
                    && x.MaLoaiDoiTac == item.MaLoaiDoiTac).ToList();

                    if (checkDuplicate.Count > 1)
                    {
                        ErrorValidate += "Dòng " + rows + ": Bị trùng lặp, vui lòng kiểm tra lại! ";
                    }
                }


                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                var checkContract = await _context.HopDongVaPhuLuc.Where(x => request.Select(y => y.MaHopDong).Contains(x.MaHopDong)).ToListAsync();
                var checkExistsContract = request.Where(x => !checkContract.Any(y => y.MaHopDong == x.MaHopDong)).Select(x => x.MaHopDong);
                if (checkExistsContract.Count() > 0)
                {
                    ErrorValidate += "Mã hợp đồng không tồn tại: " + string.Join(",", checkExistsContract);
                }

                var checkRoad = await _context.CungDuong.Where(x => request.Select(y => y.MaCungDuong).Contains(x.MaCungDuong)).ToListAsync();
                var checkExistsRoad = request.Where(x => !checkRoad.Any(y => y.MaCungDuong == x.MaCungDuong)).Select(x => x.MaCungDuong);
                if (checkExistsRoad.Count() > 0)
                {
                    ErrorValidate += "Mã cung đường không tồn tại: " + string.Join(",", checkExistsRoad);
                }

                var checkPtvc = await _context.PhuongThucVanChuyen.Where(x => request.Select(y => y.MaPtvc).Contains(x.MaPtvc)).ToListAsync();
                var checkExistsPtvc = request.Where(x => !checkPtvc.Any(y => y.MaPtvc == x.MaPtvc)).Select(x => x.MaPtvc);
                if (checkExistsPtvc.Count() > 0)
                {
                    ErrorValidate += "Mã phương thức vận chuyển không tồn tại: " + string.Join(",", checkExistsPtvc);
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => request.Select(y => y.MaLoaiPhuongTien).Contains(x.MaLoaiPhuongTien)).ToListAsync();
                var checkExistsVehicleType = request.Where(x => !checkVehicleType.Any(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien)).Select(x => x.MaLoaiPhuongTien);
                if (checkExistsVehicleType.Count() > 0)
                {
                    ErrorValidate += "Mã phương tiện vận chuyển không tồn tại: " + string.Join(",", checkExistsVehicleType);
                }

                var checkDVT = await _context.DonViTinh.Where(x => request.Select(y => y.MaDvt).Contains(x.MaDvt)).ToListAsync();
                var checkExistsDVT = request.Where(x => !checkDVT.Any(y => y.MaDvt == x.MaDvt)).Select(x => x.MaDvt);
                if (checkExistsDVT.Count() > 0)
                {
                    ErrorValidate += "Mã đơn vị tính không tồn tại: " + string.Join(",", checkExistsDVT);
                }

                var checkGoodsType = await _context.LoaiHangHoa.Where(x => request.Select(y => y.MaLoaiHangHoa).Contains(x.MaLoaiHangHoa)).ToListAsync();
                var checkExistsGoodsType = request.Where(x => !checkGoodsType.Any(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa)).Select(x => x.MaLoaiHangHoa);
                if (checkExistsGoodsType.Count() > 0)
                {
                    ErrorValidate += "Mã loại hàng hóa không tồn tại: " + string.Join(",", checkExistsGoodsType);
                }

                var checkPartner = await _context.LoaiKhachHang.Where(x => request.Select(y => y.MaLoaiDoiTac).Contains(x.MaLoaiKh)).ToListAsync();
                var checkExistsPertner = request.Where(x => !checkPartner.Any(y => y.MaLoaiKh == x.MaLoaiDoiTac)).Select(x => x.MaLoaiDoiTac);
                if (checkExistsPertner.Count() > 0)
                {
                    ErrorValidate += "Mã đối tác không tồn tại: " + string.Join(",", checkExistsPertner);
                }

                await _context.BangGia.AddRangeAsync(request.Select(x => new BangGia
                {
                    MaKhuVuc = x.MaKhuVuc,
                    MaHopDong = x.MaHopDong,
                    MaPtvc = x.MaPtvc,
                    MaCungDuong = x.MaCungDuong,
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    DonGia = x.DonGia,
                    MaDvt = x.MaDvt,
                    MaLoaiHangHoa = x.MaLoaiHangHoa,
                    NgayApDung = _context.HopDongVaPhuLuc.Where(y => y.MaHopDong == x.MaHopDong).Select(x => x.ThoiGianBatDau).FirstOrDefault(),
                    NgayHetHieuLuc = x.NgayHetHieuLuc,
                    MaLoaiDoiTac = x.MaLoaiDoiTac,
                    TrangThai = 3,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                }).ToList());

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
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

        public async Task<PagedResponseCustom<GetListPiceTableRequest>> GetListPriceTable(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getData = from kh in _context.KhachHang
                          join hd in _context.HopDongVaPhuLuc
                          on kh.MaKh equals hd.MaKh
                          join bg in _context.BangGia
                          on hd.MaHopDong equals bg.MaHopDong
                          join cd in _context.CungDuong
                          on bg.MaCungDuong equals cd.MaCungDuong
                          join tt in _context.StatusText
                          on bg.TrangThai equals tt.StatusId
                          where bg.MaHopDong != "SPDV_TBSL" && tt.LangId == tempData.LangID
                          orderby bg.Id descending
                          select new { kh, hd, bg, cd, tt };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getData = getData.Where(x => x.hd.MaHopDong.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                getData = getData.Where(x => x.bg.NgayApDung.Date >= filter.fromDate && x.bg.NgayApDung <= filter.toDate);
            }

            if (!string.IsNullOrEmpty(filter.goodsType))
            {
                getData = getData.Where(x => x.bg.MaLoaiHangHoa == filter.goodsType);
            }

            if (!string.IsNullOrEmpty(filter.vehicleType))
            {
                getData = getData.Where(x => x.bg.MaLoaiPhuongTien == filter.vehicleType);
            }

            var totalRecords = await getData.CountAsync();

            var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetListPiceTableRequest()
            {
                KhuVuc = _context.KhuVuc.Where(y => y.Id == x.bg.MaKhuVuc).Select(y => y.TenKhuVuc).FirstOrDefault(),
                MaHopDong = x.bg.MaHopDong,
                MaCungDuong = x.cd.MaCungDuong,
                SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
                MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                TenHopDong = x.hd.TenHienThi,
                TenKH = x.kh.TenKh,
                DonGia = x.bg.DonGia,
                TenCungDuong = x.cd.TenCungDuong,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(y => y.TenLoaiHangHoa).FirstOrDefault(),
                MaPtvc = x.bg.MaPtvc,
                NgayApDung = x.bg.NgayApDung,
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                TrangThai = x.tt.StatusContent,
            }).ToListAsync();

            return new PagedResponseCustom<GetListPiceTableRequest>()
            {
                paginationFilter = validFilter,
                totalCount = totalRecords,
                dataResponse = pagedData
            };
        }

        public async Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, string onlyContractId, int PageNumber, int PageSize)
        {
            var validFilter = new PaginationFilter(PageNumber, PageSize);

            var getList = from bg in _context.BangGia
                          join hd in _context.HopDongVaPhuLuc
                          on bg.MaHopDong equals hd.MaHopDong
                          join cd in _context.CungDuong
                          on bg.MaCungDuong equals cd.MaCungDuong
                          join kh in _context.KhachHang
                          on hd.MaKh equals kh.MaKh
                          where
                          cd.TrangThai == 1
                          && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                          && bg.NgayApDung <= DateTime.Now.Date
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

            var gr = from t in getList
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac, t.bg.MaKhuVuc }
                         into g
                     select new
                     {
                         g.Key.MaCungDuong,
                         g.Key.MaDvt,
                         g.Key.MaLoaiHangHoa,
                         g.Key.MaLoaiPhuongTien,
                         g.Key.MaPtvc,
                         g.Key.MaLoaiDoiTac,
                         g.Key.MaKhuVuc,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            getList = getList.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

            var totalRecords = await getList.CountAsync();

            var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetPriceListRequest()
            {
                ID = x.bg.Id,
                MaKh = x.hd.MaKh,
                KhuVuc = _context.KhuVuc.Where(y => y.Id == x.bg.MaKhuVuc).Select(y => y.TenKhuVuc).FirstOrDefault(),
                TenKH = x.kh.TenKh,
                MaHopDong = x.bg.MaHopDong,
                MaCungDuong = x.bg.MaCungDuong,
                NgayApDung = x.bg.NgayApDung,
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
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
                                 join cd in _context.CungDuong
                                 on bg.MaCungDuong equals cd.MaCungDuong
                                 where
                                 cd.TrangThai == 1 &&
                                 bg.NgayApDung.Date <= DateTime.Now.Date
                                 && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                 && bg.TrangThai == 4
                                 && hd.MaKh == MaKH
                                 orderby bg.Id descending
                                 select new { bg, hd };

            var gr = from t in listPriceTable
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac, t.bg.MaKhuVuc }
                     into g
                     select new
                     {
                         g.Key.MaCungDuong,
                         g.Key.MaDvt,
                         g.Key.MaLoaiHangHoa,
                         g.Key.MaLoaiPhuongTien,
                         g.Key.MaPtvc,
                         g.Key.MaLoaiDoiTac,
                         g.Key.MaKhuVuc,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            listPriceTable = listPriceTable.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

            return await listPriceTable.Select(x => new GetPriceListRequest()
            {
                ID = x.bg.Id,
                MaKh = x.hd.MaKh,
                KhuVuc = _context.KhuVuc.Where(y => y.Id == x.bg.MaKhuVuc).Select(y => y.TenKhuVuc).FirstOrDefault(),
                MaHopDong = x.bg.MaHopDong,
                MaCungDuong = x.bg.MaCungDuong,
                NgayApDung = x.bg.NgayApDung,
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                DonGia = x.bg.DonGia,
                MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
                MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                MaDVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
                MaPTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
                SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
                MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                TrangThai = x.bg.TrangThai,
            }).ToListAsync();
        }

        public async Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getData = from kh in _context.KhachHang
                          join hd in _context.HopDongVaPhuLuc
                          on kh.MaKh equals hd.MaKh
                          join bg in _context.BangGia
                          on hd.MaHopDong equals bg.MaHopDong
                          join cd in _context.CungDuong
                          on bg.MaCungDuong equals cd.MaCungDuong
                          orderby bg.CreatedTime descending
                          select new { kh, hd, bg, cd };

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
                KhuVuc = _context.KhuVuc.Where(y => y.Id == x.bg.MaKhuVuc).Select(y => y.TenKhuVuc).FirstOrDefault(),
                MaKh = x.kh.MaKh,
                TenKh = x.kh.TenKh,
                DonGia = x.bg.DonGia,
                MaHopDong = x.hd.MaHopDong,
                TenHopDong = x.hd.TenHienThi,
                PTVC = _context.PhuongThucVanChuyen.Where(y => y.MaPtvc == x.bg.MaPtvc).Select(x => x.TenPtvc).FirstOrDefault(),
                MaCungDuong = x.cd.MaCungDuong,
                TenCungDuong = x.cd.TenCungDuong,
                MaLoaiPhuongTien = _context.LoaiPhuongTien.Where(y => y.MaLoaiPhuongTien == x.bg.MaLoaiPhuongTien).Select(x => x.TenLoaiPhuongTien).FirstOrDefault(),
                DVT = _context.DonViTinh.Where(y => y.MaDvt == x.bg.MaDvt).Select(x => x.TenDvt).FirstOrDefault(),
                MaLoaiHangHoa = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.bg.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                MaLoaiDoiTac = _context.LoaiKhachHang.Where(y => y.MaLoaiKh == x.bg.MaLoaiDoiTac).Select(x => x.TenLoaiKh).FirstOrDefault(),
                NgayApDung = x.bg.NgayApDung.ToString("dd-MM-yyyy"),
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc == null ? null : x.bg.NgayHetHieuLuc.Value.ToString("dd-MM-yyyy"),
                ThoiGianTao = x.bg.CreatedTime.ToString("dd-MM-yyyy HH:mm:ss")
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
                    var checkExists = await _context.BangGia.Where(x => x.Id == item.Id && x.TrangThai == 3).FirstOrDefaultAsync();

                    if (checkExists == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Tồn tại bảng giá không có trong hệ thống, vui lòng xem lại" };
                    }

                    if (item.IsAgree == 1)
                    {
                        checkExists.TrangThai = 5;
                        _context.BangGia.Update(checkExists);
                    }

                    if (item.IsAgree == 0)
                    {
                        var checkOldPriceTable = await _context.BangGia.Where(x =>
                      x.MaKhuVuc == checkExists.MaKhuVuc &&
                      x.MaHopDong == checkExists.MaHopDong &&
                      x.MaPtvc == checkExists.MaPtvc &&
                      x.MaCungDuong == checkExists.MaCungDuong &&
                      x.MaLoaiPhuongTien == checkExists.MaLoaiPhuongTien &&
                      x.MaDvt == checkExists.MaDvt &&
                      x.MaLoaiHangHoa == checkExists.MaLoaiHangHoa &&
                      x.MaLoaiDoiTac == checkExists.MaLoaiDoiTac &&
                      x.TrangThai == 4
                      ).ToListAsync();

                        if (checkOldPriceTable != null)
                        {
                            checkOldPriceTable.ForEach(x => { x.TrangThai = 6; x.NgayHetHieuLuc = DateTime.Now; x.Approver = tempData.UserName; });
                        }

                        checkExists.TrangThai = 4;
                        checkExists.Approver = tempData.UserName;
                        _context.BangGia.Update(checkExists);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Approve PriceTable with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Duyệt bảng giá thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Duyệt bảng giá thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " Approve PriceTable with Error: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<GetPriceListRequest> GetPriceTableById(int id)
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

                var reuslt = await data.Select(x => new GetPriceListRequest()
                {
                    ID = x.bg.Id,
                    MaKhuVuc = x.bg.MaKhuVuc,
                    MaHopDong = x.bg.MaHopDong,
                    SoHopDongCha = x.hd.MaHopDongCha,
                    MaKh = x.hd.MaKh,
                    MaCungDuong = x.bg.MaCungDuong,
                    DonGia = x.bg.DonGia,
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

        public async Task<BoolActionResult> UpdatePriceTable(int id, GetPriceListRequest request)
        {
            try
            {
                var findById = await _context.BangGia.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (findById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Bảng giá không tồn tại" };
                }

                var checkValid = await ValidateEdit(request.MaHopDong, request.MaPTVC, request.MaCungDuong, request.MaLoaiPhuongTien, request.DonGia, request.MaDVT, request.MaLoaiHangHoa, request.NgayHetHieuLuc);

                if (checkValid != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = checkValid };
                }

                if (findById.TrangThai != 3)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể chỉnh sửa bảng giá này nữa" };
                }

                var checkPriceTable = await _context.BangGia.Where(x =>
                   x.MaKhuVuc == request.MaKhuVuc &&
                   x.MaHopDong == request.MaHopDong &&
                   x.DonGia == request.DonGia &&
                   x.MaPtvc == request.MaPTVC &&
                   x.MaCungDuong == request.MaCungDuong &&
                   x.MaLoaiPhuongTien == request.MaLoaiPhuongTien &&
                   x.MaDvt == request.MaDVT &&
                   x.MaLoaiHangHoa == request.MaLoaiHangHoa &&
                   x.TrangThai == 4
                   ).FirstOrDefaultAsync();

                if (checkPriceTable != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu muốn cập nhật đã tồn tại và đang được áp dụng" };
                }

                findById.MaKhuVuc = request.MaKhuVuc;
                findById.MaHopDong = request.MaHopDong;
                findById.MaCungDuong = request.MaCungDuong;
                findById.DonGia = request.DonGia;
                findById.MaDvt = request.MaDVT;
                findById.MaPtvc = request.MaPTVC;
                findById.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                findById.MaLoaiHangHoa = request.MaLoaiHangHoa;
                findById.NgayHetHieuLuc = request.NgayHetHieuLuc;
                findById.Updater = tempData.UserName;

                _context.Update(findById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
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

        private async Task<string> ValidateEdit(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, DateTime? NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == MaHopDong).FirstOrDefaultAsync();
            if (checkContract == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã hợp đồng không tồn tại \r\n" + Environment.NewLine;
            }

            var checkMaCungDuong = await _context.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

            if (checkMaCungDuong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Cung đường không tồn tại \r\n" + Environment.NewLine;
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
    }
}