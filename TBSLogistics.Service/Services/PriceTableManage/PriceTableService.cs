using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
                await _context.BangGia.AddRangeAsync(request.Select(x => new BangGia
                {
                    MaHopDong = x.MaHopDong,
                    MaKh = x.MaKH,
                    MaPtvc = x.MaPtvc,
                    MaCungDuong = x.MaCungDuong,
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    GiaVnd = x.GiaVnd,
                    GiaUsd = x.GiaUsd,
                    MaDvt = x.MaDvt,
                    MaLoaiHangHoa = x.MaLoaiHangHoa,
                    NgayApDung = x.NgayApDung,
                    TrangThai = x.TrangThai,
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

            var getData = from bg in _context.BangGia
                          join
                          hd in _context.HopDongVaPhuLuc
                          on bg.MaHopDong equals hd.MaHopDong
                          join
                          kh in _context.KhachHang on bg.MaKh equals kh.MaKh
                          orderby bg.CreatedTime descending
                          select new { bg, hd, kh };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getData = getData.Where(x => x.bg.MaHopDong == filter.Keyword);
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
                TenHopDong = x.hd.TenHienThi,
                MaKh = x.bg.MaHopDong,
                TenKH = x.kh.TenKh,
                MaCungDuong = x.bg.MaCungDuong,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                GiaVnd = x.bg.GiaVnd,
                GiaUsd = x.bg.GiaUsd,
                MaDvt = x.bg.MaDvt,
                MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                MaPtvc = x.bg.MaPtvc,
                NgayApDung = x.bg.NgayApDung,
                TrangThai = x.bg.TrangThai
            }).ToListAsync();

            return new PagedResponseCustom<GetListPiceTableRequest>()
            {
                paginationFilter = validFilter,
                totalCount = totalRecords,
                dataResponse = pagedData
            };
        }

        public async Task<List<GetPriceListRequest>> GetListPriceTableByContractId(string contractId)
        {
            var getList = from bg in _context.BangGia
                          join hd in _context.HopDongVaPhuLuc
                          on bg.MaHopDong equals hd.MaHopDong
                          where bg.MaHopDong == contractId
                          orderby bg.NgayApDung descending
                          select new { bg, hd };

            var list = await getList.Select(x => new GetPriceListRequest()
            {
                MaHopDong = x.bg.MaHopDong,
                MaKh = x.bg.MaKh,
                MaCungDuong = x.bg.MaCungDuong,
                NgayApDung = x.bg.NgayApDung,
                GiaVND = x.bg.GiaVnd,
                GiaUSD = x.bg.GiaUsd,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                MaDVT = x.bg.MaDvt,
                MaPTVC = x.bg.MaPtvc,
                TrangThai = x.bg.TrangThai,
            }).ToListAsync();
            return list;
        }
    }
}