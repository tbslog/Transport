using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.VehicleModel;
using TBSLogistics.Model.TempModel;
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

        public async Task<List<GetVehicleRequest>> GetListVehicle()
        {
            var list = await _context.XeVanChuyens.Select(x => new GetVehicleRequest()
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
            }).ToListAsync();

            return list;
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
