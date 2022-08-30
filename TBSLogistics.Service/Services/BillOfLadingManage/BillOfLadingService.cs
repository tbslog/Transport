using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public class BillOfLadingService : IBillOfLading
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public BillOfLadingService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }


        public async Task<BoolActionResult> CreateBillOfLading(CreateBillOfLadingRequest request)
        {
            try
            {
                var checkExists = await _context.VanDons.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn này đã tồn tại" };
                }

                await _context.VanDons.AddAsync(new VanDon()
                {
                    MaVanDon = request.MaVanDon,
                    NgayNhapHang = request.NgayNhapHang,
                    MaKh = request.MaKh,
                    MaSoXe = request.MaSoXe,
                    MaTaiXe = request.MaTaiXe,
                    MaRomooc = request.MaRomooc,
                    MaPtvc = request.MaPtvc,
                    Booking = request.Booking,
                    ClpNo = request.ClpNo,
                    ContNo = request.ContNo,
                    SealHt = request.SealHt,
                    SealHq = request.SealHq,
                    MaLoaiThungHang = request.MaLoaiThungHang,
                    MaDonViVanTai = request.MaDonViVanTai,
                    MaLoaiHangHoa = request.MaLoaiHangHoa,
                    TrongLuong = request.TrongLuong,
                    TheTich = request.TheTich,
                    MaDvt = request.MaDvt,
                    DiemLayRong = request.DiemLayRong,
                    DiemLayHang = request.DiemLayHang,
                    DiemNhapHang = request.DiemNhapHang,
                    DiemGioHang = request.DiemGioHang,
                    DiemTraRong = request.DiemTraRong,
                    ThoiGianHanLech = request.ThoiGianHanLech,
                    ThoiGianCoMat = request.ThoiGianCoMat,
                    ThoiGianCatMang = request.ThoiGianCatMang,
                    ThoiGianTraRong = request.ThoiGianTraRong,
                    HangTau = request.HangTau,
                    Tau = request.Tau,
                    CangChuyenTai = request.CangChuyenTai,
                    CangDich = request.CangDich,
                    TrangThaiDonHang = request.TrangThaiDonHang,
                    NgayTaoDon = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("BillOfLadingManage", "UserId: " + TempData.UserID + "create new BillOfLading with Id: " + request.MaVanDon);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLadingManage", "UserId: " + TempData.UserID + "create new BillOfLading with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditBillOfLading(string billOfLadingId, EditBillOfLadingRequest request)
        {
            try
            {
                var getBillOfLading = await _context.VanDons.Where(x => x.MaVanDon == billOfLadingId).FirstOrDefaultAsync();

                if (getBillOfLading == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không tồn tại" };
                }

                getBillOfLading.NgayNhapHang = request.NgayNhapHang;
                getBillOfLading.MaKh = request.MaKh;
                getBillOfLading.MaSoXe = request.MaSoXe;
                getBillOfLading.MaTaiXe = request.MaTaiXe;
                getBillOfLading.MaRomooc = request.MaRomooc;
                getBillOfLading.MaPtvc = request.MaPtvc;
                getBillOfLading.Booking = request.Booking;
                getBillOfLading.ClpNo = request.ClpNo;
                getBillOfLading.ContNo = request.ContNo;
                getBillOfLading.SealHt = request.SealHt;
                getBillOfLading.MaLoaiThungHang = request.MaLoaiThungHang;
                getBillOfLading.MaDonViVanTai = request.MaDonViVanTai;
                getBillOfLading.MaLoaiHangHoa = request.MaLoaiHangHoa;
                getBillOfLading.TrongLuong = request.TrongLuong;
                getBillOfLading.TheTich = request.TheTich;
                getBillOfLading.MaDvt = request.MaDvt;
                getBillOfLading.DiemLayRong = request.DiemLayRong;
                getBillOfLading.DiemLayHang = request.DiemLayHang;
                getBillOfLading.DiemNhapHang = request.DiemNhapHang;
                getBillOfLading.DiemGioHang = request.DiemGioHang;
                getBillOfLading.DiemTraRong = request.DiemTraRong;
                getBillOfLading.ThoiGianHanLech = request.ThoiGianHanLech;
                getBillOfLading.ThoiGianCoMat = request.ThoiGianCoMat;
                getBillOfLading.ThoiGianCatMang = request.ThoiGianCatMang;
                getBillOfLading.ThoiGianTraRong = request.ThoiGianTraRong;
                getBillOfLading.HangTau = request.HangTau;
                getBillOfLading.Tau = request.Tau;
                getBillOfLading.CangChuyenTai = request.CangChuyenTai;
                getBillOfLading.CangDich = request.CangDich;
                getBillOfLading.TrangThaiDonHang = request.TrangThaiDonHang;
                getBillOfLading.UpdatedTime = DateTime.Now;

                _context.Update(getBillOfLading);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("BillOfLadingManage", "UserId: " + TempData.UserID + "update BillOfLading with Id: " + billOfLadingId);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLadingManage", "UserId: " + TempData.UserID + "update BillOfLading with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<GetBillOfLadingRequest> GetBillOfLadingById(string billOfLadingId)
        {
            var getBillOfLading = await _context.VanDons.Where(x => x.MaVanDon == billOfLadingId).Select(x => new GetBillOfLadingRequest()
            {
                MaVanDon = x.MaVanDon,
                NgayNhapHang = x.NgayNhapHang,
                MaKh = x.MaKh,
                MaSoXe = x.MaSoXe,
                MaTaiXe = x.MaTaiXe,
                MaRomooc = x.MaRomooc,
                MaPtvc = x.MaPtvc,
                Booking = x.Booking,
                ClpNo = x.ClpNo,
                ContNo = x.ContNo,
                SealHt = x.SealHt,
                SealHq = x.SealHq,
                MaLoaiThungHang = x.MaLoaiThungHang,
                MaDonViVanTai = x.MaDonViVanTai,
                MaLoaiHangHoa = x.MaLoaiHangHoa,
                TrongLuong = x.TrongLuong,
                TheTich = x.TheTich,
                MaDvt = x.MaDvt,
                DiemLayRong = x.DiemLayRong,
                DiemLayHang = x.DiemLayHang,
                DiemNhapHang = x.DiemNhapHang,
                DiemGioHang = x.DiemGioHang,
                DiemTraRong = x.DiemTraRong,
                ThoiGianHanLech = x.ThoiGianHanLech,
                ThoiGianCoMat = x.ThoiGianCoMat,
                ThoiGianCatMang = x.ThoiGianCatMang,
                ThoiGianTraRong = x.ThoiGianTraRong,
                HangTau = x.HangTau,
                Tau = x.Tau,
                CangChuyenTai = x.CangChuyenTai,
                CangDich = x.CangDich,
                TrangThaiDonHang = x.TrangThaiDonHang,
                NgayTaoDon = x.NgayTaoDon,
                UpdateTime = x.UpdatedTime,
                Createdtime = x.CreatedTime,
            }).FirstOrDefaultAsync();

            return getBillOfLading;
        }
    }
}
