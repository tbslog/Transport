using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.PricelistManage
{
    public class PriceListService : IPriceList
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;

        public PriceListService(TMSContext context, ICommon common)
        {
            _context = context;
            _common = common;
        }

        public async Task<BoolActionResult> CreatePriceList(CreatePriceListRequest request)
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
                    MaKh = request.MaKH,
                    MaCungDuong = request.MaCungDuong,
                    MaLoaiPhuongTien = request.MaLoaiPhuongTien,
                    GiaVnd = request.GiaVnd,
                    GiaUsd = request.GiaUsd,
                    MaDvt = request.MaDvt,
                    SoLuong = request.SoLuong,
                    MaLoaiHangHoa = request.MaLoaiHangHoa,
                    MaPtvc = request.MaPtvc,
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

        public async Task<BoolActionResult> EditPriceList(string PriceListId, UpdatePriceListRequest request)
        {
            try
            {
                var getPriceList = await _context.BangGia.Where(x => x.MaBangGia == PriceListId).FirstOrDefaultAsync();

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
                    await _common.Log("PriceListManage", "UserId:" + TempData.UserID + " Update PriceList with Id: " + PriceListId);
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

        public async Task<List<GetPriceListRequest>> GetListPriceList()
        {
            var list = await _context.BangGia.Select(x => new GetPriceListRequest()
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

        public async Task<List<GetPriceListRequest>> GetListPriceListByCusId(string CustomerId)
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

        public async Task<GetPriceListRequest> GetPriceListById(string PriceListId)
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