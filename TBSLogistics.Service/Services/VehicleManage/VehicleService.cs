using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.VehicleModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.VehicleManage
{
    public class VehicleService : IVehicle
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public VehicleService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }

        public async Task<BoolActionResult> CreateVehicle(CreateVehicleRequest request)
        {
            try
            {
                var checkExists = await _context.XeVanChuyens.Where(x => x.MaSoXe == request.MaSoXe).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xe này đã tồn tại trong dữ liệu" };
                }

                await _context.XeVanChuyens.AddAsync(new XeVanChuyen()
                {
                    MaSoXe = request.MaSoXe,
                    MaNhaCungCap = request.MaNhaCungCap,
                    MaLoaiPhuongTien = request.MaLoaiPhuongTien,
                    MaTaiXeMacDinh = request.MaTaiXeMacDinh,
                    TrongTaiToiThieu = request.TrongTaiToiThieu,
                    TrongTaiToiDa = request.TrongTaiToiDa,
                    MaGps = request.MaGps,
                    MaGpsmobile = request.MaGpsmobile,
                    LoaiVanHanh = request.LoaiVanHanh,
                    MaTaiSan = request.MaTaiSan,
                    ThoiGianKhauHao = request.ThoiGianKhauHao,
                    NgayHoatDong = request.NgayHoatDong,
                    PhanLoaiXeVanChuyen = request.PhanLoaiXeVanChuyen,
                    TrangThai = request.TrangThai,
                    UpdateTime = DateTime.Now,
                    Createdtime = DateTime.Now,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("VehicleManage", "UserId: " + TempData.UserID + " create new vehicle with id:" + request.MaSoXe);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới xe thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới xe thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("VehicleManage", "UserId: " + TempData.UserID + " create new vehicle with ERROR:" + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditVehicle(string vehicleId, EditVehicleRequest request)
        {
            try
            {
                var getVehicle = await _context.XeVanChuyens.Where(x => x.MaSoXe == vehicleId).FirstOrDefaultAsync();

                if (getVehicle == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xe này không tồn tại trong dữ liệu" };
                }

                getVehicle.MaNhaCungCap = request.MaNhaCungCap;
                getVehicle.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                getVehicle.MaTaiXeMacDinh = request.MaTaiXeMacDinh;
                getVehicle.TrongTaiToiThieu = request.TrongTaiToiThieu;
                getVehicle.TrongTaiToiDa = request.TrongTaiToiDa;
                getVehicle.MaGps = request.MaGps;
                getVehicle.MaGpsmobile = request.MaGpsmobile;
                getVehicle.LoaiVanHanh = request.LoaiVanHanh;
                getVehicle.MaTaiSan = request.MaTaiSan;
                getVehicle.ThoiGianKhauHao = request.ThoiGianKhauHao;
                getVehicle.NgayHoatDong = request.NgayHoatDong;
                getVehicle.PhanLoaiXeVanChuyen = request.PhanLoaiXeVanChuyen;
                getVehicle.TrangThai = request.TrangThai;
                getVehicle.UpdateTime = DateTime.Now;

                _context.Update(getVehicle);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("VehicleManage", "UserId: " + TempData.UserID + " Update vehicle with id:" + vehicleId);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới xe thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới xe thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("VehicleManage", "UserId: " + TempData.UserID + " Update vehicle with ERROR:" + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Tạo mới xe thất bại" };
            }
        }

        public async Task<PagedResponseCustom<ListVehicleRequest>> getListVehicle(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from vehicle in _context.XeVanChuyens
                               orderby vehicle.Createdtime descending
                               select new { vehicle };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.vehicle.MaSoXe.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.vehicle.Createdtime.Date >= filter.fromDate && x.vehicle.Createdtime.Date <= filter.toDate);
                }

                var totalCount = await listData.CountAsync();

                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListVehicleRequest()
                {
                    MaSoXe = x.vehicle.MaSoXe,
                    MaNhaCungCap = x.vehicle.MaNhaCungCap,
                    MaLoaiPhuongTien = x.vehicle.MaLoaiPhuongTien,
                    MaTaiXeMacDinh = x.vehicle.MaTaiXeMacDinh,
                    TrongTaiToiThieu = x.vehicle.TrongTaiToiThieu,
                    TrongTaiToiDa = x.vehicle.TrongTaiToiDa,
                    MaGps = x.vehicle.MaGps,
                    MaGpsmobile = x.vehicle.MaGpsmobile,
                    LoaiVanHanh = x.vehicle.LoaiVanHanh,
                    MaTaiSan = x.vehicle.MaTaiSan,
                    ThoiGianKhauHao = x.vehicle.ThoiGianKhauHao,
                    NgayHoatDong = x.vehicle.NgayHoatDong,
                    PhanLoaiXeVanChuyen = x.vehicle.PhanLoaiXeVanChuyen,
                    TrangThai = x.vehicle.TrangThai,
                    UpdateTime = x.vehicle.UpdateTime,
                    Createdtime = x.vehicle.Createdtime,
                }).ToListAsync();

                return new PagedResponseCustom<ListVehicleRequest>()
                {
                    dataResponse = pagedData,
                    totalCount = totalCount,
                    paginationFilter = validFilter
                };
            }
            catch (Exception ex)
            {
                return new PagedResponseCustom<ListVehicleRequest>();
            }
        }

        public async Task<GetVehicleRequest> GetVehicleById(string vehicleId)
        {
            var vehicle = await _context.XeVanChuyens.Where(x => x.MaSoXe == vehicleId).Select(x => new GetVehicleRequest()
            {
                MaSoXe = x.MaSoXe,
                MaNhaCungCap = x.MaNhaCungCap,
                MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                MaTaiXeMacDinh = x.MaTaiXeMacDinh,
                TrongTaiToiThieu = x.TrongTaiToiThieu,
                TrongTaiToiDa = x.TrongTaiToiDa,
                MaGps = x.MaGps,
                MaGpsmobile = x.MaGpsmobile,
                LoaiVanHanh = x.LoaiVanHanh,
                MaTaiSan = x.MaTaiSan,
                ThoiGianKhauHao = x.ThoiGianKhauHao,
                NgayHoatDong = x.NgayHoatDong,
                PhanLoaiXeVanChuyen = x.PhanLoaiXeVanChuyen,
                TrangThai = x.TrangThai,
                UpdateTime = x.UpdateTime,
                Createdtime = x.Createdtime,
            }).FirstOrDefaultAsync();

            return vehicle;
        }
    }
}
