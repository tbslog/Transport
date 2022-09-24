using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
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
                foreach (var item in request)
                {
                    if (item.NgayHetHieuLuc.Date <= item.NgayApDung.Date)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "" };
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
                    NgayApDung = x.NgayApDung,
                    NgayHetHieuLuc = x.NgayHetHieuLuc,
                    MaLoaiDoiTac = x.MaLoaiDoiTac,
                    TrangThai = x.TrangThai,
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
                          orderby bg.NgayApDung descending
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
                SoHopDongCha = x.hd.SoHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
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

        public async Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, int PageNumber, int PageSize)
        {
            var validFilter = new PaginationFilter(PageNumber, PageSize);


            var getList = from bg in _context.BangGia
                          join hd in _context.HopDongVaPhuLuc
                          on bg.MaHopDong equals hd.MaHopDong
                          where bg.NgayHetHieuLuc.Date >= DateTime.Now.Date && bg.NgayApDung <= DateTime.Now.Date
                          orderby bg.NgayApDung descending
                          select new { bg, hd };



            var checkContractChild = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == contractId).FirstOrDefaultAsync();

            if (checkContractChild == null)
            {
                return null;
            }

            if (checkContractChild.SoHopDongCha == null)
            {
                var listContract = getList.Where(x => x.hd.MaHopDong == contractId || x.hd.SoHopDongCha == contractId).Select(x => x.hd.MaHopDong);
                getList = getList.Where(x => listContract.Contains(x.bg.MaHopDong));
            }
            else
            {
                var listContract = getList.Where(x => x.hd.MaHopDong == checkContractChild.SoHopDongCha || x.hd.SoHopDongCha == checkContractChild.SoHopDongCha).Select(x => x.hd.MaHopDong);
                getList = getList.Where(x => listContract.Contains(x.bg.MaHopDong));
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
                MaKh = x.hd.MaKh,
                MaHopDong = x.bg.MaHopDong,
                MaCungDuong = x.bg.MaCungDuong,
                NgayApDung = x.bg.NgayApDung,
                NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                DonGia = x.bg.DonGia,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                MaDVT = x.bg.MaDvt,
                MaPTVC = x.bg.MaPtvc,
                SoHopDongCha = x.hd.SoHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
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
    }
}