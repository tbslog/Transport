using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.MobileModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.SubFeePriceManage;

namespace TBSLogistics.Service.Services.MobileManager
{
    public class MobileServices : IMobile
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public MobileServices(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _common = common;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<List<GetDataTransportMobile>> GetDataTransportForMobile(string maTaiXe, bool isCompleted)
        {
            try
            {
                var listStatusPending = new List<int> { 21, 30, 31, 20 };


                var dataHandling = from dp in _context.DieuPhoi
                                   join tt in _context.StatusText
                                   on dp.TrangThai equals tt.StatusId
                                   where tt.LangId == tempData.LangID
                                   && dp.MaTaiXe == maTaiXe
                                   select new { dp, tt };
                if (!isCompleted)
                {
                    dataHandling = dataHandling.Where(x => !listStatusPending.Contains(x.dp.TrangThai));
                }
                else
                {
                    dataHandling = dataHandling.Where(x => x.dp.TrangThai == 20);
                }

                var listHandling = await dataHandling.ToListAsync();

                var dataTransport = await _context.VanDon.Where(x => listHandling.Select(y => y.dp.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                var data = dataTransport.Select(x => new GetDataTransportMobile()
                {
                    BookingNo = x.MaVanDonKh,
                    LoaiVanDon = x.LoaiVanDon,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    TongKhoiLuong = x.TongKhoiLuong,
                    TongTheTich = x.TongTheTich,
                    TongSoKien = x.TongSoKien,
                    HangTau = x.HangTau,
                    TenTau = x.Tau,
                    GhiChu = x.GhiChu,
                    MaPTVC = x.MaPtvc,
                    ThoiGianLayRong = x.ThoiGianLayRong,
                    ThoiGianTraRong = x.ThoiGianTraRong,
                    ThoiGianLayHang = x.ThoiGianLayHang,
                    ThoiGianTraHang = x.ThoiGianTraHang,
                    ThoiGianHaCang = x.ThoiGianHaCang,
                    ThoiGianCoMat = x.ThoiGianCoMat,
                    ThoiGianHanLenh = x.ThoiGianHanLenh,
                    getDataHandlingMobiles = listHandling.Where(y => y.dp.MaVanDon == x.MaVanDon).Select(y => new GetDataHandlingMobile()
                    {
                        LoaiPhuongTien = y.dp.MaLoaiPhuongTien,
                        HandlingId = y.dp.Id,
                        MaChuyen = y.dp.MaChuyen,
                        ContNo = y.dp.ContNo,
                        DiemLayRong = y.dp.DiemLayRong == null ? null : _context.DiaDiem.Where(u => u.MaDiaDiem == y.dp.DiemLayRong).Select(u => u.TenDiaDiem).FirstOrDefault(),
                        DiemTraRong = y.dp.DiemTraRong == null ? null : _context.DiaDiem.Where(u => u.MaDiaDiem == y.dp.DiemTraRong).Select(u => u.TenDiaDiem).FirstOrDefault(),
                        KhoiLuong = y.dp.KhoiLuong,
                        TheTich = y.dp.TheTich,
                        SoKien = y.dp.SoKien,
                        TrangThai = y.tt.StatusContent,
                        MaTrangThai = y.dp.TrangThai,
                    }).ToList()
                }).ToList();

                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BoolActionResult> UpdateContNo(string maChuyen, string ContNo)
        {
            try
            {
                var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();

                if (getListHandling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã chuyến không tồn tại" };
                }

                if (!Regex.IsMatch(ContNo.ToUpper(), "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Contno không đúng" };
                }

                getListHandling.ForEach(x =>
                {
                    x.ContNo = ContNo.ToUpper();
                    x.Updater = tempData.UserName;
                    x.UpdatedTime = DateTime.Now;
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật Contno thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật Contno thất bại" };
                }
            }
            catch (Exception ex)
            {

                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }
    }
}