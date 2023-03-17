using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.SubFeePriceManage;

namespace TBSLogistics.Service.Services.BillOfLadingManage
{
    public class BillOfLadingService : IBillOfLading
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly ISubFeePrice _subFeePrice;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public BillOfLadingService(ICommon common, TMSContext context, ISubFeePrice subFeePrice, IHttpContextAccessor httpContextAccessor)
        {
            _common = common;
            _subFeePrice = subFeePrice;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<string> LayTrongTaiXe(string vehicleType, string DonVi, double giaTri)
        {
            var getData = await _context.TrongTaiXe.Where(x => x.MaLoaiPhuongTien == vehicleType && x.DonViTrongTai == DonVi).FirstOrDefaultAsync();

            if (getData != null)
            {
                if (giaTri > getData.TrongTaiToiDa)
                {
                    return getData.DonViTrongTai + " xe đang bị vượt quá trọng tải " + (giaTri - getData.TrongTaiToiDa) + " Tối Đa:" + getData.TrongTaiToiDa;
                }
            }

            return null;
        }

        public async Task<LoadDataHandling> LoadDataHandling()
        {
            var listRomooc = from rm in _context.Romooc join lrm in _context.LoaiRomooc on rm.MaLoaiRomooc equals lrm.MaLoaiRomooc select new { rm, lrm };

            var result = new LoadDataHandling()
            {
                ListNhaPhanPhoi = await _context.KhachHang.Where(x => x.MaLoaiKh == "NCC").GroupBy(x => new { x.MaKh, x.TenKh }).Select(x => new NhaPhanPhoiSelect()
                {
                    MaNPP = x.Key.MaKh,
                    TenNPP = x.Key.TenKh
                }).ToListAsync(),
                ListKhachHang = await _context.KhachHang.Where(x => x.MaLoaiKh == "KH").GroupBy(x => new { x.MaKh, x.TenKh }).Select(x => new KhachHangSelect()
                {
                    MaKH = x.Key.MaKh,
                    TenKH = x.Key.TenKh
                }).ToListAsync(),
                ListTaiXe = await _context.TaiXe.Select(x => new DriverTransport()
                {
                    MaTaiXe = x.MaTaiXe,
                    TenTaiXe = x.HoVaTen,
                }).ToListAsync(),
                ListRomooc = await listRomooc.Select(x => new RomoocTransport()
                {
                    MaRomooc = x.rm.MaRomooc,
                    TenLoaiRomooc = x.lrm.TenLoaiRomooc
                }).ToListAsync()
            };
            return result;
        }

        #region Create Handling
        //public async Task<BoolActionResult> CreateHandling(CreateHandling request)
        //{
        //    try
        //    {
        //        var transport = await _context.VanDon.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();
        //        if (transport == null)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
        //        }

        //        var checkRoad = await _context.CungDuong.Where(x => x.MaCungDuong == request.MaCungDuong).FirstOrDefaultAsync();
        //        if (checkRoad == null)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Cung đường không tồn tại" };
        //        }

        //        string ErrorValidate = "";

        //        if (!string.IsNullOrEmpty(ErrorValidate))
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
        //        }

        //        using var transaction = _context.Database.BeginTransaction();

        //        transport.TrangThai = 9;
        //        _context.VanDon.Update(transport);

        //        string MaChuyen = transport.MaPtvc.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");

        //        if (transport.MaPtvc == "LCL" || transport.MaPtvc == "LTL")
        //        {
        //            var getHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transport.MaVanDon).FirstOrDefaultAsync();

        //            if (getHandling != null)
        //            {
        //                MaChuyen = getHandling.MaChuyen;
        //            }
        //        }

        //        foreach (var item in request.DieuPhoi)
        //        {
        //            var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();

        //            if (checkVehicleType == null)
        //            {
        //                return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
        //            }

        //            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
        //            if (checkGoodsType == null)
        //            {
        //                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
        //            }
        //            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
        //            if (checkDVT == null)
        //            {
        //                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
        //            }
        //            var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == item.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
        //            if (checkPTVC == null)
        //            {
        //                return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
        //            }

        //            if (item.PTVanChuyen.Contains("CONT"))
        //            {
        //                var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemLayTraRong.Value).FirstOrDefaultAsync();
        //                if (checkPlaceGetEmpty == null)
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
        //                }
        //            }

        //            var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
        //            if (checkSupplier == null)
        //            {
        //                return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
        //            }

        //            var checkPriceTable = from bg in _context.BangGia
        //                                  join hd in _context.HopDongVaPhuLuc
        //                                  on bg.MaHopDong equals hd.MaHopDong
        //                                  where (hd.MaKh == item.MaKH || hd.MaKh == item.DonViVanTai)
        //                                  && bg.MaCungDuong == request.MaCungDuong
        //                                  && bg.TrangThai == 4
        //                                  && bg.NgayApDung.Date <= DateTime.Now.Date
        //                                  && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
        //                                  && bg.MaDvt == item.DonViTinh
        //                                  && bg.MaLoaiHangHoa == item.LoaiHangHoa
        //                                  && bg.MaLoaiPhuongTien == item.PTVanChuyen
        //                                  && bg.MaPtvc == item.MaPTVC
        //                                  select bg;

        //            if (checkPriceTable.Count() == 2)
        //            {
        //                var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
        //                if (priceTableSupplier == null)
        //                {
        //                    return new BoolActionResult
        //                    {
        //                        isSuccess = false,
        //                        Message = "Đơn vị vận tải: "
        //                        + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
        //                    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
        //                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
        //                    ", Loại Hàng Hóa:" + checkGoodsType +
        //                    ", Đơn Vị Tính: " + checkDVT +
        //                    ", Phương thức vận chuyển: " + checkPTVC
        //                    };
        //                }

        //                var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
        //                if (priceTableCustomer == null)
        //                {
        //                    return new BoolActionResult
        //                    {
        //                        isSuccess = false,
        //                        Message = "Khách Hàng: "
        //                       + await _context.KhachHang.Where(x => x.MaKh == item.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
        //                    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
        //                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
        //                    ", Loại Hàng Hóa:" + checkGoodsType +
        //                    ", Đơn Vị Tính: " + checkDVT +
        //                    ", Phương thức vận chuyển: " + checkPTVC
        //                    };
        //                }

        //                var itemHandling = new DieuPhoi();
        //                itemHandling.MaChuyen = MaChuyen;
        //                itemHandling.MaVanDon = transport.MaVanDon;
        //                itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
        //                itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
        //                itemHandling.MaDvt = item.DonViTinh;
        //                itemHandling.DonViVanTai = item.DonViVanTai;
        //                itemHandling.BangGiaKh = priceTableCustomer.Id;
        //                itemHandling.BangGiaNcc = priceTableSupplier.Id;
        //                itemHandling.DonGiaKh = priceTableCustomer.DonGia;
        //                itemHandling.DonGiaNcc = priceTableSupplier.DonGia;
        //                itemHandling.KhoiLuong = item.KhoiLuong;
        //                itemHandling.TheTich = item.TheTich;
        //                itemHandling.SoKien = item.SoKien;
        //                itemHandling.DiemLayTraRong = item.DiemLayTraRong;
        //                itemHandling.TrangThai = 19;
        //                itemHandling.CreatedTime = DateTime.Now;
        //                itemHandling.Creator = tempData.UserName;
        //                await _context.DieuPhoi.AddAsync(itemHandling);
        //            }
        //            else
        //            {
        //                var priceTableSupplier = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "NCC").FirstOrDefaultAsync();
        //                if (priceTableSupplier == null)
        //                {
        //                    return new BoolActionResult
        //                    {
        //                        isSuccess = false,
        //                        Message = "Đơn vị vận tải: "
        //                        + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
        //                    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
        //                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
        //                    ", Loại Hàng Hóa:" + checkGoodsType +
        //                    ", Đơn Vị Tính: " + checkDVT +
        //                    ", Phương thức vận chuyển: " + checkPTVC
        //                    };
        //                }

        //                var priceTableCustomer = await checkPriceTable.Where(x => x.MaLoaiDoiTac == "KH").FirstOrDefaultAsync();
        //                if (priceTableCustomer == null)
        //                {
        //                    return new BoolActionResult
        //                    {
        //                        isSuccess = false,
        //                        Message = "Khách Hàng: "
        //                       + await _context.KhachHang.Where(x => x.MaKh == item.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
        //                    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
        //                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
        //                    ", Loại Hàng Hóa:" + checkGoodsType +
        //                    ", Đơn Vị Tính: " + checkDVT +
        //                    ", Phương thức vận chuyển: " + checkPTVC
        //                    };
        //                }

        //                return new BoolActionResult { isSuccess = false, Message = "Không có bảng giá khách hàng lẫn nhà cung cấp" };
        //            }
        //        }

        //        var result = await _context.SaveChangesAsync();

        //        if (result > 0)
        //        {
        //            transaction.Commit();
        //            await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new BillOfLading with Data: " + JsonSerializer.Serialize(request));
        //            return new BoolActionResult { isSuccess = true, Message = "Điều Phố Vận Đơn thành công" };
        //        }
        //        else
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Điều Phố Vận Đơn thất bại" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new BillOfLading with ERRORS: " + ex.ToString());
        //        return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
        //    }
        //}
        #endregion

        public async Task<BoolActionResult> CreateTransportByExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            int ErrorRow = 0;
            string ErrorValidate = "";

            List<string> listBooking = new List<string>();

