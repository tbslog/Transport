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

        public async Task<BoolActionResult> CreatePriceTable(CreatePriceListRequest request)
        {
            try
            {
                var checkExists = await _context.BangGia.Where(x => x.MaBangGia == request.MaBangGia).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Bảng giá đã tồn tại" };
                }

                await _context.BangGia.AddAsync(new BangGia()
                {
                    MaBangGia = request.MaBangGia,
                    MaHopDong = request.MaHopDong,
                    MaKh = request.MaKH,
                    MaCungDuong = request.MaCungDuong,
                    MaLoaiPhuongTien = request.MaLoaiPhuongTien,
                    GiaVnd = request.GiaVnd,
                    GiaUsd = request.GiaUsd,
                    MaDvt = request.MaDvt,
                    SoLuong = request.SoLuong,
                    MaLoaiHangHoa = request.MaLoaiHangHoa,
                    MaPtvc = request.MaPtvc,
                    NgayApDung = request.NgayApDung,
                    TrangThai = request.TrangThai,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " create new PriceList with Id: " + request.MaBangGia);
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

        public async Task<BoolActionResult> EditPriceTable(string Id, UpdatePriceListRequest request)
        {
            try
            {
                var getPriceList = await _context.BangGia.Where(x => x.MaBangGia == Id).FirstOrDefaultAsync();

                if (getPriceList == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Bảng giá không tồn tại, Vui lòng tạo mới" };
                }

                getPriceList.MaKh = request.MaKh;
                getPriceList.MaCungDuong = request.MaCungDuong;
                getPriceList.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                getPriceList.GiaVnd = request.GiaVnd;
                getPriceList.GiaUsd = request.GiaUsd;
                getPriceList.MaDvt = request.MaDvt;
                getPriceList.SoLuong = request.SoLuong;
                getPriceList.MaLoaiHangHoa = request.MaLoaiHangHoa;
                getPriceList.MaPtvc = request.MaPtvc;
                getPriceList.UpdatedTime = DateTime.Now;

                _context.BangGia.Update(getPriceList);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " Update PriceList with Id: " + Id);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới bảng giá thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới bảng giá thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " Update PriceList with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<PagedResponseCustom<GetListPiceTableRequest>> GetListPriceTable(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getData = from bg in _context.BangGia
                          join kh in _context.KhachHang
                          on bg.MaKh equals kh.MaKh
                          join hd in _context.HopDongVaPhuLuc on bg.MaHopDong equals hd.MaHopDong
                          join cd in _context.CungDuong on bg.MaCungDuong equals cd.MaCungDuong
                          where hd.SoHopDongCha == null
                          orderby bg.UpdatedTime descending
                          select new { bg, kh, hd, cd };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getData = getData.Where(x => x.bg.MaKh.ToLower().Contains(filter.Keyword.ToLower()) ||
                x.bg.MaBangGia.ToLower().Contains(filter.Keyword.ToLower()) ||
                x.kh.TenKh.ToLower().Contains(filter.Keyword.ToLower())
                );
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
                MaBangGia = x.bg.MaBangGia,
                MaHopDong = x.bg.MaHopDong,
                TenHopDong = x.hd.TenHienThi,
                MaKh = x.bg.MaHopDong,
                TenKH = x.kh.TenKh,
                MaCungDuong = x.bg.MaCungDuong,
                TenCungDuong = x.cd.TenCungDuong,
                MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                GiaVnd = x.bg.GiaVnd,
                GiaUsd = x.bg.GiaUsd,
                MaDvt = x.bg.MaDvt,
                SoLuong = x.bg.SoLuong,
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

        public async Task<List<GetPriceListRequest>> GetListPriceTableByCusId(string CustomerId)
        {
            var list = await _context.BangGia.Where(x => x.MaKh == CustomerId).Select(x => new GetPriceListRequest()
            {
                MaBangGia = x.MaBangGia,
                MaKh = x.MaKh,
                MaCungDuong = x.MaCungDuong,
                MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                GiaVnd = x.GiaVnd,
                GiaUsd = x.GiaUsd,
                MaDvt = x.MaDvt,
                SoLuong = x.SoLuong,
                MaLoaiHangHoa = x.MaLoaiHangHoa,
                MaPtvc = x.MaPtvc,
            }).ToListAsync();

            return list;
        }

        public async Task<GetPriceListRequest> GetPriceTableById(string PriceListId)
        {
            var PriceList = await _context.BangGia.Where(x => x.MaBangGia == PriceListId).Select(x => new GetPriceListRequest()
            {
                MaBangGia = x.MaBangGia,
                MaKh = x.MaKh,
                MaCungDuong = x.MaCungDuong,
                MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                GiaVnd = x.GiaVnd,
                GiaUsd = x.GiaUsd,
                MaDvt = x.MaDvt,
                SoLuong = x.SoLuong,
                MaLoaiHangHoa = x.MaLoaiHangHoa,
                MaPtvc = x.MaPtvc,
            }).FirstOrDefaultAsync();

            return PriceList;
        }
    }
}