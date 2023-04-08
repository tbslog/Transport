using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.MobileModel;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.SFeeByTcommandManage;

namespace TBSLogistics.Service.Services.MobileManager
{
    public class MobileServices : IMobile
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly ISFeeByTcommand _SFeeByTcommand;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public MobileServices(ICommon common, TMSContext context, ISFeeByTcommand sFeeByTcommand, IHttpContextAccessor httpContextAccessor)
        {
            _common = common;
            _SFeeByTcommand = sFeeByTcommand;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }


        public async Task<BoolActionResult> ResetStatus(string maChuyen)
        {
            var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();
            var getListTransport = await _context.VanDon.Where(x => getListHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

            getListHandling.ForEach(x =>
            {
                x.TrangThai = 27;
            });

            getListTransport.ForEach(x =>
            {
                x.TrangThai = 9;
            });

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new BoolActionResult { isSuccess = true, Message = "Ok fen" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Lỗi rồi fen" };
            }
        }

        public async Task<List<GetDataTransportMobile>> GetDataTransportForMobile(string maTaiXe, bool isCompleted)
        {
            try
            {
                maTaiXe = tempData.UserName;

                var listStatusPending = new List<int> { 30, 19, 31 };

                var dataHandling = from dp in _context.DieuPhoi
                                   where dp.MaTaiXe == maTaiXe
                                   select new { dp };
                if (!isCompleted)
                {
                    dataHandling = dataHandling.Where(x => !listStatusPending.Contains(x.dp.TrangThai));
                }
                else
                {
                    dataHandling = dataHandling.Where(x => x.dp.TrangThai == 20);
                }

                var listHandling = await dataHandling.ToListAsync();
                var list = listHandling.ToList();
                var strTrasnport = new List<string>();

                foreach (var item in listHandling)
                {
                    strTrasnport.Add(item.dp.MaVanDon);
                    if (list.Where(x => x.dp.MaChuyen == item.dp.MaChuyen).Count() > 1)
                    {
                        list.Remove(item);
                    }
                }

                listHandling = list;

                var listTransport = from dp in _context.DieuPhoi
                                    join vd in _context.VanDon
                                    on dp.MaVanDon equals vd.MaVanDon
                                    where strTrasnport.Contains(dp.MaVanDon)
                                    select new { dp, vd };

                var listStatus = await _context.StatusText.Where(x => x.LangId == tempData.LangID).ToListAsync();

                var data = listHandling.Select(x => new GetDataTransportMobile()
                {
                    MaChuyen = x.dp.MaChuyen,
                    MaPTVC = _context.VanDon.Where(y => y.MaVanDon == x.dp.MaVanDon).Select(y => y.MaPtvc).FirstOrDefault(),
                    LoaiPhuongTien = x.dp.MaLoaiPhuongTien,
                    ThoiGianLayRong = _context.VanDon.Where(y => y.MaVanDon == x.dp.MaVanDon).Select(y => y.ThoiGianLayRong).FirstOrDefault(),
                    ThoiGianTraRong = _context.VanDon.Where(y => y.MaVanDon == x.dp.MaVanDon).Select(y => y.ThoiGianTraRong).FirstOrDefault(),
                    ThoiGianHaCang = _context.VanDon.Where(y => y.MaVanDon == x.dp.MaVanDon).Select(y => y.ThoiGianHaCang).FirstOrDefault(),
                    ThoiGianHanLenh = _context.VanDon.Where(y => y.MaVanDon == x.dp.MaVanDon).Select(y => y.ThoiGianHanLenh).FirstOrDefault(),
                    getDataHandlingMobiles = listTransport.Where(o => o.dp.MaChuyen == x.dp.MaChuyen).Select(c => new GetDataHandlingMobile()
                    {
                        BookingNo = c.vd.MaVanDonKh,
                        MaVanDon = c.vd.MaVanDon,
                        HandlingId = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "LTL") ? _context.DieuPhoi.Where(o => o.MaVanDon == c.vd.MaVanDon).Select(o => o.Id).FirstOrDefault() : _context.DieuPhoi.Where(o => o.MaChuyen == x.dp.MaChuyen).Select(o => o.Id).FirstOrDefault(),
                        LoaiVanDon = c.vd.LoaiVanDon,
                        DiemLayHang = _context.DiaDiem.Where(o => o.MaDiaDiem == c.vd.DiemDau).Select(o => o.TenDiaDiem).FirstOrDefault(),
                        DiemTraHang = _context.DiaDiem.Where(o => o.MaDiaDiem == c.vd.DiemCuoi).Select(o => o.TenDiaDiem).FirstOrDefault(),
                        MaDiemLayHang = c.vd.DiemDau,
                        MaDiemTraHang = c.vd.DiemCuoi,
                        MaDiemLayRong = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "FCL") ? _context.DieuPhoi.Where(o => o.MaChuyen == x.dp.MaChuyen).Select(o => o.DiemLayRong).FirstOrDefault() : null,
                        MaDiemTraRong = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "FCL") ? _context.DieuPhoi.Where(o => o.MaChuyen == x.dp.MaChuyen).Select(o => o.DiemTraRong).FirstOrDefault() : null,
                        HangTau = c.vd.HangTau,
                        GhiChu = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "LTL") ? _context.DieuPhoi.Where(o => o.MaVanDon == c.vd.MaVanDon).Select(o => o.GhiChu).FirstOrDefault() : _context.DieuPhoi.Where(o => o.MaChuyen == x.dp.MaChuyen).Select(o => o.GhiChu).FirstOrDefault(),
                        ContNo = x.dp.ContNo,
                        DiemTraRong = x.dp.DiemTraRong == null ? "" : _context.DiaDiem.Where(o => o.MaDiaDiem == x.dp.DiemTraRong).Select(o => o.TenDiaDiem).FirstOrDefault(),
                        DiemLayRong = x.dp.DiemLayRong == null ? "" : _context.DiaDiem.Where(o => o.MaDiaDiem == x.dp.DiemLayRong).Select(o => o.TenDiaDiem).FirstOrDefault(),
                        KhoiLuong = x.dp.KhoiLuong,
                        TheTich = x.dp.TheTich,
                        SoKien = x.dp.SoKien,
                        ThuTuGiaoHang = x.dp.ThuTuGiaoHang,
                        TrangThai = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "LTL") ? _context.StatusText.Where(z => z.LangId == tempData.LangID && z.FunctionId == "Handling" && z.StatusId == (_context.DieuPhoi.Where(z => z.MaVanDon == c.vd.MaVanDon).Select(z => z.TrangThai).FirstOrDefault())).Select(z => z.StatusContent).FirstOrDefault() : _context.StatusText.Where(z => z.LangId == tempData.LangID && z.FunctionId == "Handling" && z.StatusId == (_context.DieuPhoi.Where(z => z.MaChuyen == c.dp.MaChuyen).Select(z => z.TrangThai).FirstOrDefault())).Select(z => z.StatusContent).FirstOrDefault(),
                        MaTrangThai = (c.vd.MaPtvc == "LCL" || c.vd.MaPtvc == "LTL") ? _context.DieuPhoi.Where(o => o.MaVanDon == c.vd.MaVanDon).Select(o => o.TrangThai).FirstOrDefault() : _context.DieuPhoi.Where(o => o.MaChuyen == x.dp.MaChuyen).Select(o => o.TrangThai).FirstOrDefault(),
                        ThoiGianLayHang = c.vd.ThoiGianLayHang,
                        ThoiGianTraHang = c.vd.ThoiGianTraHang,
                        ThoiGianCoMat = c.vd.ThoiGianCoMat,
                    }).ToList()
                }).ToList();
                return data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BoolActionResult> UpdateContNo(string maChuyen, string contNo)
        {
            try
            {
                var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();

                if (getListHandling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã chuyến không tồn tại" };
                }

                if (!Regex.IsMatch(contNo.ToUpper(), "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Contno không đúng" };
                }

                getListHandling.ForEach(x =>
                {
                    x.ContNo = contNo.ToUpper();
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

        public async Task<BoolActionResult> WriteNoteHandling(int handlingId, string note)
        {
            try
            {
                var getHandling = await _context.DieuPhoi.Where(x => x.Id == handlingId).FirstOrDefaultAsync();
                if (getHandling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                getHandling.GhiChu = note;
                getHandling.UpdatedTime = DateTime.Now;

                _context.DieuPhoi.Update(getHandling);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật ghi chú thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật ghi chú không thành công" };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ListSubFeeIncurred>> GetListSubfeeIncurred(string maChuyen)
        {
            var listHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();

            var dataSubFeeIncurred = from dp in _context.DieuPhoi
                                     join sfc in _context.SfeeByTcommand
                                     on dp.Id equals sfc.IdTcommand
                                     join sf in _context.SubFee
                                     on sfc.SfId equals sf.SubFeeId
                                     join status in _context.StatusText
                                     on sfc.ApproveStatus equals status.StatusId
                                     where sfc.ApproveStatus == 14
                                     && status.LangId == tempData.LangID
                                     orderby sfc.CreatedDate descending
                                     select new { dp, sfc, sf, status };

            var listDataSubFeeIncurred = await dataSubFeeIncurred.Where(x => listHandling.Select(y => y.Id).Contains(x.sfc.IdTcommand)).Select(x => new ListSubFeeIncurred()
            {
                Id = x.sfc.Id,
                MaVanDon = x.dp.MaVanDon,
                MaSoXe = x.dp.MaSoXe,
                DiaDiem = x.sfc.PlaceId == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfc.PlaceId).Select(y => y.TenDiaDiem).FirstOrDefault(),
                Type = "Phụ Phí Phát Sinh",
                TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                SubFee = x.sf.SfName,
                Price = x.sfc.Price,
                TrangThai = x.status.StatusContent,
                ApprovedDate = x.sfc.ApprovedDate.Value,
                CreatedDate = x.sfc.CreatedDate,
            }).ToListAsync();

            return listDataSubFeeIncurred;
        }

        public async Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request, string maChuyen = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(maChuyen))
                {
                    var listStatus = new List<int>(new int[] { 21, 31, 38 });

                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen && !listStatus.Contains(x.TrangThai)).ToListAsync();

                    foreach (var item in getListHandling)
                    {
                        foreach (var itemRequest in request)
                        {
                            itemRequest.IdTcommand = item.Id;
                            itemRequest.TransportId = item.MaVanDon;
                            var add = await _SFeeByTcommand.CreateSFeeByTCommand(request);

                            if (!add.isSuccess)
                            {
                                return add;
                            }

                        }
                    }
                    return new BoolActionResult { isSuccess = true, Message = "Thêm phụ phí thành công" };
                }

                var Create = await _SFeeByTcommand.CreateSFeeByTCommand(request);

                return Create;



            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}