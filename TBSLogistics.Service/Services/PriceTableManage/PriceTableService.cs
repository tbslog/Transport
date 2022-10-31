using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TBSLogisticsDbContext;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.PricelistManage
{
    public class PriceTableService : IPriceTable
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;

        public PriceTableService(TMSContext context, ICommon common)
        {
            _context = context;
            _common = common;
        }

        public async Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
        {
            try
            {
                string ErrorValidate = "";

                foreach (var item in request)
                {
                    if (!string.IsNullOrEmpty(item.NgayHetHieuLuc.ToString()))
                    {
                        if (item.NgayHetHieuLuc.Value.Date <= DateTime.Now.Date)
                        {
                            ErrorValidate += "Ngày hết hiệu lực không được nhỏ hôm nay";
                        }
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
                    ErrorValidate += "Mã hợp đồng không tồn tại: " + String.Join(",", checkExistsContract);
                }

                var checkRoad = await _context.CungDuong.Where(x => request.Select(y => y.MaCungDuong).Contains(x.MaCungDuong)).ToListAsync();
                var checkExistsRoad = request.Where(x => !checkRoad.Any(y => y.MaCungDuong == x.MaCungDuong)).Select(x => x.MaCungDuong);
                if (checkExistsRoad.Count() > 0)
                {
                    ErrorValidate += "Mã cung đường không tồn tại: " + String.Join(",", checkExistsRoad);
                }

                var checkPtvc = await _context.PhuongThucVanChuyen.Where(x => request.Select(y => y.MaPtvc).Contains(x.MaPtvc)).ToListAsync();
                var checkExistsPtvc = request.Where(x => !checkPtvc.Any(y => y.MaPtvc == x.MaPtvc)).Select(x => x.MaPtvc);
                if (checkExistsPtvc.Count() > 0)
                {
                    ErrorValidate += "Mã phương thức vận chuyển không tồn tại: " + String.Join(",", checkExistsPtvc);
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => request.Select(y => y.MaLoaiPhuongTien).Contains(x.MaLoaiPhuongTien)).ToListAsync();
                var checkExistsVehicleType = request.Where(x => !checkVehicleType.Any(y => y.MaLoaiPhuongTien == x.MaLoaiPhuongTien)).Select(x => x.MaLoaiPhuongTien);
                if (checkExistsVehicleType.Count() > 0)
                {
                    ErrorValidate += "Mã phương tiện vận chuyển không tồn tại: " + String.Join(",", checkExistsVehicleType);
                }

                var checkDVT = await _context.DonViTinh.Where(x => request.Select(y => y.MaDvt).Contains(x.MaDvt)).ToListAsync();
                var checkExistsDVT = request.Where(x => !checkDVT.Any(y => y.MaDvt == x.MaDvt)).Select(x => x.MaDvt);
                if (checkExistsDVT.Count() > 0)
                {
                    ErrorValidate += "Mã đơn vị tính không tồn tại: " + String.Join(",", checkExistsDVT);
                }

                var checkGoodsType = await _context.LoaiHangHoa.Where(x => request.Select(y => y.MaLoaiHangHoa).Contains(x.MaLoaiHangHoa)).ToListAsync();
                var checkExistsGoodsType = request.Where(x => !checkGoodsType.Any(y => y.MaLoaiHangHoa == x.MaLoaiHangHoa)).Select(x => x.MaLoaiHangHoa);
                if (checkExistsGoodsType.Count() > 0)
                {
                    ErrorValidate += "Mã loại hàng hóa không tồn tại: " + String.Join(",", checkExistsGoodsType);
                }

                var checkPartner = await _context.LoaiKhachHang.Where(x => request.Select(y => y.MaLoaiDoiTac).Contains(x.MaLoaiKh)).ToListAsync();
                var checkExistsPertner = request.Where(x => !checkPartner.Any(y => y.MaLoaiKh == x.MaLoaiDoiTac)).Select(x => x.MaLoaiDoiTac);
                if (checkExistsPertner.Count() > 0)
                {
                    ErrorValidate += "Mã đối tác không tồn tại: " + String.Join(",", checkExistsPertner);
                }

                var checkStatus = await _context.StatusText.Where(x => request.Select(y => y.TrangThai).Contains(x.StatusId)).ToListAsync();
                var checkExistsStatus = request.Where(x => !checkStatus.Any(y => y.StatusId == x.TrangThai)).Select(x => x.TrangThai);
                if (checkExistsStatus.Count() > 0)
                {
                    ErrorValidate += "Mã trạng thái không tồn tại: " + String.Join(",", checkExistsStatus);
                }


                foreach (var item in request)
                {
                    var checkPriceTable = await _context.BangGia.Where(x =>
                    x.MaHopDong == item.MaHopDong &&
                    x.MaPtvc == item.MaPtvc &&
                    x.MaCungDuong == item.MaCungDuong &&
                    x.MaLoaiPhuongTien == item.MaLoaiPhuongTien &&
                    x.MaDvt == item.MaDvt &&
                    x.MaLoaiHangHoa == item.MaLoaiHangHoa &&
                    x.MaLoaiDoiTac == item.MaLoaiDoiTac
                    ).FirstOrDefaultAsync();

                    if (checkPriceTable != null)
                    {
                        checkPriceTable.TrangThai = 2;
                        _context.BangGia.Update(checkPriceTable);
                    }
                }

                await _context.BangGia.AddRangeAsync(request.Select(x => new BangGia
                {
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
                    UpdatedTime = DateTime.Now
                }).ToList());

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " create new PriceList with contract: " + request.Select(x => x.MaHopDong).FirstOrDefault());
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới bảng giá thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới bảng giá thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " create new PriceList with Error: " + ex.ToString());
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
                          where bg.MaHopDong != "SPDV_TBSL"
                          orderby bg.Id descending
                          select new { kh, hd, bg, cd };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getData = getData.Where(x => x.hd.MaHopDong == filter.Keyword);
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
                MaHopDong = x.bg.MaHopDong,
                SoHopDongCha = x.hd.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
                MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                TenHopDong = x.hd.TenHienThi,
                TenKH = x.kh.TenKh,
                TenCungDuong = x.cd.TenCungDuong,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                MaPtvc = x.bg.MaPtvc,
                NgayApDung = x.bg.NgayApDung,
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                TrangThai = x.bg.TrangThai
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
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac }
                         into g
                     select new
                     {
                         MaCungDuong = g.Key.MaCungDuong,
                         MaDvt = g.Key.MaDvt,
                         MaLoaiHangHoa = g.Key.MaLoaiHangHoa,
                         MaLoaiPhuongTien = g.Key.MaLoaiPhuongTien,
                         MaPtvc = g.Key.MaPtvc,
                         MaLoaiDoiTac = g.Key.MaLoaiDoiTac,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            getList = getList.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

            var totalRecords = await getList.CountAsync();

            var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetPriceListRequest()
            {
                ID = x.bg.Id,
                MaKh = x.hd.MaKh,
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
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac }
                     into g
                     select new
                     {
                         MaCungDuong = g.Key.MaCungDuong,
                         MaDvt = g.Key.MaDvt,
                         MaLoaiHangHoa = g.Key.MaLoaiHangHoa,
                         MaLoaiPhuongTien = g.Key.MaLoaiPhuongTien,
                         MaPtvc = g.Key.MaPtvc,
                         MaLoaiDoiTac = g.Key.MaLoaiDoiTac,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            listPriceTable = listPriceTable.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));

            return await listPriceTable.Select(x => new GetPriceListRequest()
            {
                ID = x.bg.Id,
                MaCungDuong = x.bg.MaCungDuong,
                DonGia = x.bg.DonGia,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                MaDVT = x.bg.MaDvt,
                MaPTVC = x.bg.MaPtvc
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
                      x.MaHopDong == checkExists.MaHopDong &&
                      x.MaPtvc == checkExists.MaPtvc &&
                      x.MaCungDuong == checkExists.MaCungDuong &&
                      x.MaLoaiPhuongTien == checkExists.MaLoaiPhuongTien &&
                      x.MaDvt == checkExists.MaDvt &&
                      x.MaLoaiHangHoa == checkExists.MaLoaiHangHoa &&
                      x.MaLoaiDoiTac == checkExists.MaLoaiDoiTac &&
                      x.TrangThai == 4
                      ).FirstOrDefaultAsync();

                        if (checkOldPriceTable != null)
                        {
                            checkOldPriceTable.TrangThai = 6;
                            checkOldPriceTable.NgayHetHieuLuc = DateTime.Now;
                            _context.BangGia.Update(checkOldPriceTable);
                        }

                        checkExists.TrangThai = 4;
                        _context.BangGia.Update(checkExists);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Duyệt bảng giá thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Duyệt bảng giá thất bại" };
                }
            }
            catch (Exception ex)
            {
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

                findById.MaHopDong = request.MaHopDong;
                findById.MaCungDuong = request.MaCungDuong;
                findById.DonGia = request.DonGia;
                findById.MaDvt = request.MaDVT;
                findById.MaPtvc = request.MaPTVC;
                findById.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                findById.MaLoaiHangHoa = request.MaLoaiHangHoa;
                findById.NgayHetHieuLuc = request.NgayHetHieuLuc;

                _context.Update(findById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật bảng giá thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật bảng giá thất bại" };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<string> ValidateEdit(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, DateTime? NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == MaHopDong).FirstOrDefaultAsync();
            if (checkContract == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã hợp đồng không tồn tại \r\n" + System.Environment.NewLine;
            }

            var checkMaCungDuong = await _context.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

            if (checkMaCungDuong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Cung đường không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaLoaiPhuongTien = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

            if (checkMaLoaiPhuongTien == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện không tồn tại \r\n" + System.Environment.NewLine;
            }

            if (DonGia < 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đơn giá không được để trong và phải lớn hơn 0  \r\n" + System.Environment.NewLine;
            }
            //validate MaDVT
            var checkMaDVT = await _context.DonViTinh.Where(x => x.MaDvt == MaDVT).FirstOrDefaultAsync();

            if (checkMaDVT == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Đơn VỊ Tính không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate LoaiHangHoa
            var checkMaLoaiHangHoa = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == MaLoaiHangHoa).FirstOrDefaultAsync();

            if (checkMaLoaiHangHoa == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Hàng Hóa không tồn tại \r\n" + System.Environment.NewLine;
            }

            var checkMaPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == MaPTVC).FirstOrDefaultAsync();

            if (checkMaPTVC == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phương Thức Vận Chuyển không tồn tại \r\n" + System.Environment.NewLine;
            }

            if (NgayHetHieuLuc != null)
            {
                if (NgayHetHieuLuc.Value.Date <= DateTime.Now.Date)
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Ngày hết hiệu lực không được nhỏ hơn hoặc bằng ngày hiện tại \r\n" + System.Environment.NewLine;
                }
            }
            return ErrorValidate;
        }
    }
}