            try
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
                }

                if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
                }

                return null;
                //var list = new List<CreateTransport>();

                //using (var stream = new MemoryStream())
                //{
                //    await formFile.CopyToAsync(stream, cancellationToken);

                //    using (var package = new ExcelPackage(stream))
                //    {
                //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                //        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //        var rowCount = worksheet.Dimension.Rows;

                //        if (rowCount == 0)
                //        {
                //            return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
                //        }

                //        if (worksheet.Cells[1, 1].Value.ToString().Trim() != "LoaiVanDon" ||
                //            worksheet.Cells[1, 2].Value.ToString().Trim() != "MaPTVC" ||
                //            worksheet.Cells[1, 3].Value.ToString().Trim() != "MaKH" ||
                //            worksheet.Cells[1, 4].Value.ToString().Trim() != "MaVanDonKH" ||
                //            worksheet.Cells[1, 5].Value.ToString().Trim() != "HangTau" ||
                //            worksheet.Cells[1, 6].Value.ToString().Trim() != "Tau" ||
                //            worksheet.Cells[1, 7].Value.ToString().Trim() != "DiemDongHang" ||
                //            worksheet.Cells[1, 8].Value.ToString().Trim() != "DiemHaHang" ||
                //            worksheet.Cells[1, 9].Value.ToString().Trim() != "TongKhoiLuong" ||
                //            worksheet.Cells[1, 10].Value.ToString().Trim() != "TongTheTich" ||
                //            worksheet.Cells[1, 11].Value.ToString().Trim() != "TongSoKien" ||
                //            worksheet.Cells[1, 12].Value.ToString().Trim() != "MaLoaiHangHoa" ||
                //            worksheet.Cells[1, 13].Value.ToString().Trim() != "MaLoaiPhuongTien" ||
                //            worksheet.Cells[1, 14].Value.ToString().Trim() != "DiemLayTraRong" ||
                //            worksheet.Cells[1, 15].Value.ToString().Trim() != "KhoiLuong" ||
                //            worksheet.Cells[1, 16].Value.ToString().Trim() != "TheTich" ||
                //            worksheet.Cells[1, 17].Value.ToString().Trim() != "SoKien" ||
                //            worksheet.Cells[1, 18].Value.ToString().Trim() != "ThoiGianLayTraRong" ||
                //            worksheet.Cells[1, 19].Value.ToString().Trim() != "ThoiGianHanLenh/ThoiGianHaCang" ||
                //            worksheet.Cells[1, 20].Value.ToString().Trim() != "ThoiGianLayHang" ||
                //            worksheet.Cells[1, 21].Value.ToString().Trim() != "ThoiGianTraHang" ||
                //            worksheet.Cells[1, 22].Value.ToString().Trim() != "GhiChu" ||
                //            worksheet.Cells[1, 23].Value.ToString().Trim() != "DonViVanTai" ||
                //            worksheet.Cells[1, 24].Value.ToString().Trim() != "MaSoXe" ||
                //            worksheet.Cells[1, 25].Value.ToString().Trim() != "MaTaiXe" ||
                //            worksheet.Cells[1, 26].Value.ToString().Trim() != "Cont_No" ||
                //            worksheet.Cells[1, 27].Value.ToString().Trim() != "SEAL_NP" ||
                //            worksheet.Cells[1, 28].Value.ToString().Trim() != "SEAL_HQ" ||
                //            worksheet.Cells[1, 29].Value.ToString().Trim() != "GhiChuDP"
                //            )
                //        {
                //            return new BoolActionResult { isSuccess = false, Message = "File excel không đúng định dạng chuẩn" };
                //        }

                //        for (int row = 3; row <= rowCount; row++)
                //        {
                //            ErrorRow = row;

                //            string LoaiVanDon = worksheet.Cells[row, 1].Value == null ? null : worksheet.Cells[row, 1].Value.ToString().Trim();
                //            string MaPTVC = worksheet.Cells[row, 2].Value == null ? null : worksheet.Cells[row, 2].Value.ToString().Trim();
                //            string MaKH = worksheet.Cells[row, 3].Value == null ? null : worksheet.Cells[row, 3].Value.ToString().Trim();
                //            string MaVanDonKH = worksheet.Cells[row, 4].Value == null ? null : worksheet.Cells[row, 4].Value.ToString().Trim();
                //            string HangTau = worksheet.Cells[row, 5].Value == null ? null : worksheet.Cells[row, 5].Value.ToString().Trim();
                //            string Tau = worksheet.Cells[row, 6].Value == null ? null : worksheet.Cells[row, 6].Value.ToString().Trim();
                //            string DiemDau = worksheet.Cells[row, 7].Value == null ? null : worksheet.Cells[row, 7].Value.ToString().Trim();
                //            string DiemCuoi = worksheet.Cells[row, 8].Value == null ? null : worksheet.Cells[row, 8].Value.ToString().Trim();
                //            string TongKhoiLuong = worksheet.Cells[row, 9].Value == null ? null : worksheet.Cells[row, 9].Value.ToString().Trim();
                //            string TongTheTich = worksheet.Cells[row, 10].Value == null ? null : worksheet.Cells[row, 10].Value.ToString().Trim();
                //            string TongSoKien = worksheet.Cells[row, 11].Value == null ? null : worksheet.Cells[row, 11].Value.ToString().Trim();
                //            string MaLoaiHangHoa = worksheet.Cells[row, 12].Value == null ? null : worksheet.Cells[row, 12].Value.ToString().Trim();
                //            string MaLoaiPhuongTien = worksheet.Cells[row, 13].Value == null ? null : worksheet.Cells[row, 13].Value.ToString().Trim();
                //            string DiemTraRong = worksheet.Cells[row, 14].Value == null ? null : worksheet.Cells[row, 14].Value.ToString().Trim();
                //            string DiemLayRong = worksheet.Cells[row, 14].Value == null ? null : worksheet.Cells[row, 14].Value.ToString().Trim();
                //            string KhoiLuong = worksheet.Cells[row, 15].Value == null ? null : worksheet.Cells[row, 15].Value.ToString().Trim();
                //            string TheTich = worksheet.Cells[row, 16].Value == null ? null : worksheet.Cells[row, 16].Value.ToString().Trim();
                //            string SoKien = worksheet.Cells[row, 17].Value == null ? null : worksheet.Cells[row, 17].Value.ToString().Trim();
                //            string ThoiGianLayTraRong = worksheet.Cells[row, 18].Value == null ? null : worksheet.Cells[row, 18].Value.ToString().Trim();
                //            string ThoiGianHanLenhOrThoiGianHaCang = worksheet.Cells[row, 19].Value == null ? null : worksheet.Cells[row, 19].Value.ToString().Trim();
                //            string ThoiGianLayHang = worksheet.Cells[row, 20].Value == null ? null : worksheet.Cells[row, 20].Value.ToString().Trim();
                //            string ThoiGianTraHang = worksheet.Cells[row, 21].Value == null ? null : worksheet.Cells[row, 21].Value.ToString().Trim();
                //            string GhiChu = worksheet.Cells[row, 22].Value == null ? null : worksheet.Cells[row, 22].Value.ToString().Trim();
                //            string DonViVanTai = worksheet.Cells[row, 23].Value == null ? null : worksheet.Cells[row, 23].Value.ToString().Trim();
                //            string MaSoXe = worksheet.Cells[row, 24].Value == null ? null : worksheet.Cells[row, 24].Value.ToString().Trim();
                //            string MaTaiXe = worksheet.Cells[row, 25].Value == null ? null : worksheet.Cells[row, 25].Value.ToString().Trim();
                //            string Cont_No = worksheet.Cells[row, 26].Value == null ? null : worksheet.Cells[row, 26].Value.ToString().Trim();
                //            string SEAL_NP = worksheet.Cells[row, 27].Value == null ? null : worksheet.Cells[row, 27].Value.ToString().Trim();
                //            string SEAL_HQ = worksheet.Cells[row, 28].Value == null ? null : worksheet.Cells[row, 28].Value.ToString().Trim();
                //            string GhiChuDP = worksheet.Cells[row, 29].Value == null ? null : worksheet.Cells[row, 29].Value.ToString().Trim();

                //            if (!listBooking.Contains(MaVanDonKH))
                //            {
                //                listBooking.Add(MaVanDonKH);

                //                list.Add(new CreateTransport()
                //                {
                //                    LoaiVanDon = LoaiVanDon,
                //                    MaKH = MaKH,
                //                    MaVanDonKH = MaVanDonKH,
                //                    HangTau = HangTau,
                //                    TenTau = Tau,
                //                    DiemCuoi = int.Parse(DiemCuoi),
                //                    DiemDau = int.Parse(DiemDau),
                //                    TongKhoiLuong = string.IsNullOrEmpty(TongKhoiLuong) ? null : double.Parse(TongKhoiLuong),
                //                    TongTheTich = string.IsNullOrEmpty(TongTheTich) ? null : double.Parse(TongTheTich),
                //                    TongSoKien = string.IsNullOrEmpty(TongSoKien) ? null : double.Parse(TongSoKien),
                //                    GhiChu = GhiChu,
                //                    MaPTVC = MaPTVC,
                //                    ThoiGianLayHang = string.IsNullOrEmpty(ThoiGianLayHang) ? null : DateTime.Parse(ThoiGianLayHang),
                //                    ThoiGianTraHang = string.IsNullOrEmpty(ThoiGianTraHang) ? null : DateTime.Parse(ThoiGianTraHang),
                //                    ThoiGianLayRong = string.IsNullOrEmpty(ThoiGianLayTraRong) ? null : DateTime.Parse(ThoiGianLayTraRong),
                //                    ThoiGianTraRong = string.IsNullOrEmpty(ThoiGianLayTraRong) ? null : DateTime.Parse(ThoiGianLayTraRong),
                //                    ThoiGianHaCang = LoaiVanDon == "xuat" ? string.IsNullOrEmpty(ThoiGianHanLenhOrThoiGianHaCang) ? null : DateTime.Parse(ThoiGianHanLenhOrThoiGianHaCang) : null,
                //                    ThoiGianHanLenh = LoaiVanDon == "nhap" ? string.IsNullOrEmpty(ThoiGianHanLenhOrThoiGianHaCang) ? null : DateTime.Parse(ThoiGianHanLenhOrThoiGianHaCang) : null,
                //                    arrHandlings = new List<arrHandling>()
                //                    {
                //                       new arrHandling()
                //                       {
                //                        GhiChu = GhiChuDP,
                //                        ContNo = string.IsNullOrEmpty(Cont_No)? null:Cont_No,
                //                        DonViVanTai =  string.IsNullOrEmpty(DonViVanTai)? null:DonViVanTai,
                //                        PTVanChuyen = MaLoaiPhuongTien ,
                //                        LoaiHangHoa = MaLoaiHangHoa ,
                //                        DonViTinh = "CHUYEN",
                //                        DiemLayRong = string.IsNullOrEmpty(DiemLayTraRong)?null: int.Parse(DiemLayTraRong),
                //                        DiemTraRong = string.IsNullOrEmpty(DiemLayTraRong)?null: int.Parse(DiemLayTraRong),
                //                        KhoiLuong =  string.IsNullOrEmpty(KhoiLuong)?null: double.Parse(KhoiLuong) ,
                //                        TheTich =  string.IsNullOrEmpty(TheTich)?null: double.Parse(TheTich) ,
                //                        SoKien =  string.IsNullOrEmpty(SoKien)?null: double.Parse(SoKien) ,
                //                       },
                //                    }
                //                });
                //            }
                //            else
                //            {
                //                list = list.Where(x => x.MaVanDonKH == MaVanDonKH).ToList();

                //                foreach (var item in list)
                //                {
                //                    item.arrHandlings.Add(new arrHandling
                //                    {
                //                        GhiChu = GhiChuDP,
                //                        ContNo = string.IsNullOrEmpty(Cont_No) ? null : Cont_No,
                //                        DonViVanTai = string.IsNullOrEmpty(DonViVanTai) ? null : DonViVanTai,
                //                        PTVanChuyen = MaLoaiPhuongTien,
                //                        LoaiHangHoa = MaLoaiHangHoa,
                //                        DonViTinh = "CHUYEN",
                //                        DiemLayTraRong = string.IsNullOrEmpty(DiemLayTraRong) ? null : int.Parse(DiemLayTraRong),
                //                        KhoiLuong = string.IsNullOrEmpty(KhoiLuong) ? null : double.Parse(KhoiLuong),
                //                        TheTich = string.IsNullOrEmpty(TheTich) ? null : double.Parse(TheTich),
                //                        SoKien = string.IsNullOrEmpty(SoKien) ? null : double.Parse(SoKien),
                //                    });
                //                }
                //            }
                //        }

                //        foreach (var item in list)
                //        {
                //            if (string.IsNullOrEmpty(item.MaVanDonKH))
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Mã BookingNo";
                //            }
                //            if (string.IsNullOrEmpty(item.LoaiVanDon))
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Loại Vận Đơn";
                //            }
                //            if (string.IsNullOrEmpty(item.MaKH))
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Mã Khách Hàng";
                //            }
                //            if (string.IsNullOrEmpty(item.MaPTVC))
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Loại Hình";
                //            }
                //            if (string.IsNullOrEmpty(item.MaCungDuong))
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Mã Cung Đường";
                //            }

                //            if (item.LoaiVanDon == "nhap")
                //            {
                //                if (item.MaPTVC == "FCL")
                //                {
                //                    foreach (var itemHandling in item.arrHandlings)
                //                    {
                //                        if (itemHandling.DiemLayTraRong == null)
                //                        {
                //                            ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống điểm lấy trả rỗng";
                //                        }

                //                        if (!string.IsNullOrEmpty(itemHandling.ContNo))
                //                        {
                //                            if (!Regex.IsMatch(itemHandling.ContNo.Trim(), "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                //                            {
                //                                ErrorValidate += "Booking " + item.MaVanDonKH + ": Mã ContNo không hợp lệ \r\n";
                //                            }
                //                        }

                //                        if (item.ThoiGianHanLenh == null)
                //                        {
                //                            ErrorValidate += "Booking " + item.MaVanDonKH + ": Không được bỏ trống Thời Gian Hạn Lệnh";
                //                        }
                //                    }
                //                }
                //                else if (item.MaPTVC == "FTL")
                //                {
                //                    foreach (var itemHandling in item.arrHandlings)
                //                    {
                //                        itemHandling.DiemLayTraRong = null;
                //                        itemHandling.ContNo = null;
                //                    }
                //                }
                //                else
                //                {
                //                    ErrorValidate += "Booking " + item.MaVanDonKH + ": Phương thức vận chuyển này không được hỗ trợ";
                //                }
                //            }
                //            else if (item.LoaiVanDon == "xuat")
                //            {
                //                if (item.MaPTVC == "FCL")
                //                {
                //                    foreach (var itemHandling in item.arrHandlings)
                //                    {
                //                        if (itemHandling.DiemLayTraRong == null)
                //                        {
                //                            ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống điểm lấy trả rỗng";
                //                        }

                //                        if (!string.IsNullOrEmpty(itemHandling.ContNo))
                //                        {
                //                            if (!Regex.IsMatch(itemHandling.ContNo, "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                //                            {
                //                                ErrorValidate += "Booking " + item.MaVanDonKH + ": Mã ContNo không hợp lệ";
                //                            }
                //                        }

                //                        if (item.ThoiGianHaCang == null)
                //                        {
                //                            ErrorValidate += "Booking " + item.MaVanDonKH + ": Không được bỏ trống Thời Gian Hạ Cảng";
                //                        }
                //                    }
                //                }
                //                else if (item.MaPTVC == "FTL")
                //                {
                //                    foreach (var itemHandling in item.arrHandlings)
                //                    {
                //                        itemHandling.DiemLayTraRong = null;
                //                        itemHandling.ContNo = null;
                //                    }
                //                }
                //                else
                //                {
                //                    ErrorValidate += "Booking " + item.MaVanDonKH + ": Phương thức vận chuyển này không được hỗ trợ";
                //                }
                //            }
                //            else
                //            {
                //                ErrorValidate += "Booking " + item.MaVanDonKH + ": Loại Vận Đơn này không được hỗ trợ";
                //            }
                //        }

                //        if (ErrorValidate == "")
                //        {
                //            foreach (var item in list)
                //            {
                //                var createTransport = await CreateTransport(item);

                //                if (!createTransport.isSuccess)
                //                {
                //                    return createTransport;
                //                }
                //            }
                //            return new BoolActionResult { isSuccess = true, Message = "Tạo Đơn Hàng Từ Excel Thành Công!" };
                //        }
                //        else
                //        {
                //            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = "Lỗi dữ liệu tại dòng " + ErrorRow };
            }
        }

        public async Task<BoolActionResult> CreateTransport(CreateTransport request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                var checkTransportByBooking = await _context.VanDon.Where(x =>
                x.MaVanDonKh == request.MaVanDonKH
                && x.MaKh == request.MaKH
                && x.TrangThai != 11
                && x.TrangThai != 29).FirstOrDefaultAsync();

                if (checkTransportByBooking != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã booking đã tồn tại trong hệ thống" };
                }

                if (request.DiemDau == request.DiemCuoi)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu và điểm cuối không được giống nhau" };
                }

                var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemDau || x.MaDiaDiem == request.DiemCuoi).ToListAsync();
                if (checkPlace.Count != 2)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu hoặc điểm cuối không tồn tại" };
                }

                var checkCus = await _context.KhachHang.Where(x => x.MaKh == request.MaKH).FirstOrDefaultAsync();
                if (checkCus == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (string.IsNullOrEmpty(request.MaVanDonKH))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng không bỏ trống Mã Vận Đơn Khách Hàng" };
                }

                if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                {
                    if (request.LoaiVanDon == "nhap")
                    {
                        if (request.ThoiGianHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh" };
                        }

                        if (request.ThoiGianTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được nhỏ hơn thời gian trả hàng" };
                            }

                            if (request.ThoiGianTraRong >= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.ThoiGianTraRong >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                    else
                    {
                        if (request.ThoiGianHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.ThoiGianLayRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được nhỏ hơn thời gian trả hàng" };
                            }

                            if (request.ThoiGianLayRong <= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.ThoiGianLayRong <= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                }

                if (request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được lơn hơn hoặc bằng Thời Gian Trả Hàng" };
                    }
                }

                var getMaxTransportID = await _context.VanDon.OrderByDescending(x => x.MaVanDon).Select(x => x.MaVanDon).FirstOrDefaultAsync();
                string transPortId = "";

                if (string.IsNullOrEmpty(getMaxTransportID))
                {
                    transPortId = DateTime.Now.ToString("yy") + "00000001";
                }
                else
                {
                    transPortId = DateTime.Now.ToString("yy") + (int.Parse(getMaxTransportID.Substring(2, getMaxTransportID.Length - 2)) + 1).ToString("00000000");
                }

                if (request.LoaiVanDon == "nhap")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianLayRong = null;
                        request.ThoiGianTraRong = null;
                        request.ThoiGianCoMat = null;
                        request.ThoiGianHanLenh = null;
                    }
                    request.HangTau = null;
                    request.TenTau = null;
                }
                else
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayRong = null;
                        request.ThoiGianTraRong = null;
                    }
                }

                await _context.VanDon.AddAsync(new VanDon()
                {
                    MaPtvc = request.MaPTVC,
                    TongSoKien = request.TongSoKien,
                    MaKh = request.MaKH,
                    HangTau = request.HangTau,
                    Tau = request.TenTau,
                    MaVanDon = transPortId,
                    MaVanDonKh = request.MaVanDonKH,
                    TongThungHang = request.arrHandlings.Count(),
                    LoaiVanDon = request.LoaiVanDon,
                    DiemDau = request.DiemDau,
                    DiemCuoi = request.DiemCuoi,
                    TongKhoiLuong = request.TongKhoiLuong,
                    TongTheTich = request.TongTheTich,
                    ThoiGianTraRong = request.ThoiGianTraRong,
                    ThoiGianLayRong = request.ThoiGianLayRong,
                    ThoiGianCoMat = request.ThoiGianCoMat,
                    ThoiGianHanLenh = request.ThoiGianHanLenh,
                    ThoiGianHaCang = request.ThoiGianHaCang,
                    ThoiGianLayHang = request.ThoiGianLayHang,
                    ThoiGianTraHang = request.ThoiGianTraHang,
                    GhiChu = request.GhiChu,
                    TrangThai = tempData.AccType == "NV" ? 8 : 28,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    foreach (var item in request.arrHandlings)
                    {
                        var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                        if (checkVehicleType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                        }

                        var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                        if (checkGoodsType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                        }

                        var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                        if (checkDVT == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                        }

                        var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                        if (checkPTVC == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                        }


                        int? getEmptyPlace = null;
                        if (item.PTVanChuyen.Contains("CONT"))
                        {
                            if (request.LoaiVanDon == "nhap")
                            {
                                if (!item.DiemTraRong.HasValue)
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                                }
                                getEmptyPlace = item.DiemTraRong;
                            }
                            else
                            {
                                if (!item.DiemLayRong.HasValue)
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                }
                                getEmptyPlace = item.DiemLayRong;
                            }

                            var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                            if (checkPlaceGetEmpty == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                            }
                        }

                        if (tempData.AccType == "KH")
                        {
                            var itemHandling = new DieuPhoi();
                            itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            itemHandling.MaVanDon = transPortId;
                            itemHandling.ContNo = item.ContNo;
                            itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                            itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                            itemHandling.MaDvt = item.DonViTinh;
                            itemHandling.DonViVanTai = item.DonViVanTai;
                            itemHandling.KhoiLuong = item.KhoiLuong;
                            itemHandling.TheTich = item.TheTich;
                            itemHandling.SoKien = item.SoKien;
                            itemHandling.DiemLayRong = item.DiemLayRong;
                            itemHandling.DiemTraRong = item.DiemTraRong;
                            itemHandling.TrangThai = 30;
                            itemHandling.CreatedTime = DateTime.Now;
                            itemHandling.Creator = tempData.UserName;
                            await _context.DieuPhoi.AddAsync(itemHandling);
                        }

                        if (tempData.AccType == "NV")
                        {
                            var itemHandling = new DieuPhoi();

                            if (!string.IsNullOrEmpty(item.DonViVanTai))
                            {
                                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                                if (checkSupplier == null)
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                                }

                                var priceSup = await GetPriceTable(item.DonViVanTai, request.DiemDau, request.DiemCuoi, getEmptyPlace, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);
                                var priceCus = await GetPriceTable(request.MaKH, request.DiemDau, request.DiemCuoi, getEmptyPlace, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);

                                var fPlace = checkPlace.Where(x => x.MaDiaDiem == request.DiemDau).FirstOrDefault();
                                var sPlace = checkPlace.Where(x => x.MaDiaDiem == request.DiemCuoi).FirstOrDefault();

                                var ePlace = new DiaDiem();
                                if (getEmptyPlace.HasValue)
                                {
                                    ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                                }

                                if (priceSup == null)
                                {
                                    return new BoolActionResult
                                    {
                                        isSuccess = false,
                                        Message = "Đơn vị vận tải: "
                                        + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                    + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                    " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                    ", Loại Hàng Hóa:" + checkGoodsType +
                                    ", Đơn Vị Tính: " + checkDVT +
                                    ", Phương thức vận chuyển: " + checkPTVC +
                                     (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                    };
                                }

                                if (priceCus == null)
                                {
                                    return new BoolActionResult
                                    {
                                        isSuccess = false,
                                        Message = "Đơn vị vận tải: "
                                        + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                                    + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                    " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                    ", Loại Hàng Hóa:" + checkGoodsType +
                                    ", Đơn Vị Tính: " + checkDVT +
                                    ", Phương thức vận chuyển: " + checkPTVC +
                                     (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                    };
                                }
                                itemHandling.DonViVanTai = item.DonViVanTai;
                                itemHandling.BangGiaKh = priceCus.ID;
                                itemHandling.BangGiaNcc = priceSup.ID;
                                itemHandling.DonGiaKh = priceCus.DonGia;
                                itemHandling.DonGiaNcc = priceSup.DonGia;
                            }

                            itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            itemHandling.MaVanDon = transPortId;

                            if (item.PTVanChuyen.Contains("CONT"))
                            {
                                itemHandling.ContNo = item.ContNo;

                                if (request.LoaiVanDon == "nhap")
                                {
                                    itemHandling.DiemTraRong = item.DiemTraRong;
                                }
                                else
                                {
                                    itemHandling.DiemLayRong = item.DiemLayRong;
                                }
                            }

                            itemHandling.GhiChu = item.GhiChu;
                            itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                            itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                            itemHandling.MaDvt = item.DonViTinh;
                            itemHandling.KhoiLuong = item.KhoiLuong;
                            itemHandling.TheTich = item.TheTich;
                            itemHandling.SoKien = item.SoKien;
                            itemHandling.CreatedTime = DateTime.Now;
                            itemHandling.Creator = tempData.UserName;
                            itemHandling.TrangThai = 19;
                            var insertHandling = await _context.DieuPhoi.AddAsync(itemHandling);
                        }
                    }

                    var resultDP = await _context.SaveChangesAsync();
                    if (resultDP > 0)
                    {
                        await transaction.CommitAsync();
                        await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " create new Transport with Data: " + JsonSerializer.Serialize(request));
                        return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn Thành Công!" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn thất Bại!" };
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất Bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + " create new Transport with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CreateTransportLess(CreateTransportLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkTransportByBooking = await _context.VanDon.Where(x =>
              x.MaVanDonKh == request.MaVanDonKH
              && x.MaKh == request.MaKH
              && x.TrangThai != 11
              && x.TrangThai != 29).FirstOrDefaultAsync();

                if (request.DiemDau == request.DiemCuoi)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu và điểm cuối không được giống nhau" };
                }

                var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemDau || x.MaDiaDiem == request.DiemCuoi).ToListAsync();

                if (checkPlace.Count != 2)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu hoặc điểm cuối không tồn tại" };
                }

                var checkTransportType = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).FirstOrDefaultAsync();

                if (checkTransportType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phương thức vận chuyển không tồn tại" };
                }

                var checkCustomer = await _context.KhachHang.Where(x => x.MaKh == request.MaKH && x.MaLoaiKh == "KH").FirstOrDefaultAsync();
                if (checkCustomer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                //if (request.TongKhoiLuong < 1 || request.TongTheTich < 1 || request.TongSoKien < 1)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Tổng khối lượng, tổng thể tích, tổng thùng hàng không được nhỏ hơn 1" };
                //}

                if (request.ThoiGianLayHang != null || request.ThoiGianLayHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Mốc thời gian không đúng" };
                    }
                }

                var getMaxTransportID = await _context.VanDon.OrderByDescending(x => x.MaVanDon).Select(x => x.MaVanDon).FirstOrDefaultAsync();
                string transPortId = "";

                if (string.IsNullOrEmpty(getMaxTransportID))
                {
                    transPortId = DateTime.Now.ToString("yy") + "00000001";
                }
                else
                {
                    transPortId = DateTime.Now.ToString("yy") + (int.Parse(getMaxTransportID.Substring(2, getMaxTransportID.Length - 2)) + 1).ToString("00000000");
                }

                await _context.VanDon.AddRangeAsync(new VanDon()
                {
                    MaVanDon = transPortId,
                    Tau = request.TenTau,
                    HangTau = request.HangTau,
                    LoaiVanDon = request.LoaiVanDon,
                    MaPtvc = request.MaPTVC,
                    MaKh = request.MaKH,
                    MaVanDonKh = request.MaVanDonKH,
                    DiemDau = request.DiemDau,
                    DiemCuoi = request.DiemCuoi,
                    TongKhoiLuong = request.TongKhoiLuong,
                    TongSoKien = request.TongSoKien,
                    TongTheTich = request.TongTheTich,
                    TongThungHang = 1,
                    ThoiGianTraHang = request.ThoiGianTraHang,
                    ThoiGianLayHang = request.ThoiGianLayHang,
                    GhiChu = request.GhiChu,
                    TrangThai = tempData.AccType == "NV" ? 8 : 28,
                    ThoiGianTaoDon = DateTime.Now,
                    CreatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Tạo vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<LoadJoinTransports> LoadJoinTransport(JoinTransports request)
        {
            try
            {
                var list = from vd in _context.VanDon
                           join dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon into vdless
                           from vdl in vdless.DefaultIfEmpty()
                           select new { vd, dpl = vdl == null ? null : vdl };

                if (!string.IsNullOrEmpty(request.MaChuyen) && request.TransportIds.Count == 0)
                {
                    list = list.Where(x => x.dpl.MaChuyen == request.MaChuyen && (x.vd.TrangThai != 11 && x.vd.TrangThai != 29));
                }
                else
                {
                    list = list.Where(x => request.TransportIds.Contains(x.vd.MaVanDon) && x.vd.TrangThai == 8);

                    if (list.Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                    }

                    if (list.Where(x => x.vd.MaPtvc == list.Select(x => x.vd.MaPtvc).FirstOrDefault()).Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển (LCL/LTL) " };
                    }

                    if (list.Where(x => x.vd.MaPtvc == list.Select(x => x.vd.MaPtvc).FirstOrDefault()).Select(x => x.vd.MaPtvc).FirstOrDefault() != "LTL")
                    {
                        if (list.Where(x => x.vd.LoaiVanDon == list.Select(y => y.vd.LoaiVanDon).FirstOrDefault()).Count() != request.TransportIds.Count)
                        {
                            return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                        }
                    }
                }

                var data = await list.ToListAsync();

                return new LoadJoinTransports()
                {
                    HangTau = data.Select(x => x.vd.HangTau).FirstOrDefault(),
                    TenTau = data.Select(x => x.vd.Tau).FirstOrDefault(),
                    handlingLess = data.Select(x => x.dpl).FirstOrDefault() != null ? data.Select(x => new CreateHandlingLess()
                    {
                        PTVanChuyen = x.dpl.MaLoaiPhuongTien,
                        DiemLayRong = x.dpl.DiemLayRong,
                        DiemTraRong = x.dpl.DiemTraRong,
                        DonViVanTai = x.dpl.DonViVanTai,
                        XeVanChuyen = x.dpl.MaSoXe,
                        TaiXe = x.dpl.MaTaiXe,
                        GhiChu = x.dpl.GhiChu,
                        Romooc = x.dpl.MaRomooc,
                        CONTNO = x.dpl.ContNo,
                        SEALHQ = x.dpl.SealHq,
                        SEALNP = x.dpl.SealNp,
                        TGHanLenh = x.vd.ThoiGianHanLenh,
                        TGLayRong = x.vd.ThoiGianLayRong,
                        TGTraRong = x.vd.ThoiGianTraRong,
                        TGHaCang = x.vd.ThoiGianHaCang,
                    }).FirstOrDefault() : null,
                    loadTransports = data.Select(x => new LoadTransports()
                    {
                        LoaiVanDon = x.vd.LoaiVanDon,
                        MaPTVC = x.vd.MaPtvc,
                        MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                        MaVanDon = x.vd.MaVanDon,
                        MaVanDonKH = x.vd.MaVanDonKh,
                        DiemDau = _context.DiaDiem.Where(z => z.MaDiaDiem == x.vd.DiemDau).Select(z => z.TenDiaDiem).FirstOrDefault(),
                        DiemCuoi = _context.DiaDiem.Where(z => z.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        TongKhoiLuong = x.vd.TongKhoiLuong,
                        TongTheTich = x.vd.TongTheTich,
                        TongSoKien = x.vd.TongSoKien,
                        ThoiGianLayHang = x.vd.ThoiGianLayHang,
                        ThoiGianTraHang = x.vd.ThoiGianTraHang,
                    }).ToList(),
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<BoolActionResult> CreateHandlingLess(CreateHandlingLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(request.XeVanChuyen))
                {
                    var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.XeVanChuyen).FirstOrDefaultAsync();
                    if (checkVehicle == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Xe này không tồn tại trong hệ thống" };
                    }
                }

                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon) && x.TrangThai == 8).ToListAsync();
                if (loadTransports.Count != request.arrTransports.Count())
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                }

                if (loadTransports.Where(x => x.MaPtvc == loadTransports.Select(x => x.MaPtvc).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển(LCL/LTL) " };
                }

                if (loadTransports.Select(x=>x.MaPtvc).FirstOrDefault() != "LTL")
                {
                    if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                    }
                }

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkStatusDriver = await CheckDriverStats(request.TaiXe);
                if (checkStatusDriver.isSuccess == false)
                {
                    return checkStatusDriver;
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                int? getEmptyPlace = null;
                if (loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Contains("LCL"))
                {
                    if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                    {
                        if (request.TGHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                        }

                        if (request.TGTraRong != null && request.TGLayHang != null && request.TGTraHang != null)
                        {
                            if (request.TGTraRong <= request.TGLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.TGTraRong <= request.TGTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }

                        if (!request.DiemTraRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemTraRong;
                    }
                    else
                    {
                        if (request.TGHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.TGLayRong != null && request.TGLayHang != null && request.TGTraHang != null)
                        {
                            if (request.TGLayRong >= request.TGLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.TGLayRong >= request.TGTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }

                        if (request.DiemLayRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemLayRong;
                    }

                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }

                if (request.TGLayHang != null && request.TGTraHang != null)
                {
                    if (request.TGLayHang >= request.TGTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thời gian Lấy Hàng không được lớn hơn Thời Gian Trả Hàng" };
                    }
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                string MaVanDonChung = loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");


                foreach (var item in loadTransports)
                {
                    foreach (var itemRequest in request.arrTransports)
                    {
                        if (itemRequest.MaVanDon == item.MaVanDon)
                        {
                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            var priceSup = await GetPriceTable(request.DonViVanTai, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                            var priceCus = await GetPriceTable(item.MaKh, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

                            var fPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemDau).FirstOrDefault();
                            var sPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemCuoi).FirstOrDefault();

                            var ePlace = new DiaDiem();
                            if (getEmptyPlace.HasValue)
                            {
                                ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                            }

                            if (priceSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                };
                            }

                            if (priceCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                };
                            }


                            if (item.MaPtvc == "LTL")
                            {
                                item.ThoiGianLayRong = null;
                                item.ThoiGianTraHang = null;
                                item.ThoiGianHaCang = null;
                                item.ThoiGianHanLenh = null;
                                request.Romooc = null;
                            }

                            if (item.MaPtvc == "LCL")
                            {
                                if (item.LoaiVanDon == "nhap")
                                {
                                    item.ThoiGianTraRong = request.TGTraRong;
                                    item.ThoiGianHanLenh = request.TGHanLenh;
                                    item.ThoiGianHaCang = null;
                                }

                                if (item.LoaiVanDon == "xuat")
                                {
                                    item.ThoiGianLayRong = request.TGLayRong;
                                    item.ThoiGianHanLenh = null;
                                    item.ThoiGianHaCang = request.TGHaCang;
                                }
                            }

                            int trangthai = 19;
                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe))
                            {
                                item.TrangThai = 9;
                                trangthai = 27;
                                var handleVehicleStatus = await HandleVehicleStatus(trangthai, request.XeVanChuyen);
                            }
                            _context.VanDon.Update(item);

                            var insertHandling = await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = item.MaVanDon,
                                MaChuyen = MaVanDonChung,
                                MaSoXe = request.XeVanChuyen,
                                MaTaiXe = request.TaiXe,
                                MaLoaiHangHoa = itemRequest.MaLoaiHangHoa,
                                MaLoaiPhuongTien = request.PTVanChuyen,
                                MaDvt = itemRequest.MaDVT,
                                DonViVanTai = request.DonViVanTai,
                                BangGiaKh = priceCus.ID,
                                BangGiaNcc = priceSup.ID,
                                DonGiaKh = priceCus.DonGia,
                                DonGiaNcc = priceSup.DonGia,
                                MaRomooc = request.Romooc,
                                ContNo = request.CONTNO,
                                SealNp = request.SEALNP,
                                SealHq = request.SEALHQ,
                                SoKien = item.TongSoKien,
                                KhoiLuong = item.TongKhoiLuong,
                                TheTich = item.TongTheTich,
                                GhiChu = item.GhiChu,
                                DiemLayRong = request.DiemLayRong,
                                DiemTraRong = request.DiemTraRong,
                                TrangThai = trangthai,
                                CreatedTime = DateTime.Now,
                                Creator = tempData.UserName,
                            });

                            await _context.SaveChangesAsync();

                            var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.MaKh, itemRequest.MaLoaiHangHoa, item.DiemDau, item.DiemCuoi, getEmptyPlace, null, request.PTVanChuyen);
                            foreach (var sfp in getSubFee)
                            {
                                await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                                {
                                    PriceId = sfp.PriceId,
                                    MaDieuPhoi = insertHandling.Entity.Id,
                                    CreatedDate = DateTime.Now,
                                    Creator = tempData.UserName,
                                });
                            }
                        }
                    }
                }

                var result = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new BoolActionResult { isSuccess = true, Message = "Ghép vận đơn với xe thành công!" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            };
        }

        public async Task<BoolActionResult> UpdateHandling(int id, UpdateHandling request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkById = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                if (checkById.TrangThai == 30)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng duyệt chuyến trước khi điều phối" };
                }

                if (checkById.TrangThai == 20 || checkById.TrangThai == 21 || checkById.TrangThai == 31)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể cập nhật chuyến này nữa" };
                }

                var getTransport = await _context.VanDon.Where(x => x.MaVanDon == checkById.MaVanDon).FirstOrDefaultAsync();

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();

                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == request.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                if (checkGoodsType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                }
                var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == request.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                if (checkDVT == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                }

                if (!string.IsNullOrEmpty(request.MaTaiXe))
                {
                    var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == request.MaTaiXe).FirstOrDefaultAsync();
                    if (checkDriver == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Tài xế không tồn tại \r\n" };
                    }

                    if (checkById.MaTaiXe != request.MaTaiXe)
                    {
                        var checkStatusDriver = await CheckDriverStats(request.MaTaiXe);
                        if (checkStatusDriver.isSuccess == false)
                        {
                            return checkStatusDriver;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(request.MaSoXe))
                {
                    var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.MaSoXe).FirstOrDefaultAsync();
                    if (checkVehicle == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Xe vận chuyển không tồn tại \r\n" };
                    }

                    if (!request.PTVanChuyen.Contains(checkVehicle.MaLoaiPhuongTien))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại xe không khớp với xe vận chuyển \r\n" };
                    }

                    if (checkById.MaSoXe != request.MaSoXe)
                    {
                        if (checkVehicle.TrangThai == 33)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Xe này đã được điều phối cho chuyến khác" };
                        }

                        if (checkVehicle.TrangThai == 34)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Xe này đang vận chuyển hàng hóa cho chuyến khác" };
                        }
                    }
                    var handleVehicleStatus = await HandleVehicleStatus(checkById.TrangThai, request.MaSoXe, checkById.MaSoXe);
                }

                int? getEmptyPlace = null;
                if (request.PTVanChuyen.Contains("CONT"))
                {
                    if (getTransport.LoaiVanDon == "nhap")
                    {
                        if (!request.DiemTraRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemTraRong;
                    }
                    else
                    {
                        if (!request.DiemLayRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemLayRong;
                    }

                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }
                else
                {
                    request.DiemLayRong = null;
                    request.DiemTraRong = null;
                    checkById.ThoiGianTraRongThucTe = null;
                    checkById.ThoiGianLayRongThucTe = null;
                    request.ThoiGianCoMatThucTe = null;
                    request.ThoiGianHaCangThucTe = null;
                }

                //if (getTransport.MaPtvc == "FCL" || getTransport.MaPtvc == "FTL")
                //{
                //    var checkVehicleOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == getTransport.MaVanDon && x.MaSoXe == request.MaSoXe && x.Id != id).FirstOrDefaultAsync();

                //    if (checkVehicleOfTransport != null)
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Xe này đã được điều phối trước đó, vui lòng chọn xe khác" };
                //    }
                //}

                if (checkById.TrangThai == 17 || checkById.TrangThai == 18 || checkById.TrangThai == 20)
                {
                    checkById.ThoiGianLayHangThucTe = request.ThoiGianLayHangThucTe;
                    checkById.ThoiGianTraRongThucTe = request.ThoiGianTraRongThucTe;
                    checkById.ThoiGianLayRongThucTe = request.ThoiGianLayRongThucTe;
                    checkById.ThoiGianCoMatThucTe = request.ThoiGianCoMatThucTe;
                }

                if (checkById.TrangThai == 19 || checkById.TrangThai == 27)
                {
                    var priceSup = await GetPriceTable(request.DonViVanTai, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);
                    var priceCus = await GetPriceTable(getTransport.MaKh, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);

                    var fPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == getTransport.DiemDau).FirstOrDefault();
                    var sPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == getTransport.DiemCuoi).FirstOrDefault();

                    var ePlace = new DiaDiem();
                    if (getEmptyPlace.HasValue)
                    {
                        ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                    }

                    if (priceSup == null)
                    {
                        return new BoolActionResult
                        {
                            isSuccess = false,
                            Message = "Đơn vị vận tải: "
                            + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + " )" +
                        " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + " )" +
                        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                        ", Loại Hàng Hóa:" + checkGoodsType +
                        ", Đơn Vị Tính: " + checkDVT +
                        ", Phương thức vận chuyển: " + getTransport.MaPtvc +
                         (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + ")")
                        };
                    }

                    if (priceCus == null)
                    {
                        return new BoolActionResult
                        {
                            isSuccess = false,
                            Message = "Đơn vị vận tải: "
                            + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + " )" +
                        " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + " )" +
                        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                        ", Loại Hàng Hóa:" + checkGoodsType +
                        ", Đơn Vị Tính: " + checkDVT +
                        ", Phương thức vận chuyển: " + getTransport.MaPtvc +
                         (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefault() + ")")
                        };
                    }

                    checkById.MaLoaiHangHoa = request.LoaiHangHoa;
                    checkById.MaLoaiPhuongTien = request.PTVanChuyen;
                    checkById.MaDvt = request.DonViTinh;
                    checkById.DonViVanTai = request.DonViVanTai;
                    checkById.BangGiaNcc = priceSup.ID;
                    checkById.DonGiaNcc = priceSup.DonGia;
                    checkById.BangGiaKh = priceCus.ID;
                    checkById.DonGiaKh = priceCus.DonGia;
                    checkById.SoKien = request.SoKien;
                    checkById.KhoiLuong = request.KhoiLuong;
                    checkById.TheTich = request.TheTich;
                    checkById.DiemLayRong = request.DiemLayRong;
                    checkById.DiemTraRong = request.DiemTraRong;
                    checkById.TrangThai = 27;
                    getTransport.TrangThai = 9;
                }

                checkById.MaSoXe = request.MaSoXe;
                checkById.ContNo = request.ContNo;
                checkById.SealHq = request.SealHq;
                checkById.SealNp = request.SealNp;
                checkById.MaTaiXe = request.MaTaiXe;
                checkById.MaRomooc = request.MaRomooc;
                checkById.GhiChu = request.GhiChu;
                checkById.UpdatedTime = DateTime.Now;
                checkById.Updater = tempData.UserName;
                getTransport.Updater = tempData.UserName;

                _context.Update(getTransport);
                _context.Update(checkById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var getSubFee = await _subFeePrice.GetListSubFeePriceActive(getTransport.MaKh, checkById.MaLoaiHangHoa, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, checkById.Id, checkById.MaLoaiPhuongTien);
                    foreach (var sfp in getSubFee)
                    {
                        await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                        {
                            PriceId = sfp.PriceId,
                            MaDieuPhoi = checkById.Id,
                            CreatedDate = DateTime.Now,
                            Creator = tempData.UserName,
                        });
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " update handling with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Điều Phối Chuyến Thành Công!" };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Điều Phối Chuyến Thất Bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                if (loadTransports.Where(x => x.TrangThai == 22).Count() > 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể cập nhật chuyến này nữa" };
                }

                if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                }

                if (loadTransports.Where(x => x.MaPtvc == loadTransports.Select(x => x.MaPtvc).FirstOrDefault()).Count() != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển(LCL/LTL) " };
                }

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                int? getEmptyPlace = null;
                if (loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Contains("LCL"))
                {
                    if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                    {
                        if (request.TGHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                        }

                        if (request.TGTraRong != null && request.TGLayHang != null && request.TGTraHang != null)
                        {
                            if (request.TGTraRong <= request.TGLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.TGTraRong <= request.TGTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }

                        if (!request.DiemTraRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemTraRong;
                    }
                    else
                    {
                        if (request.TGHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.TGLayRong != null && request.TGLayHang != null && request.TGTraHang != null)
                        {
                            if (request.TGLayRong >= request.TGLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.TGLayRong >= request.TGTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }

                        if (request.DiemLayRong.HasValue)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                        }
                        getEmptyPlace = request.DiemLayRong;
                    }

                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                    if (checkPlaceGetEmpty == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                    }
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                var data = from vd in _context.VanDon
                           join dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           where dp.MaChuyen == handlingId
                           select new { vd, dp };

                if (data.Select(x => x.dp.MaTaiXe).FirstOrDefault() != request.TaiXe)
                {
                    var checkStatusDriver = await CheckDriverStats(request.TaiXe);
                    if (checkStatusDriver.isSuccess == false)
                    {
                        return checkStatusDriver;
                    }
                }

                if (!string.IsNullOrEmpty(request.XeVanChuyen))
                {
                    var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.XeVanChuyen).FirstOrDefaultAsync();
                    if (checkVehicle == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Xe này không tồn tại trong hệ thống" };
                    }

                    if (await data.Select(x => x.dp.MaSoXe).FirstOrDefaultAsync() != request.XeVanChuyen)
                    {
                        if (checkVehicle.TrangThai == 33)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Xe này đã được điều phối cho chuyến khác" };
                        }

                        if (checkVehicle.TrangThai == 34)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Xe này đang vận chuyển hàng hóa cho chuyến khác" };
                        }
                    }
                }


                #region loại bỏ các vận đơn đã được ghép trước đó

                var dataRemove = await data.Where(x => !request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToListAsync();
                _context.DieuPhoi.RemoveRange(dataRemove.Select(x => x.dp));
                dataRemove.ForEach(x =>
                {
                    x.vd.TrangThai = 8;
                    x.vd.UpdatedTime = DateTime.Now;
                    x.vd.Updater = tempData.UserName;
                });

                #endregion loại bỏ các vận đơn đã được ghép trước đó

                #region Cập nhật thông tin các vận đơn đã được ghép

                var listTransport = await data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToListAsync();
                foreach (var x in listTransport.Select(x => x.vd).ToList())
                {
                    if (x.TrangThai == 11 || x.TrangThai == 22 || x.TrangThai == 29)
                    {
                        continue;
                    }

                    x.UpdatedTime = DateTime.Now;
                    x.Updater = tempData.UserName;

                    if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe) && x.TrangThai == 8)
                    {
                        x.TrangThai = 9;
                    }

                    if (x.MaPtvc == "LTL")
                    {
                        x.ThoiGianLayRong = null;
                        x.ThoiGianTraRong = null;
                        x.ThoiGianHaCang = null;
                        x.ThoiGianHanLenh = null;
                        request.DiemLayRong = null;
                        request.DiemTraRong = null;
                        request.Romooc = null;
                    }

                    if (x.MaPtvc == "LCL")
                    {
                        if (x.LoaiVanDon == "nhap")
                        {
                            x.ThoiGianHanLenh = request.TGHanLenh;
                            x.ThoiGianHaCang = null;
                            x.ThoiGianTraRong = request.TGTraRong;
                        }

                        if (x.LoaiVanDon == "xuat")
                        {
                            x.ThoiGianLayRong = request.TGLayRong;
                            x.ThoiGianHanLenh = null;
                            x.ThoiGianHaCang = request.TGHaCang;
                        }
                    }
                    _context.VanDon.Update(x);
                }

                #endregion Cập nhật thông tin các vận đơn đã được ghép

                #region Cập nhật thông tin các chuyến đã tồn tại trong Db

                foreach (var item in data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToList())
                {
                    foreach (var itemRequest in request.arrTransports)
                    {
                        if (item.dp.MaVanDon == itemRequest.MaVanDon)
                        {
                            if (item.dp.TrangThai == 31 || item.dp.TrangThai == 21 || item.dp.TrangThai == 20)
                            {
                                continue;
                            }

                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe))
                            {
                                if (item.dp.TrangThai == 19)
                                {
                                    item.dp.TrangThai = 27;
                                }
                                var handleVehicleStatus = await HandleVehicleStatus(item.dp.TrangThai, request.XeVanChuyen, item.dp.MaSoXe);
                            }

                            var priceSup = await GetPriceTable(request.DonViVanTai, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);
                            var priceCus = await GetPriceTable(item.vd.MaKh, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);

                            var fPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.vd.DiemDau).FirstOrDefault();
                            var sPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.vd.DiemCuoi).FirstOrDefault();

                            var ePlace = new DiaDiem();
                            if (getEmptyPlace.HasValue)
                            {
                                ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                            }

                            if (priceSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.vd.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                };
                            }

                            if (priceCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == item.vd.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.vd.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                };
                            }

                            item.dp.MaSoXe = request.XeVanChuyen;
                            item.dp.MaTaiXe = request.TaiXe;
                            item.dp.MaLoaiHangHoa = itemRequest.MaLoaiHangHoa;
                            item.dp.MaLoaiPhuongTien = request.PTVanChuyen;
                            item.dp.MaDvt = itemRequest.MaDVT;
                            item.dp.DonViVanTai = request.DonViVanTai;
                            item.dp.BangGiaKh = priceCus.ID;
                            item.dp.BangGiaNcc = priceSup.ID;
                            item.dp.DonGiaKh = priceCus.DonGia;
                            item.dp.DonGiaNcc = priceSup.DonGia;
                            item.dp.MaRomooc = request.Romooc;
                            item.dp.ContNo = request.CONTNO;
                            item.dp.SealNp = request.SEALNP;
                            item.dp.SealHq = request.SEALHQ;
                            item.dp.DiemLayRong = request.DiemLayRong;
                            item.dp.DiemTraRong = request.DiemTraRong;
                            item.dp.Updater = tempData.UserName;

                            var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.vd.MaKh, itemRequest.MaLoaiHangHoa, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, item.dp.Id, item.dp.MaLoaiPhuongTien);

                            foreach (var sfp in getSubFee)
                            {
                                await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                                {
                                    PriceId = sfp.PriceId,
                                    MaDieuPhoi = item.dp.Id,
                                    CreatedDate = DateTime.Now,
                                    Creator = tempData.UserName,
                                });
                            }
                        }
                    }
                }

                #endregion Cập nhật thông tin các chuyến đã tồn tại trong Db

                #region Xử lý các vận đơn mới được ghép thêm vào

                var arrDataRequest = request.arrTransports.Where(x => !data.Select(y => y.dp.MaVanDon).Contains(x.MaVanDon));
                var getListNewTransport = await _context.VanDon.Where(x => arrDataRequest.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();
                foreach (var item in getListNewTransport)
                {
                    foreach (var itemRequest in arrDataRequest)
                    {
                        if (itemRequest.MaVanDon == item.MaVanDon)
                        {
                            var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == itemRequest.MaLoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                            if (checkGoodsType == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                            }

                            var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == itemRequest.MaDVT).Select(x => x.TenDvt).FirstOrDefaultAsync();
                            if (checkDVT == null)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                            }

                            var priceSup = await GetPriceTable(request.DonViVanTai, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                            var priceCus = await GetPriceTable(item.MaKh, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

                            var fPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemDau).FirstOrDefault();
                            var sPlace = _context.DiaDiem.Where(x => x.MaDiaDiem == item.DiemCuoi).FirstOrDefault();

                            var ePlace = new DiaDiem();
                            if (getEmptyPlace.HasValue)
                            {
                                ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                            }

                            if (priceSup == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                };
                            }

                            if (priceCus == null)
                            {
                                return new BoolActionResult
                                {
                                    isSuccess = false,
                                    Message = "Đơn vị vận tải: "
                                    + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem) + " )" +
                                ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                ", Loại Hàng Hóa:" + checkGoodsType +
                                ", Đơn Vị Tính: " + checkDVT +
                                ", Phương thức vận chuyển: " + item.MaPtvc +
                                 (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem) + ")")
                                };
                            }

                            int trangthai = 19;
                            if (!string.IsNullOrEmpty(request.XeVanChuyen) && !string.IsNullOrEmpty(request.TaiXe) && item.TrangThai == 8)
                            {
                                trangthai = 27;
                            }

                            var insertHandling = await _context.DieuPhoi.AddAsync(new DieuPhoi()
                            {
                                MaVanDon = item.MaVanDon,
                                MaChuyen = handlingId,
                                MaSoXe = request.XeVanChuyen,
                                MaTaiXe = request.TaiXe,
                                MaLoaiHangHoa = itemRequest.MaLoaiHangHoa,
                                MaLoaiPhuongTien = request.PTVanChuyen,
                                MaDvt = itemRequest.MaDVT,
                                DonViVanTai = request.DonViVanTai,
                                BangGiaKh = priceCus.ID,
                                BangGiaNcc = priceSup.ID,
                                DonGiaKh = priceCus.DonGia,
                                DonGiaNcc = priceSup.DonGia,
                                MaRomooc = request.Romooc,
                                ContNo = request.CONTNO,
                                SealNp = request.SEALNP,
                                SealHq = request.SEALHQ,
                                SoKien = item.TongSoKien,
                                KhoiLuong = item.TongKhoiLuong,
                                TheTich = item.TongTheTich,
                                GhiChu = item.GhiChu,
                                DiemLayRong = request.DiemLayRong,
                                DiemTraRong = request.DiemTraRong,
                                TrangThai = trangthai,
                                CreatedTime = DateTime.Now,
                                Creator = tempData.UserName,
                                Updater = tempData.UserName,
                            });

                            var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.MaKh, itemRequest.MaLoaiHangHoa, item.DiemDau, item.DiemCuoi, getEmptyPlace, null, request.PTVanChuyen);
                            foreach (var sfp in getSubFee)
                            {
                                await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                                {
                                    PriceId = sfp.PriceId,
                                    MaDieuPhoi = insertHandling.Entity.Id,
                                    CreatedDate = DateTime.Now,
                                    Creator = tempData.UserName,
                                });
                            }
                        }
                    }
                }

                #endregion Xử lý các vận đơn mới được ghép thêm vào

                var result = await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new BoolActionResult() { isSuccess = true, Message = "Cập nhật điều phối thành công!" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult() { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<UpdateTransportLess> GetTransportLessById(string transportId)
        {
            try
            {
                var transport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (transport == null)
                {
                    return null;
                }

                return new UpdateTransportLess()
                {
                    MaKH = transport.MaKh,
                    MaVanDonKH = transport.MaVanDonKh,
                    LoaiVanDon = transport.LoaiVanDon,
                    HangTau = transport.HangTau,
                    TenTau = transport.Tau,
                    DiemLayHang = transport.DiemDau,
                    DiemTraHang = transport.DiemCuoi,
                    TongKhoiLuong = transport.TongKhoiLuong,
                    TongTheTich = transport.TongTheTich,
                    TongSoKien = transport.TongSoKien,
                    GhiChu = transport.GhiChu,
                    MaPTVC = transport.MaPtvc,
                    ThoiGianLayHang = transport.ThoiGianLayHang,
                    ThoiGianTraHang = transport.ThoiGianTraHang,
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetTransport> GetTransportById(string transportId)
        {
            try
            {
                var getTransport = from transport in _context.VanDon
                                   where transport.MaVanDon == transportId
                                   select new { transport };

                return await getTransport.Select(x => new GetTransport()
                {
                    MaPTVC = x.transport.MaPtvc,
                    MaVanDonKH = x.transport.MaVanDonKh,
                    DiemDau = x.transport.DiemDau,
                    DiemCuoi = x.transport.DiemCuoi,
                    MaVanDon = x.transport.MaVanDon,
                    LoaiVanDon = x.transport.LoaiVanDon,
                    MaKh = x.transport.MaKh,
                    ThoiGianLayRong = x.transport.ThoiGianLayRong,
                    ThoiGianTraRong = x.transport.ThoiGianTraRong,
                    DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.transport.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.transport.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                    TongKhoiLuong = x.transport.TongKhoiLuong,
                    TongTheTich = x.transport.TongTheTich,
                    TongSoKien = x.transport.TongSoKien,
                    ThoiGianCoMat = x.transport.ThoiGianCoMat,
                    ThoiGianHaCang = x.transport.ThoiGianHaCang,
                    ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
                    ThoiGianLayHang = x.transport.ThoiGianLayHang,
                    ThoiGianTraHang = x.transport.ThoiGianTraHang,
                    ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                    HangTau = x.transport.HangTau,
                    TenTau = x.transport.Tau,
                    GhiChu = x.transport.GhiChu,
                    arrHandlings = _context.DieuPhoi.Where(y => y.MaVanDon == transportId).Select(y => new arrHandling()
                    {
                        PTVanChuyen = y.MaLoaiPhuongTien,
                        LoaiHangHoa = y.MaLoaiHangHoa,
                        DonViTinh = y.MaDvt,
                        DiemLayRong = y.DiemLayRong,
                        DiemTraRong = y.DiemTraRong,
                        KhoiLuong = y.KhoiLuong,
                        TheTich = y.TheTich,
                        SoKien = y.SoKien,
                    }).ToList(),
                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> UpdateTransport(string transPortId, UpdateTransport request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.DiemDau == request.DiemCuoi)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu và điểm cuối không được giống nhau" };
                }

                var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemDau || x.MaDiaDiem == request.DiemCuoi).ToListAsync();
                if (checkPlace.Count != 2)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu hoặc điểm cuối không tồn tại" };
                }

                if (string.IsNullOrEmpty(request.MaVanDonKH))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng không bỏ trống Mã Vận Đơn Khách Hàng" };
                }

                if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                {
                    if (request.LoaiVanDon == "nhap")
                    {
                        if (request.ThoiGianHanLenh == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh" };
                        }

                        if (request.ThoiGianTraRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được nhỏ hơn thời gian trả hàng" };
                            }

                            if (request.ThoiGianTraRong >= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }

                            if (request.ThoiGianTraRong >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                    else
                    {
                        if (request.ThoiGianHaCang == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                        }

                        if (request.ThoiGianLayRong != null && request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                        {
                            if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được nhỏ hơn thời gian trả hàng" };
                            }

                            if (request.ThoiGianLayRong <= request.ThoiGianLayHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                            }
                            if (request.ThoiGianLayRong <= request.ThoiGianTraHang)
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                            }
                        }
                    }
                }

                if (request.ThoiGianLayHang != null && request.ThoiGianTraHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy hàng không được lơn hơn hoặc bằng Thời Gian Trả Hàng" };
                    }
                }

                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transPortId).FirstOrDefaultAsync();

                if (checkTransport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                if (tempData.AccType == "KH")
                {
                    if (checkTransport.TrangThai != 28)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không thể chỉnh sửa vận đơn này nữa" };
                    }
                }

                if (tempData.AccType == "NV")
                {
                    if (checkTransport.TrangThai != 8)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                    }
                }

                if (request.LoaiVanDon == "nhap")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianTraRong = null;
                        request.ThoiGianCoMat = null;
                        request.ThoiGianHanLenh = null;
                    }
                    request.HangTau = null;
                    request.TenTau = null;
                }
                else
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() == 0)
                    {
                        request.ThoiGianHaCang = null;
                        request.ThoiGianLayRong = null;
                        request.HangTau = null;
                        request.TenTau = null;
                    }
                }

                checkTransport.LoaiVanDon = request.LoaiVanDon;
                checkTransport.MaPtvc = request.MaPTVC;
                checkTransport.HangTau = request.HangTau;
                checkTransport.Tau = request.TenTau;
                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.DiemDau = request.DiemDau;
                checkTransport.DiemCuoi = request.DiemCuoi;
                checkTransport.MaKh = request.MaKH;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.TongTheTich = request.TongTheTich;
                checkTransport.TongSoKien = request.TongSoKien;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;
                checkTransport.ThoiGianLayRong = request.ThoiGianLayRong;
                checkTransport.ThoiGianTraRong = request.ThoiGianTraRong;
                checkTransport.ThoiGianCoMat = request.ThoiGianCoMat;
                checkTransport.ThoiGianHaCang = request.ThoiGianHaCang;
                checkTransport.ThoiGianHanLenh = request.ThoiGianHanLenh;
                checkTransport.UpdatedTime = DateTime.Now;
                checkTransport.GhiChu = request.GhiChu;
                checkTransport.Updater = tempData.UserName;

                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transPortId).ToListAsync();

                    _context.SubFeeByContract.RemoveRange(_context.SubFeeByContract.Where(x => getListHandling.Select(y => y.Id).Contains(x.MaDieuPhoi)));
                    _context.SfeeByTcommand.RemoveRange(_context.SfeeByTcommand.Where(x => getListHandling.Select(y => y.Id).Contains(x.IdTcommand)));
                    _context.DieuPhoi.RemoveRange(getListHandling);

                    foreach (var item in request.arrHandlings)
                    {
                        var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                        if (checkVehicleType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                        }

                        var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == item.LoaiHangHoa).Select(x => x.TenLoaiHangHoa).FirstOrDefaultAsync();
                        if (checkGoodsType == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
                        }
                        var checkDVT = await _context.DonViTinh.Where(x => x.MaDvt == item.DonViTinh).Select(x => x.TenDvt).FirstOrDefaultAsync();
                        if (checkDVT == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Đơn Vị Tính Không Tồn Tại" };
                        }
                        var checkPTVC = await _context.PhuongThucVanChuyen.Where(x => x.MaPtvc == request.MaPTVC).Select(x => x.TenPtvc).FirstOrDefaultAsync();
                        if (checkPTVC == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Phương Thức Vận Chuyển Không Tồn Tại" };
                        }

                        if (request.LoaiVanDon == "nhap")
                        {
                            if (request.MaPTVC == "FLC")
                            {
                                item.DiemLayRong = null;
                            }
                            else
                            {
                                item.DiemLayRong = null;
                                item.DiemTraRong = null;
                            }
                        }
                        else if (request.LoaiVanDon == "xuat")
                        {
                            if (request.MaPTVC == "FLC")
                            {
                                item.DiemTraRong = null;
                            }
                            else
                            {
                                item.DiemLayRong = null;
                                item.DiemTraRong = null;
                            }
                        }

                        if (tempData.AccType == "KH")
                        {
                            var itemHandling = new DieuPhoi();
                            itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            itemHandling.MaVanDon = transPortId;
                            itemHandling.ContNo = item.ContNo;
                            itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                            itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                            itemHandling.MaDvt = item.DonViTinh;
                            itemHandling.DonViVanTai = item.DonViVanTai;
                            itemHandling.KhoiLuong = item.KhoiLuong;
                            itemHandling.TheTich = item.TheTich;
                            itemHandling.SoKien = item.SoKien;
                            itemHandling.DiemLayRong = item.DiemLayRong;
                            itemHandling.DiemTraRong = item.DiemTraRong;
                            itemHandling.TrangThai = 30;
                            itemHandling.CreatedTime = DateTime.Now;
                            itemHandling.Creator = tempData.UserName;
                            await _context.DieuPhoi.AddAsync(itemHandling);
                        }

                        if (tempData.AccType == "NV")
                        {
                            //var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                            //if (checkSupplier == null)
                            //{
                            //    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                            //}

                            //var priceSup = await GetPriceTable(item.DonViVanTai, request.MaCungDuong, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);
                            //var priceCus = await GetPriceTable(request.MaKH, request.MaCungDuong, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);

                            //if (priceSup == null)
                            //{
                            //    return new BoolActionResult
                            //    {
                            //        isSuccess = false,
                            //        Message = "Đơn vị vận tải: "
                            //        + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                            //    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            //    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            //    ", Loại Hàng Hóa:" + checkGoodsType +
                            //    ", Đơn Vị Tính: " + checkDVT +
                            //    ", Phương thức vận chuyển: " + checkPTVC
                            //    };
                            //}

                            //if (priceCus == null)
                            //{
                            //    return new BoolActionResult
                            //    {
                            //        isSuccess = false,
                            //        Message = "Khách Hàng: "
                            //       + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            //    + " chưa có bảng giá cho Cung Đường: " + request.MaCungDuong +
                            //    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            //    ", Loại Hàng Hóa:" + checkGoodsType +
                            //    ", Đơn Vị Tính: " + checkDVT +
                            //    ", Phương thức vận chuyển: " + checkPTVC
                            //    };
                            //}

                            var itemHandling = new DieuPhoi();
                            itemHandling.MaChuyen = request.MaPTVC.Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            itemHandling.MaVanDon = transPortId;
                            itemHandling.ContNo = item.ContNo;
                            itemHandling.MaLoaiHangHoa = item.LoaiHangHoa;
                            itemHandling.MaLoaiPhuongTien = item.PTVanChuyen;
                            itemHandling.MaDvt = item.DonViTinh;
                            itemHandling.DonViVanTai = item.DonViVanTai;
                            //itemHandling.BangGiaKh = priceCus.ID;
                            //itemHandling.BangGiaNcc = priceSup.ID;
                            //itemHandling.DonGiaKh = priceCus.DonGia;
                            //itemHandling.DonGiaNcc = priceSup.DonGia;
                            itemHandling.KhoiLuong = item.KhoiLuong;
                            itemHandling.TheTich = item.TheTich;
                            itemHandling.SoKien = item.SoKien;
                            itemHandling.DiemLayRong = item.DiemLayRong;
                            itemHandling.DiemTraRong = item.DiemTraRong;
                            itemHandling.TrangThai = 19;
                            itemHandling.CreatedTime = DateTime.Now;
                            itemHandling.Creator = tempData.UserName;
                            var insertHandling = await _context.DieuPhoi.AddAsync(itemHandling);
                        }
                    }

                    var resultDP = await _context.SaveChangesAsync();

                    if (resultDP > 0)
                    {
                        await transaction.CommitAsync();
                        await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " Update Transport with Data: " + JsonSerializer.Serialize(request));
                        return new BoolActionResult { isSuccess = true, Message = "Cập nhật vận đơn thành công" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Cập nhật vận đơn thất bại" };
                    }
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + " Update Transport with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request)
        {
            try
            {
                if (request.LoaiVanDon != "nhap" && request.LoaiVanDon != "xuat")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không tồn tại loại vận đơn" + request.LoaiVanDon };
                }

                if (request.ThoiGianLayHang != null || request.ThoiGianLayHang != null)
                {
                    if (request.ThoiGianLayHang >= request.ThoiGianTraHang)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Mốc thời gian không đúng" };
                    }
                }

                //if (request.TongKhoiLuong < 1 || request.TongTheTich < 1 || request.TongSoKien < 1)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Tổng khối lượng, tổng thể tích, tổng thùng hàng không được nhỏ hơn 1" };
                //}

                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (checkTransport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn không tồn tại" };
                }

                if (request.DiemDau == request.DiemCuoi)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu và điểm cuối không được giống nhau" };
                }

                var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.DiemDau || x.MaDiaDiem == request.DiemCuoi).ToListAsync();
                if (checkPlace.Count != 2)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Điểm đầu hoặc điểm cuối không tồn tại" };
                }


                if (tempData.AccType == "NV")
                {
                    if (checkTransport.TrangThai != 8)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                    }
                }

                if (tempData.AccType == "KH")
                {
                    if (checkTransport.TrangThai != 28)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vận đơn này không thể sửa nữa" };
                    }
                }

                checkTransport.MaVanDonKh = request.MaVanDonKH;
                checkTransport.MaKh = request.MaKH;
                checkTransport.DiemDau = request.DiemDau;
                checkTransport.DiemCuoi = request.DiemCuoi;
                checkTransport.LoaiVanDon = request.LoaiVanDon;
                checkTransport.HangTau = request.HangTau;
                checkTransport.Tau = request.TenTau;
                checkTransport.TongKhoiLuong = request.TongKhoiLuong;
                checkTransport.TongTheTich = request.TongTheTich;
                checkTransport.TongSoKien = request.TongSoKien;
                checkTransport.GhiChu = request.GhiChu;
                checkTransport.MaPtvc = request.MaPTVC;
                checkTransport.ThoiGianLayHang = request.ThoiGianLayHang;
                checkTransport.ThoiGianTraHang = request.ThoiGianTraHang;
                checkTransport.Updater = tempData.UserName;

                _context.VanDon.Update(checkTransport);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<PagedResponseCustom<ListTransport>> GetListTransport(ListFilter listFilter, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from transport in _context.VanDon
                           join
                           status in _context.StatusText
                           on
                           transport.TrangThai equals status.StatusId
                           join kh in _context.KhachHang
                           on
                           transport.MaKh equals kh.MaKh
                           where status.LangId == tempData.LangID
                           select new { transport, status, kh };

            var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
            listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.kh.MaKh));

            if (listFilter.customers.Count > 0)
            {
                listData = listData.Where(x => listFilter.customers.Contains(x.kh.MaKh));
            }

            if (listFilter.users.Count > 0)
            {
                listData = listData.Where(x => listFilter.users.Contains(x.transport.Creator));
            }

            if (!string.IsNullOrEmpty(filter.maptvc))
            {
                listData = listData.Where(x => x.transport.MaPtvc == filter.maptvc);
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.transport.MaVanDon.Contains(filter.Keyword) || x.transport.MaVanDonKh.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.customerId))
            {
                listData = listData.Where(x => x.transport.MaKh == filter.customerId);
            }

            if (!string.IsNullOrEmpty(filter.statusId))
            {
                listData = listData.Where(x => x.status.StatusId == int.Parse(filter.statusId));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.transport.ThoiGianTaoDon.Date >= filter.fromDate.Value.Date && x.transport.ThoiGianTaoDon.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await listData.CountAsync();

            var pagedData = await listData.OrderByDescending(x => x.transport.MaVanDon).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListTransport()
            {
                MaVanDonKH = x.transport.MaVanDonKh,
                MaPTVC = x.transport.MaPtvc,
                MaVanDon = x.transport.MaVanDon,
                MaKH = x.transport.MaKh,
                TenKH = x.kh.TenKh,
                LoaiVanDon = x.transport.LoaiVanDon,
                DiemLayHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.transport.DiemDau).Select(y => y.TenDiaDiem).FirstOrDefault(),
                DiemTraHang = _context.DiaDiem.Where(y => y.MaDiaDiem == x.transport.DiemCuoi).Select(y => y.TenDiaDiem).FirstOrDefault(),
                TongSoKien = x.transport.TongSoKien,
                TongKhoiLuong = x.transport.TongKhoiLuong,
                TongTheTich = x.transport.TongTheTich,
                ThoiGianLayHang = x.transport.ThoiGianLayHang,
                ThoiGianTaoDon = x.transport.ThoiGianTaoDon,
                ThoiGianTraHang = x.transport.ThoiGianTraHang,
                ThoiGianCoMat = x.transport.ThoiGianCoMat,
                ThoiGianLayRong = x.transport.ThoiGianLayRong,
                ThoiGianTraRong = x.transport.ThoiGianTraRong,
                ThoiGianHanLenh = x.transport.ThoiGianHanLenh,
                ThoiGianHaCang = x.transport.ThoiGianHaCang,
                HangTau = x.transport.HangTau,
                TongThungHang = x.transport.TongThungHang,
                MaTrangThai = x.status.StatusId,
                TrangThai = x.status.StatusContent,
            }).ToListAsync();

            return new PagedResponseCustom<ListTransport>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandling(string transportId, ListFilter listFilter, PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var listData = from vd in _context.VanDon
                           join
                           dp in _context.DieuPhoi
                           on vd.MaVanDon equals dp.MaVanDon
                           join tt in _context.StatusText
                           on dp.TrangThai equals tt.StatusId
                           where tt.LangId == tempData.LangID && vd.MaPtvc != "LTL" && vd.MaPtvc != "LCL"
                           select new { vd, dp, tt };

            var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
            listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.vd.MaKh));

            if (!string.IsNullOrEmpty(transportId))
            {
                listData = listData.Where(x => x.vd.MaVanDon == transportId);
            }

            if (listFilter.customers.Count() > 0)
            {
                listData = listData.Where(x => listFilter.customers.Contains(x.vd.MaKh));
            }

            if (listFilter.users.Count() > 0)
            {
                listData = listData.Where(x => listFilter.users.Contains(x.dp.Creator));
            }

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.dp.MaSoXe.Contains(filter.Keyword) || x.dp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword));
            }

            if (!string.IsNullOrEmpty(filter.statusId))
            {
                listData = listData.Where(x => x.dp.TrangThai == int.Parse(filter.statusId));
            }

            if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
            {
                listData = listData.Where(x => x.dp.CreatedTime.Date >= filter.fromDate.Value.Date && x.dp.CreatedTime.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await listData.CountAsync();

            var pagedData = await listData.OrderByDescending(x => x.vd.MaVanDon).ThenBy(x => x.dp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
            {
                DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                HangTau = x.vd.HangTau,
                MaVanDonKH = x.vd.MaVanDonKh,
                MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.dp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                MaPTVC = x.vd.MaPtvc,
                MaVanDon = x.dp.MaVanDon,
                PhanLoaiVanDon = x.vd.LoaiVanDon,
                MaDieuPhoi = x.dp.Id,
                DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                MaSoXe = x.dp.MaSoXe,
                PTVanChuyen = x.dp.MaLoaiPhuongTien,
                MaRomooc = x.dp.MaRomooc,
                ContNo = x.dp.ContNo,
                KhoiLuong = x.dp.KhoiLuong,
                SoKien = x.dp.SoKien,
                TheTich = x.dp.TheTich,
                TrangThai = x.tt.StatusContent,
                statusId = x.tt.StatusId,
                ThoiGianTaoDon = x.vd.ThoiGianTaoDon
            }).ToListAsync();

            int row = 0;
            foreach (var item in pagedData)
            {
                foreach (var item2 in pagedData.Where(x => x.MaVanDon == item.MaVanDon))
                {
                    row += 1;
                    item2.ContNum = row;
                }
                row = 0;
            }

            return new PagedResponseCustom<ListHandling>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandlingByTransportId(string transportId, PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from vd in _context.VanDon
                               join
                               dp in _context.DieuPhoi
                               on vd.MaVanDon equals dp.MaVanDon into vddpp
                               from vddp in vddpp.DefaultIfEmpty()
                               join tt in _context.StatusText
                               on vddp.TrangThai equals tt.StatusId into tt
                               from status in tt.DefaultIfEmpty()
                               join ttvd in _context.StatusText
                               on vd.TrangThai equals ttvd.StatusId
                               where (status.LangId == tempData.LangID || status.LangId == null)
                               && vd.MaVanDon == transportId
                               && ttvd.LangId == tempData.LangID
                               select new { vd, vddp, status, ttvd };

                var que = listData.ToQueryString();

                var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
                listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.vd.MaKh));

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.vddp.MaSoXe.Contains(filter.Keyword) || x.vddp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    listData = listData.Where(x => x.vd.TrangThai == int.Parse(filter.statusId));
                }

                var totalCount = await listData.CountAsync();

                var getPoint = from point in _context.DiaDiem select point;

                var pagedData = await listData.OrderByDescending(x => x.vd.MaVanDon).ThenBy(x => x.vddp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
                {
                    DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    HangTau = x.vd.HangTau,
                    MaVanDonKH = x.vd.MaVanDonKh,
                    MaPTVC = x.vd.MaPtvc,
                    PhanLoaiVanDon = x.vd.LoaiVanDon,
                    MaDieuPhoi = x.vddp.Id,
                    DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vddp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vddp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaSoXe = x.vddp.MaSoXe,
                    PTVanChuyen = x.vddp.MaLoaiPhuongTien,
                    ContNo = x.vddp.ContNo,
                    KhoiLuong = x.vddp.KhoiLuong,
                    SoKien = x.vddp.SoKien,
                    TheTich = x.vddp.TheTich,
                    TrangThai = string.IsNullOrEmpty(x.status.StatusContent) ? x.ttvd.StatusContent : x.status.StatusContent,
                    statusId = x.status.StatusId,
                    ThoiGianTaoDon = x.vd.ThoiGianTaoDon
                }).ToListAsync();

                return new PagedResponseCustom<ListHandling>()
                {
                    dataResponse = pagedData,
                    totalCount = totalCount,
                    paginationFilter = validFilter
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PagedResponseCustom<ListHandling>> GetListHandlingLess(ListFilter listFilter, PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from vd in _context.VanDon
                               join dp in _context.DieuPhoi
                               on vd.MaVanDon equals dp.MaVanDon into vddpp
                               from vddp in vddpp.DefaultIfEmpty()
                               join tt in _context.StatusText
                               on vddp.TrangThai equals tt.StatusId into ttdpp
                               from ttdp in ttdpp.DefaultIfEmpty()
                               join ttvd in _context.StatusText
                               on vd.TrangThai equals ttvd.StatusId
                               where (ttdp.LangId == tempData.LangID || ttdp.LangId == null)
                               && (vd.MaPtvc == "LTL" || vd.MaPtvc == "LCL")
                               && ttvd.LangId == tempData.LangID
                               select new { vd, vddp, ttdp, ttvd };

                var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
                listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.vd.MaKh));

                if (listFilter.customers.Count > 0)
                {
                    listData = listData.Where(x => listFilter.customers.Contains(x.vd.MaKh));
                }

                if (listFilter.users.Count > 0)
                {
                    listData = listData.Where(x => listFilter.users.Contains(x.vddp.Creator));
                }

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.vddp.MaSoXe.Contains(filter.Keyword) || x.vddp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    if (filter.statusId == "null")
                    {
                        listData = listData.Where(x => x.vddp.MaChuyen == null);
                    }
                    else
                    {
                        listData = listData.Where(x => x.vddp.TrangThai == int.Parse(filter.statusId));
                    }
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.vddp.CreatedTime.Date >= filter.fromDate.Value.Date && x.vddp.CreatedTime.Date <= filter.toDate.Value.Date);
                }

                var totalCount = await listData.CountAsync();

                var getPoint = from point in _context.DiaDiem select point;

                var pagedData = await listData.OrderByDescending(x => x.vd.CreatedTime).ThenByDescending(x => x.vddp.MaChuyen).ThenBy(x => x.vddp.Id).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
                {
                    MaChuyen = x.vddp.MaChuyen,
                    DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    HangTau = x.vd.HangTau,
                    MaVanDonKH = x.vd.MaVanDonKh,
                    MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                    DonViVanTai = _context.KhachHang.Where(y => y.MaKh == x.vddp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
                    DiemLayRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vddp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemTraRong = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vddp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    MaPTVC = x.vd.MaPtvc,
                    MaVanDon = x.vd.MaVanDon,
                    PhanLoaiVanDon = x.vd.LoaiVanDon,
                    MaDieuPhoi = x.vddp.Id,
                    MaSoXe = x.vddp.MaSoXe,
                    PTVanChuyen = x.vddp.MaLoaiPhuongTien,
                    MaRomooc = x.vddp.MaRomooc,
                    ContNo = x.vddp.ContNo,
                    KhoiLuong = x.vddp.KhoiLuong,
                    SoKien = x.vddp.SoKien,
                    TheTich = x.vddp.TheTich,
                    TrangThai = string.IsNullOrEmpty(x.ttdp.StatusContent) ? x.ttvd.StatusContent : x.ttdp.StatusContent,
                    statusId = x.ttdp.StatusId,
                    ThoiGianTaoDon = x.vd.ThoiGianTaoDon
                }).ToListAsync();

                return new PagedResponseCustom<ListHandling>()
                {
                    dataResponse = pagedData,
                    totalCount = totalCount,
                    paginationFilter = validFilter
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetHandling> GetHandlingById(int id)
        {
            try
            {
                var getHandling = from vd in _context.VanDon
                                  join
                                  dp in _context.DieuPhoi
                                  on vd.MaVanDon equals dp.MaVanDon
                                  where dp.Id == id
                                  select new { vd, dp };

                var data = await getHandling.FirstOrDefaultAsync();

                var RoadDetail = new RoadDetail()
                {
                    DiemLayHang = await _context.DiaDiem.Where(x => x.MaDiaDiem == data.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefaultAsync(),
                    DiemTraHang = await _context.DiaDiem.Where(x => x.MaDiaDiem == data.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefaultAsync(),
                    DiemLayRong = await _context.DiaDiem.Where(x => x.MaDiaDiem == data.dp.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefaultAsync(),
                    DiemTraRong = await _context.DiaDiem.Where(x => x.MaDiaDiem == data.dp.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefaultAsync()
                };

                return new GetHandling()
                {
                    CungDuong = RoadDetail,
                    PhanLoaiVanDon = data.vd.LoaiVanDon,
                    MaLoaiHangHoa = data.dp.MaLoaiHangHoa,
                    MaDVT = data.dp.MaDvt,
                    MaKh = data.vd.MaKh,
                    MaVanDon = data.vd.MaVanDon,
                    MaSoXe = data.dp.MaSoXe,
                    MaTaiXe = data.dp.MaTaiXe,
                    DonViVanTai = data.dp.DonViVanTai,
                    PTVanChuyen = data.dp.MaLoaiPhuongTien,
                    TenTau = data.vd.Tau,
                    HangTau = data.vd.HangTau == null ? null : _context.ShippingInfomation.Where(x => x.ShippingCode == data.vd.HangTau).Select(x => x.ShippingLineName).FirstOrDefault(),
                    MaRomooc = data.dp.MaRomooc,
                    DiemLayRong = data.dp.DiemLayRong,
                    DiemTraRong = data.dp.DiemTraRong,
                    ContNo = data.dp.ContNo,
                    SealNp = data.dp.SealNp,
                    SealHq = data.dp.SealHq,
                    TongKhoiLuong = data.vd.TongKhoiLuong,
                    TongTheTich = data.vd.TongTheTich,
                    TongSoKien = data.vd.TongSoKien,
                    KhoiLuong = data.dp.KhoiLuong,
                    TheTich = data.dp.TheTich,
                    SoKien = data.dp.SoKien,
                    GhiChu = data.dp.GhiChu,
                    GhiChuVanDon = data.vd.GhiChu,
                    ThoiGianLayRong = data.vd.ThoiGianLayRong,
                    ThoiGianTraRong = data.vd.ThoiGianTraRong,
                    ThoiGianHaCang = data.vd.ThoiGianHaCang,
                    ThoiGianHanLenh = data.vd.ThoiGianHanLenh,
                    ThoiGianCoMat = data.vd.ThoiGianCoMat,
                    ThoiGianLayHang = data.vd.ThoiGianLayHang,
                    ThoiGianTraHang = data.vd.ThoiGianTraHang,
                    ThoiGianLayRongThucTe = data.dp.ThoiGianLayRongThucTe,
                    ThoiGianTraRongThucTe = data.dp.ThoiGianTraRongThucTe,
                    ThoiGianCoMatThucTe = data.dp.ThoiGianCoMatThucTe,
                    ThoiGianLayHangThucTe = data.dp.ThoiGianLayHangThucTe,
                    ThoiGianTraHangThucTe = data.dp.ThoiGianTraHangThucTe,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> SetRunning(int id)
        {
            try
            {
                var handling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                string message = "";

                if (handling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                var getTransport = await _context.VanDon.Where(x => x.MaVanDon == handling.MaVanDon).FirstOrDefaultAsync();
                handling.Updater = tempData.UserName;

                switch (handling.TrangThai)
                {
                    case 35:
                        handling.TrangThai = 20;

                        if (getTransport.LoaiVanDon == "xuat")
                        {
                            handling.ThoiGianLayRongThucTe = DateTime.Now;
                        }
                        else
                        {
                            handling.ThoiGianTraRongThucTe = DateTime.Now;
                        }

                        handling.ThoiGianHoanThanh = DateTime.Now;

                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.Id + " hoan thanh chuyen");

                        _context.Update(handling);
                        message = "Tài Xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " và Xe: " + handling.MaSoXe + " đã hoàn thành chuyến";
                        break;

                    case 18:
                        //if (handling.ThoiGianTraHangThucTe == null)
                        //{
                        //    return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Trả Hàng thực tế " };
                        //}
                        //if (handling.ThoiGianLayHangThucTe == null)
                        //{
                        //    return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy Hàng thực tế " };
                        //}

                        //if (handling.MaLoaiPhuongTien.Contains("CONT"))
                        //{
                        //    if (handling.ThoiGianLayTraRongThucTe == null)
                        //    {
                        //        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật Thời Gian Lấy/Trả rỗng thực tế " };
                        //    }
                        //}

                        if (getTransport.MaPtvc == "FCL" || getTransport.MaPtvc == "FTL")
                        {
                            if (getTransport.LoaiVanDon == "nhap" && handling.MaLoaiPhuongTien.Contains("CONT"))
                            {
                                handling.TrangThai = 35;
                                handling.ThoiGianTraHangThucTe = DateTime.Now;
                                await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.Id + " di tra rong");
                                message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " đi trả rỗng";
                                _context.Update(handling);
                            }
                            else
                            {
                                handling.TrangThai = 20;
                                handling.ThoiGianTraHangThucTe = DateTime.Now;
                                handling.ThoiGianHoanThanh = DateTime.Now;
                                await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.Id + " hoan thanh chuyen");

                                _context.Update(handling);
                                message = "Tài Xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " và Xe: " + handling.MaSoXe + " đã hoàn thành chuyến";
                            }
                        }

                        if (getTransport.MaPtvc == "LCL" || getTransport.MaPtvc == "LTL")
                        {
                            handling.TrangThai = 36;
                            handling.ThoiGianTraHangThucTe = DateTime.Now;
                            message = "Tài Xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " và Xe: " + handling.MaSoXe + " đã giao hàng";
                            await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.Id + " da giao hang");
                            _context.Update(handling);
                        }
                        break;

                    case 17:
                        if (!handling.MaLoaiPhuongTien.Contains("CONT"))
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không phải xe container" };
                        }

                        if (string.IsNullOrEmpty(handling.ContNo))
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                        }

                        handling.ThoiGianLayHangThucTe = DateTime.Now;
                        handling.TrangThai = 18;
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handling.Id + " van chuyen hang");

                        _context.Update(handling);
                        message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + " vận chuyển hàng";
                        break;

                    case 27:
                        string status = "";

                        if (handling.MaLoaiPhuongTien.Contains("CONT") && getTransport.LoaiVanDon == "xuat")
                        {
                            handling.TrangThai = 17;
                            if (getTransport.LoaiVanDon == "xuat")
                            {
                                handling.ThoiGianLayRongThucTe = DateTime.Now;
                            }
                            else
                            {
                                handling.ThoiGianTraRongThucTe = DateTime.Now;
                            }
                            status = " đi lấy rỗng";
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(handling.ContNo))
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                            }

                            handling.TrangThai = 18;
                            handling.ThoiGianLayHangThucTe = DateTime.Now;
                            status = " vận chuyển hàng";
                        }

                        getTransport.TrangThai = 10;
                        getTransport.Updater = tempData.UserName;

                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handling.Id + status);

                        _context.Update(handling);
                        _context.Update(getTransport);
                        message = "Đã điều phối xe: " + handling.MaSoXe + " và tài xế: " + await _context.TaiXe.Where(x => x.MaTaiXe == handling.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefaultAsync() + status;
                        break;

                    default:
                        break;
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    if (handling.TrangThai == 36)
                    {
                        var checkStatusHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == handling.MaChuyen).ToListAsync();
                        if (checkStatusHandling.Count == checkStatusHandling.Where(x => x.TrangThai == 36).Count())
                        {
                            if (getTransport.LoaiVanDon == "nhap" && handling.MaLoaiPhuongTien.Contains("CONT"))
                            {
                                checkStatusHandling.ForEach(x =>
                                {
                                    x.TrangThai = 35;
                                    x.ThoiGianTraRongThucTe = DateTime.Now;
                                    x.Updater = tempData.UserName;
                                });
                            }
                            else
                            {
                                await HandleVehicleStatus(20, handling.MaSoXe);

                                checkStatusHandling.ForEach(x =>
                                {
                                    x.TrangThai = 20;
                                    x.ThoiGianHoanThanh = DateTime.Now;
                                    x.Updater = tempData.UserName;
                                });

                                var getListTransport = await _context.VanDon.Where(x => checkStatusHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();
                                getListTransport.ForEach(x =>
                                {
                                    x.TrangThai = 22;
                                    x.Updater = tempData.UserName;
                                    getTransport.ThoiGianHoanThanh = DateTime.Now;
                                });
                                _context.UpdateRange(getListTransport);
                            }

                            _context.UpdateRange(checkStatusHandling);

                            var save = await _context.SaveChangesAsync();

                            if (save > 0)
                            {
                                return new BoolActionResult { isSuccess = true, Message = "Vận đơn đã hoàn thành" };
                            }
                            else
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Lỗi! không được thực thi" };
                            }
                        }
                    }

                    if (handling.TrangThai == 20)
                    {
                        await HandleVehicleStatus(20, handling.MaSoXe);

                        var checkAllHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == handling.MaVanDon).ToListAsync();
                        if (checkAllHandling.Where(x => x.TrangThai == 20 || x.TrangThai == 21).Count() == checkAllHandling.Count())
                        {
                            getTransport.TrangThai = 22;
                            getTransport.Updater = tempData.UserName;
                            getTransport.ThoiGianHoanThanh = DateTime.Now;

                            _context.Update(getTransport);

                            var updateTransport = await _context.SaveChangesAsync();
                            if (updateTransport > 0)
                            {
                                await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set " + handling.MaVanDon + " completed");
                                return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + handling.MaVanDon + " đã hoàn thành" };
                            }
                            else
                            {
                                return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                            }
                        }
                    }
                    else
                    {
                        await HandleVehicleStatus(handling.TrangThai, handling.MaSoXe);
                    }

                    return new BoolActionResult { isSuccess = true, Message = message };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lỗi! không được thực thi" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + "  SetRuning with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> SetRunningLess(string handlingId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == handlingId && x.TrangThai != 20 && x.TrangThai != 21 && x.TrangThai != 31).ToListAsync();
                var getlistTransports = await _context.VanDon.Where(x => getListHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon) && x.TrangThai != 11 && x.TrangThai != 22 && x.TrangThai != 29).ToListAsync();

                if (getlistTransports.Select(x => x.MaPtvc).FirstOrDefault() == "LCL" && getListHandling.Select(x => x.TrangThai).FirstOrDefault() == 17)
                {
                    if (string.IsNullOrEmpty(getListHandling.Select(x => x.ContNo).FirstOrDefault()))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Cập Nhật ContNo trước đã" };
                    }

                    getListHandling.ForEach(x =>
                    {
                        x.Updater = tempData.UserName;
                        x.TrangThai = 18;
                        x.ThoiGianLayHangThucTe = DateTime.Now;
                    });

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        var handleVehicleStatus = await HandleVehicleStatus(18, getListHandling.Select(x => x.MaSoXe).FirstOrDefault());
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handlingId + "Shipping");
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe vận chuyển hàng" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (getListHandling.Select(x => x.TrangThai).FirstOrDefault() == 35)
                {
                    getListHandling.ForEach(x =>
                    {
                        x.Updater = tempData.UserName;
                        x.TrangThai = 20;
                        x.ThoiGianHoanThanh = DateTime.Now;
                    });
                    getlistTransports.ForEach(x =>
                    {
                        x.Updater = tempData.UserName;
                        x.TrangThai = 22;
                        x.ThoiGianHoanThanh = DateTime.Now;
                    });

                    _context.VanDon.UpdateRange(getlistTransports);
                    _context.DieuPhoi.UpdateRange(getListHandling);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        var handleVehicleStatus = await HandleVehicleStatus(20, getListHandling.Select(x => x.MaSoXe).FirstOrDefault());
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handlingId + " Completed");
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã Hoàn Thành Vận Đơn" };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }

                if (getListHandling.Select(x => x.TrangThai).FirstOrDefault() == 27)
                {
                    string status = "";

                    if (getlistTransports.Select(x => x.LoaiVanDon).FirstOrDefault() == "xuat" && getlistTransports.Select(x => x.MaPtvc).FirstOrDefault() == "LCL")
                    {
                        getListHandling.ForEach(x =>
                        {
                            x.Updater = tempData.UserName;
                            x.TrangThai = 17;
                            x.ThoiGianLayRongThucTe = DateTime.Now;
                        });
                        status = " đi lấy rỗng";
                        var handleVehicleStatus = await HandleVehicleStatus(17, getListHandling.Select(x => x.MaSoXe).FirstOrDefault());
                    }
                    else
                    {
                        getListHandling.ForEach(x =>
                        {
                            x.Updater = tempData.UserName;
                            x.TrangThai = 18;
                            x.ThoiGianLayHangThucTe = DateTime.Now;
                        });
                        status = " vận chuyển hàng";
                        var handleVehicleStatus = await HandleVehicleStatus(18, getListHandling.Select(x => x.MaSoXe).FirstOrDefault());
                    }

                    getlistTransports.ForEach(x =>
                    {
                        x.Updater = tempData.UserName;
                        x.TrangThai = 10;
                    });

                    _context.DieuPhoi.UpdateRange(getListHandling);
                    _context.VanDon.UpdateRange(getlistTransports);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("BillOfLading", " UserId: " + tempData.UserName + "Set " + handlingId + status);
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã điều phối xe " + status };
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                    }
                }
                return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi " };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandling(int id)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getByid = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (getByid == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lệnh điều phối không tồn tại" };
                }

                if (getByid.TrangThai != 19 && getByid.TrangThai != 27)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy lệnh điều phối này" };
                }

                getByid.Updater = tempData.UserName;
                getByid.TrangThai = 21;
                _context.Update(getByid);

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    var getAllHandlingOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == getByid.MaVanDon).ToListAsync();

                    if (getAllHandlingOfTransport.Where(x => x.TrangThai == 21).Count() == getAllHandlingOfTransport.Count())
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getByid.MaVanDon).FirstOrDefaultAsync();

                        getTransport.TrangThai = 11;
                        getTransport.Updater = tempData.UserName;
                        _context.Update(getTransport);

                        var result1 = await _context.SaveChangesAsync();
                        if (result1 > 0)
                        {
                            await transaction.CommitAsync();
                            return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối và hủy vận đơn thành công" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh thất bại" };
                        }
                    }

                    if (getAllHandlingOfTransport.Where(x => x.TrangThai == 20 || x.TrangThai == 21).Count() == getAllHandlingOfTransport.Count())
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getByid.MaVanDon).FirstOrDefaultAsync();
                        getTransport.TrangThai = 22;
                        getTransport.ThoiGianHoanThanh = DateTime.Now;
                        getTransport.Updater = tempData.UserName;
                        _context.Update(getTransport);

                        var result3 = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            await transaction.CommitAsync();
                            await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " Set TransportId: " + getTransport.MaVanDon + " completed");
                            return new BoolActionResult { isSuccess = true, Message = "Mã vận đơn " + getTransport.MaVanDon + " đã hoàn thành" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Điều phối xe thất bại" };
                        }
                    }

                    await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " set  Cancel handling " + id);
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + "  CancelHandling with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandlingByCus(int? id, string transportId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (checkTransport.TrangThai != 28)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy chuyến của vận đơn này nữa" };
                }

                if (checkTransport.MaPtvc == "FCL" || checkTransport.MaPtvc == "FTL")
                {
                    var checkHandling = await _context.DieuPhoi.Where(x => x.Id == id && x.MaVanDon == transportId).FirstOrDefaultAsync();
                    if (checkHandling == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Điều Phối không tồn tại" };
                    }

                    checkHandling.TrangThai = 21;
                    _context.DieuPhoi.Update(checkHandling);

                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transportId).ToListAsync();
                        if (getListHandling.Where(x => x.TrangThai == 21).Count() == getListHandling.Count)
                        {
                            checkTransport.TrangThai = 11;
                            _context.VanDon.Update(checkTransport);

                            var result1 = await _context.SaveChangesAsync();
                            if (result1 > 0)
                            {
                                await transaction.CommitAsync();
                                return new BoolActionResult { isSuccess = true, Message = "Đã hủy chuyến và vận đơn thành công" };
                            }
                            else
                            {
                                await transaction.RollbackAsync();
                                return new BoolActionResult { isSuccess = false, Message = "Đã hủy chuyến và vận đơn thất bại" };
                            }
                        }
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã hủy chuyến thành công" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Hủy chuyến thất bại" };
                    }
                }

                if (checkTransport.MaPtvc == "LTL" || checkTransport.MaPtvc == "LCL")
                {
                    checkTransport.TrangThai = 11;
                    _context.Update(checkTransport);

                    var result = await _context.SaveChangesAsync();
                    if (result > 0)
                    {
                        await transaction.CommitAsync();
                        return new BoolActionResult { isSuccess = true, Message = "Đã hủy vận đơn thành công" };
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return new BoolActionResult { isSuccess = false, Message = "Hủy chuyến thất bại" };
                    }
                }

                return new BoolActionResult { isSuccess = false, Message = "Hủy chuyến thất bại" };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> AcceptOrRejectTransport(string transportId, int action)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkTransport = await _context.VanDon.Where(x => x.MaVanDon == transportId).FirstOrDefaultAsync();

                if (checkTransport == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vận đơn không tồn tại" };
                }

                if (action == 0)
                {
                    checkTransport.TrangThai = 8;

                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transportId).ToListAsync();
                    if (getListHandling.Count > 0)
                    {
                        getListHandling.ForEach(x => { x.TrangThai = 19; });
                        _context.DieuPhoi.UpdateRange(getListHandling);
                    }
                }

                if (action == 1)
                {
                    checkTransport.TrangThai = 29;

                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == transportId).ToListAsync();
                    if (getListHandling.Count > 0)
                    {
                        getListHandling.ForEach(x => { x.TrangThai = 31; });
                        _context.DieuPhoi.UpdateRange(getListHandling);
                    }
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = action == 0 ? "Đã nhận đơn hàng thành công!" : "Đã từ chối đơn hàng thành công!" };
                }
                else
                {
                    await transaction.RollbackAsync();
                    return new BoolActionResult { isSuccess = false, Message = action == 0 ? "Đã nhận đơn hàng thất bại!" : "Đã từ chối đơn hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandlingLess(string handlingId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var listHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == handlingId).ToListAsync();
                var listHandlingCancel = new List<long>();

                foreach (var item in listHandling)
                {
                    if (item.TrangThai == 19 || item.TrangThai == 27)
                    {
                        listHandlingCancel.Add(item.Id);
                    }
                }

                if (listHandlingCancel.Count == 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy lệnh điều phối này" };
                }

                listHandling = listHandling.Where(x => listHandlingCancel.Contains(x.Id)).ToList();

                listHandling.ForEach(x =>
                {
                    x.Updater = tempData.UserName;
                    x.TrangThai = 21;
                    x.UpdatedTime = DateTime.Now;
                });

                var listTransports = await _context.VanDon.Where(x => listHandling.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();
                listTransports.ForEach(x =>
                {
                    x.Updater = tempData.UserName;
                    x.TrangThai = 11;
                    x.UpdatedTime = DateTime.Now;
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy lệnh điều phối và hủy vận đơn thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy lệnh điều phối và hủy vận đơn thất bại" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new BoolActionResult() { isSuccess = false, Message = ex.ToString() };
            }
        }

        //public async Task<BoolActionResult> CloneHandling(int id)
        //{
        //    var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var checkHangling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

        //        if (checkHangling.TrangThai != 19)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Không thể copy dữ liệu điều phối này" };
        //        }

        //        await _context.DieuPhoi.AddAsync(new DieuPhoi()
        //        {
        //            MaVanDon = checkHangling.MaVanDon,
        //            TrangThai = checkHangling.TrangThai,
        //            CreatedTime = DateTime.Now,
        //        });

        //        var result = await _context.SaveChangesAsync();

        //        if (result > 0)
        //        {
        //            await transaction.CommitAsync();
        //            return new BoolActionResult { isSuccess = true, Message = "Copy dữ liệu điều phối thành công!" };
        //        }
        //        else
        //        {
        //            await transaction.RollbackAsync();
        //            return new BoolActionResult { isSuccess = false, Message = "Copy dữ liệu điều phối thất bại!" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
        //    }
        //}

        //public async Task<BoolActionResult> RemoveHandling(int id)
        //{
        //    var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var checkHangling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();

        //        if (checkHangling.TrangThai != 19)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Không thể xóa điều phối này" };
        //        }

        //        var checkCount = await _context.DieuPhoi.Where(x => x.MaVanDon == checkHangling.MaVanDon).ToListAsync();
        //        if (checkCount.Count <= 1)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Không thể xóa điều phối này" };
        //        }

        //        _context.DieuPhoi.Remove(checkHangling);

        //        var result = await _context.SaveChangesAsync();

        //        if (result > 0)
        //        {
        //            await transaction.CommitAsync();
        //            return new BoolActionResult { isSuccess = true, Message = "Xóa dữ liệu điều phối thành công!" };
        //        }
        //        else
        //        {
        //            await transaction.RollbackAsync();
        //            return new BoolActionResult { isSuccess = false, Message = "Xóa dữ liệu điều phối thất bại!" };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
        //    }
        //}

        public async Task<List<Attachment>> GetListImageByHandlingId(int handlingId)
        {
            var list = await _context.Attachment.Where(x => x.DieuPhoiId == handlingId).ToListAsync();
            return list.Select(x => new Attachment()
            {
                Id = x.Id,
                FileName = x.FileName.Replace(x.FileType, ""),
                FileType = x.FileType,
                // FilePath =Path.Combine(_common.GetFile(x.FilePath)),
                UploadedTime = x.UploadedTime
            }).ToList();
        }

        public async Task<BoolActionResult> ChangeImageName(int id, string newName)
        {
            try
            {
                var getById = await _context.Attachment.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (getById == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "File này không tồn tại" };
                }

                if (string.IsNullOrEmpty(newName) || newName.Length < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được rỗng" };
                }

                if (!Regex.IsMatch(newName, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được để dấu và có ký tự đặc biệt" };
                }

                getById.FileName = newName;
                _context.Update(getById);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Đổi tên thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đã có lỗi xảy ra" };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Attachment> GetImageById(int id)
        {
            var image = await _context.Attachment.Where(x => x.Id == id).FirstOrDefaultAsync();

            var name = image.FileName.Replace(image.FileType, "");

            return new Attachment
            {
                FilePath = Path.Combine(_common.GetFile(image.FilePath)),
                FileName = image.FileName.Replace(image.FileType.Trim(), ""),
                FileType = image.FileType
            };
        }

        public async Task<BoolActionResult> DeleteImageById(int imageId)
        {
            try
            {
                var image = await _context.Attachment.Where(x => x.Id == imageId).FirstOrDefaultAsync();

                if (image == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hình ảnh không tồn tại" };
                }

                await _common.DeleteFileAsync(image.FileName, image.FilePath);

                _context.Attachment.Remove(image);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Xóa hình ảnh thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xóa hình ảnh thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> UploadFile(UploadImagesHandling request)
        {
            var PathFolder = $"Transport/{request.transportId}/{request.handlingId}";

            if (request.files.Files.Count < 1)
            {
                return new BoolActionResult { isSuccess = false, Message = "Không có file nào" };
            }

            var checkTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == request.transportId && x.Id == request.handlingId).FirstOrDefaultAsync();

            if (checkTransport == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "Điều phối không tồn tại trong vận đơn" };
            }

            foreach (var fileItem in request.files.Files)
            {
                var originalFileName = ContentDispositionHeaderValue.Parse(fileItem.ContentDisposition).FileName.Trim('"');
                var supportedTypes = new[] { "jpg", "jpeg", "png" };
                var fileExt = Path.GetExtension(originalFileName).Substring(1).ToLower();
                if (!supportedTypes.Contains(fileExt))
                {
                    return new BoolActionResult { isSuccess = false, Message = "File không được hỗ trợ" };
                }

                var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), Guid.NewGuid().ToString());
                var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";

                var attachment = new Attachment()
                {
                    FileName = fileName,
                    FilePath = _common.GetFileUrl(fileName, PathFolder),
                    FileSize = fileItem.Length,
                    FileType = Path.GetExtension(fileName),
                    FolderName = "Transport",
                    DieuPhoiId = request.handlingId,
                    UploadedTime = DateTime.Now,
                    Creator = tempData.UserName,
                };

                var add = await _common.AddAttachment(attachment);

                if (add.isSuccess == false)
                {
                    return new BoolActionResult { isSuccess = false, Message = add.Message };
                }
                await _common.SaveFileAsync(fileItem.OpenReadStream(), fileName, PathFolder);
            }

            await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " upload file with data: " + JsonSerializer.Serialize(request));
            return new BoolActionResult { isSuccess = true, Message = "Upload hình ảnh thành công" };
        }

        //public async Task<BoolActionResult> ChangeStatusDoc(int idHandling, int type)
        //{
        //    //Type 1 = Confirm Full Doc
        //    //Type 2 = Lock/Open Doc

        //    try
        //    {
        //        string messages = "";
        //        var checkHandling = await _context.DieuPhoi.Where(x => x.Id == idHandling).FirstOrDefaultAsync();

        //        if (type == 1)
        //        {
        //            checkHandling.TrangThaiChungTu = checkHandling.TrangThaiChungTu == 0 ? 1 : 0;

        //            if(checkHandling.TrangThaiChungTu == 0)
        //            {

        //            }

        //            if(checkHandling.TrangThaiChungTu == 0)
        //            {

        //            }
        //            messages = "";
        //        }

        //        if (type == 2)
        //        {

        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //}

        private async Task<GetPriceListRequest> GetPriceTable(string MaKH, int firstPlace, int secondPlace, int? emptyPlace, string MaDVT, string LoaiHangHoa, string LoaiPhuongTien, string MaPTVC)
        {
            var checkPriceTable = from bg in _context.BangGia
                                  join hd in _context.HopDongVaPhuLuc
                                  on bg.MaHopDong equals hd.MaHopDong
                                  where hd.MaKh == MaKH
                                  && bg.TrangThai == 4
                                  && bg.NgayApDung.Date <= DateTime.Now.Date
                                  && (bg.NgayHetHieuLuc.Value.Date > DateTime.Now.Date || bg.NgayHetHieuLuc == null)
                                  && bg.MaDvt == MaDVT
                                  && bg.MaLoaiHangHoa == LoaiHangHoa
                                  && bg.MaLoaiPhuongTien == LoaiPhuongTien
                                  && bg.MaPtvc == MaPTVC
                                  select bg;

            var getFirstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == firstPlace).FirstOrDefaultAsync();
            var getSecondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == secondPlace).FirstOrDefaultAsync();
            var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == emptyPlace).FirstOrDefaultAsync();

            checkPriceTable = checkPriceTable.Where(x =>
            (x.DiemDau == getFirstPlace.MaDiaDiem || x.DiemDau == getFirstPlace.DiaDiemCha)
            && (x.DiemCuoi == getSecondPlace.MaDiaDiem || x.DiemCuoi == getSecondPlace.DiaDiemCha)
            && (x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem) || x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)));

            var quey = checkPriceTable.ToQueryString();

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem));

                if (checkPriceTable.Count() == 1)
                {
                    return await checkPriceTable.Select(x => new GetPriceListRequest()
                    {
                        ID = x.Id,
                        DonGia = x.DonGia,
                        MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                        MaLoaiHangHoa = x.MaLoaiHangHoa,
                        MaLoaiDoiTac = x.MaLoaiDoiTac,
                        MaDVT = x.MaDvt,
                        MaPTVC = x.MaPtvc
                    }).FirstOrDefaultAsync();
                }
                else
                {
                    return null;
                }
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));

                if (checkPriceTable.Count() == 1)
                {
                    return await checkPriceTable.Select(x => new GetPriceListRequest()
                    {
                        ID = x.Id,
                        DonGia = x.DonGia,
                        MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                        MaLoaiHangHoa = x.MaLoaiHangHoa,
                        MaLoaiDoiTac = x.MaLoaiDoiTac,
                        MaDVT = x.MaDvt,
                        MaPTVC = x.MaPtvc
                    }).FirstOrDefaultAsync();
                }
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                checkPriceTable = checkPriceTable.Where(x => x.DiemDau == getFirstPlace.DiaDiemCha && x.DiemCuoi == getSecondPlace.DiaDiemCha && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));
            }

            if (checkPriceTable.Count() == 1)
            {
                return await checkPriceTable.Select(x => new GetPriceListRequest()
                {
                    ID = x.Id,
                    DonGia = x.DonGia,
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    MaLoaiHangHoa = x.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.MaLoaiDoiTac,
                    MaDVT = x.MaDvt,
                    MaPTVC = x.MaPtvc
                }).FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        private async Task<BoolActionResult> HandleVehicleStatus(int handlingStatus, string newVehicleId = null, string oldVehicleId = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(newVehicleId))
                {
                    var getNewVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == newVehicleId).FirstOrDefaultAsync();
                    switch (handlingStatus)
                    {
                        case 17:
                            getNewVehicle.TrangThai = 34;
                            break;

                        case 18:
                            getNewVehicle.TrangThai = 34;
                            break;

                        case 19:
                            getNewVehicle.TrangThai = 33;
                            break;

                        case 27:
                            getNewVehicle.TrangThai = 33;
                            break;

                        case 35:
                            getNewVehicle.TrangThai = 34;
                            break;

                        case 36:
                            getNewVehicle.TrangThai = 34;
                            break;

                        default:
                            getNewVehicle.TrangThai = 32;
                            break;
                    }
                    _context.Update(getNewVehicle);
                }

                if (newVehicleId != oldVehicleId)
                {
                    if (!string.IsNullOrEmpty(oldVehicleId))
                    {
                        var getOldVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == oldVehicleId).FirstOrDefaultAsync();
                        getOldVehicle.TrangThai = 32;
                        _context.Update(getOldVehicle);
                    }
                }

                var result = await _context.SaveChangesAsync();

                return new BoolActionResult { isSuccess = true };
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        private async Task<BoolActionResult> CheckDriverStats(string driverId)
        {
            try
            {
                //var listStatusPending = new List<int> { 19, 20, 21, 30, 31 };

                //var check = await _context.DieuPhoi.Where(x => x.MaTaiXe == driverId && !listStatusPending.Contains(x.TrangThai)).ToListAsync();

                //if (check.Count > 0)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Tài xế đang bận, không thể điều chuyến" };
                //}

                return new BoolActionResult { isSuccess = true };
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}