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
using TBSLogistics.Model.Model.FileModel;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Model.Model.PriceListModel;
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

                var list = new List<CreateTransport>();

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream, cancellationToken);

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        var rowCount = worksheet.Dimension.Rows;

                        if (rowCount == 0)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
                        }

                        if (worksheet.Cells[1, 1].Value.ToString().Trim() != "LoaiVanDon" ||
                            worksheet.Cells[1, 2].Value.ToString().Trim() != "MaPTVC" ||
                            worksheet.Cells[1, 3].Value.ToString().Trim() != "MaKH" ||
                            worksheet.Cells[1, 4].Value.ToString().Trim() != "Account" ||
                            worksheet.Cells[1, 5].Value.ToString().Trim() != "MaVanDonKH" ||
                            worksheet.Cells[1, 6].Value.ToString().Trim() != "HangTau" ||
                            worksheet.Cells[1, 7].Value.ToString().Trim() != "Tau" ||
                            worksheet.Cells[1, 8].Value.ToString().Trim() != "DiemDongHang" ||
                            worksheet.Cells[1, 9].Value.ToString().Trim() != "DiemHaHang" ||
                            worksheet.Cells[1, 10].Value.ToString().Trim() != "TongKhoiLuong" ||
                            worksheet.Cells[1, 11].Value.ToString().Trim() != "TongTheTich" ||
                            worksheet.Cells[1, 12].Value.ToString().Trim() != "TongSoKien" ||
                            worksheet.Cells[1, 13].Value.ToString().Trim() != "MaLoaiHangHoa" ||
                            worksheet.Cells[1, 14].Value.ToString().Trim() != "MaLoaiPhuongTien" ||
                            worksheet.Cells[1, 15].Value.ToString().Trim() != "DiemTraRong" ||
                            worksheet.Cells[1, 16].Value.ToString().Trim() != "DiemLayRong" ||
                            worksheet.Cells[1, 17].Value.ToString().Trim() != "KhoiLuong" ||
                            worksheet.Cells[1, 18].Value.ToString().Trim() != "TheTich" ||
                            worksheet.Cells[1, 19].Value.ToString().Trim() != "SoKien" ||
                            worksheet.Cells[1, 20].Value.ToString().Trim() != "ThoiGianLayTraRong" ||
                            worksheet.Cells[1, 21].Value.ToString().Trim() != "ThoiGianHanLenh/ThoiGianHaCang" ||
                            worksheet.Cells[1, 22].Value.ToString().Trim() != "ThoiGianLayHang" ||
                            worksheet.Cells[1, 23].Value.ToString().Trim() != "ThoiGianTraHang" ||
                            worksheet.Cells[1, 24].Value.ToString().Trim() != "GhiChu" ||
                            worksheet.Cells[1, 25].Value.ToString().Trim() != "DonViVanTai" ||
                            worksheet.Cells[1, 26].Value.ToString().Trim() != "MaSoXe" ||
                            worksheet.Cells[1, 27].Value.ToString().Trim() != "MaTaiXe" ||
                            worksheet.Cells[1, 28].Value.ToString().Trim() != "Cont_No" ||
                            worksheet.Cells[1, 29].Value.ToString().Trim() != "SEAL_NP" ||
                            worksheet.Cells[1, 30].Value.ToString().Trim() != "SEAL_HQ" ||
                            worksheet.Cells[1, 31].Value.ToString().Trim() != "GhiChuDP"
                            )
                        {
                            return new BoolActionResult { isSuccess = false, Message = "File excel không đúng định dạng chuẩn" };
                        }

                        for (int row = 3; row <= rowCount; row++)
                        {
                            ErrorRow = row;
                            string LoaiVanDon = worksheet.Cells[row, 1].Value == null ? null : worksheet.Cells[row, 1].Value.ToString().Trim();
                            string MaPTVC = worksheet.Cells[row, 2].Value == null ? null : worksheet.Cells[row, 2].Value.ToString().Trim();
                            string MaKH = worksheet.Cells[row, 3].Value == null ? null : worksheet.Cells[row, 3].Value.ToString().Trim();
                            string Account = worksheet.Cells[row, 4].Value == null ? null : worksheet.Cells[row, 4].Value.ToString().Trim();
                            string MaVanDonKH = worksheet.Cells[row, 5].Value == null ? null : worksheet.Cells[row, 5].Value.ToString().Trim();
                            string HangTau = worksheet.Cells[row, 6].Value == null ? null : worksheet.Cells[row, 6].Value.ToString().Trim();
                            string Tau = worksheet.Cells[row, 7].Value == null ? null : worksheet.Cells[row, 7].Value.ToString().Trim();
                            string DiemDau = worksheet.Cells[row, 8].Value == null ? null : worksheet.Cells[row, 8].Value.ToString().Trim();
                            string DiemCuoi = worksheet.Cells[row, 9].Value == null ? null : worksheet.Cells[row, 9].Value.ToString().Trim();
                            string TongKhoiLuong = worksheet.Cells[row, 10].Value == null ? null : worksheet.Cells[row, 10].Value.ToString().Trim();
                            string TongTheTich = worksheet.Cells[row, 11].Value == null ? null : worksheet.Cells[row, 11].Value.ToString().Trim();
                            string TongSoKien = worksheet.Cells[row, 12].Value == null ? null : worksheet.Cells[row, 12].Value.ToString().Trim();
                            string MaLoaiHangHoa = worksheet.Cells[row, 13].Value == null ? null : worksheet.Cells[row, 13].Value.ToString().Trim();
                            string MaLoaiPhuongTien = worksheet.Cells[row, 14].Value == null ? null : worksheet.Cells[row, 14].Value.ToString().Trim();
                            string DiemTraRong = worksheet.Cells[row, 15].Value == null ? null : worksheet.Cells[row, 15].Value.ToString().Trim();
                            string DiemLayRong = worksheet.Cells[row, 16].Value == null ? null : worksheet.Cells[row, 16].Value.ToString().Trim();
                            string KhoiLuong = worksheet.Cells[row, 17].Value == null ? null : worksheet.Cells[row, 17].Value.ToString().Trim();
                            string TheTich = worksheet.Cells[row, 18].Value == null ? null : worksheet.Cells[row, 18].Value.ToString().Trim();
                            string SoKien = worksheet.Cells[row, 19].Value == null ? null : worksheet.Cells[row, 19].Value.ToString().Trim();
                            string ThoiGianLayTraRong = worksheet.Cells[row, 20].Value == null ? null : worksheet.Cells[row, 20].Value.ToString().Trim();
                            string ThoiGianHanLenhOrThoiGianHaCang = worksheet.Cells[row, 21].Value == null ? null : worksheet.Cells[row, 21].Value.ToString().Trim();
                            string ThoiGianLayHang = worksheet.Cells[row, 22].Value == null ? null : worksheet.Cells[row, 22].Value.ToString().Trim();
                            string ThoiGianTraHang = worksheet.Cells[row, 23].Value == null ? null : worksheet.Cells[row, 23].Value.ToString().Trim();
                            string GhiChu = worksheet.Cells[row, 24].Value == null ? null : worksheet.Cells[row, 24].Value.ToString().Trim();
                            string DonViVanTai = worksheet.Cells[row, 25].Value == null ? null : worksheet.Cells[row, 25].Value.ToString().Trim();
                            string MaSoXe = worksheet.Cells[row, 26].Value == null ? null : worksheet.Cells[row, 26].Value.ToString().Trim();
                            string MaTaiXe = worksheet.Cells[row, 27].Value == null ? null : worksheet.Cells[row, 27].Value.ToString().Trim();
                            string Cont_No = worksheet.Cells[row, 28].Value == null ? null : worksheet.Cells[row, 28].Value.ToString().Trim();
                            string SEAL_NP = worksheet.Cells[row, 29].Value == null ? null : worksheet.Cells[row, 29].Value.ToString().Trim();
                            string SEAL_HQ = worksheet.Cells[row, 30].Value == null ? null : worksheet.Cells[row, 30].Value.ToString().Trim();
                            string GhiChuDP = worksheet.Cells[row, 31].Value == null ? null : worksheet.Cells[row, 31].Value.ToString().Trim();

                            if (!listBooking.Contains(MaVanDonKH))
                            {
                                listBooking.Add(MaVanDonKH);

                                list.Add(new CreateTransport()
                                {
                                    LoaiVanDon = LoaiVanDon,
                                    MaKH = MaKH,
                                    AccountId = Account,
                                    MaVanDonKH = MaVanDonKH,
                                    HangTau = HangTau,
                                    TenTau = Tau,
                                    DiemCuoi = int.Parse(DiemCuoi),
                                    DiemDau = int.Parse(DiemDau),
                                    TongKhoiLuong = string.IsNullOrEmpty(TongKhoiLuong) ? null : double.Parse(TongKhoiLuong),
                                    TongTheTich = string.IsNullOrEmpty(TongTheTich) ? null : double.Parse(TongTheTich),
                                    TongSoKien = string.IsNullOrEmpty(TongSoKien) ? null : double.Parse(TongSoKien),
                                    GhiChu = GhiChu,
                                    MaPTVC = MaPTVC,
                                    ThoiGianLayHang = string.IsNullOrEmpty(ThoiGianLayHang) ? null : DateTime.Parse(ThoiGianLayHang),
                                    ThoiGianTraHang = string.IsNullOrEmpty(ThoiGianTraHang) ? null : DateTime.Parse(ThoiGianTraHang),
                                    ThoiGianLayRong = string.IsNullOrEmpty(ThoiGianLayTraRong) ? null : DateTime.Parse(ThoiGianLayTraRong),
                                    ThoiGianTraRong = string.IsNullOrEmpty(ThoiGianLayTraRong) ? null : DateTime.Parse(ThoiGianLayTraRong),
                                    ThoiGianHaCang = LoaiVanDon == "xuat" ? string.IsNullOrEmpty(ThoiGianHanLenhOrThoiGianHaCang) ? null : DateTime.Parse(ThoiGianHanLenhOrThoiGianHaCang) : null,
                                    ThoiGianHanLenh = LoaiVanDon == "nhap" ? string.IsNullOrEmpty(ThoiGianHanLenhOrThoiGianHaCang) ? null : DateTime.Parse(ThoiGianHanLenhOrThoiGianHaCang) : null,
                                    arrHandlings = new List<arrHandling>()
                                    {
                                       new arrHandling()
                                       {
                                        GhiChu = GhiChuDP,
                                        ContNo = string.IsNullOrEmpty(Cont_No)? null:Cont_No,
                                        DonViVanTai =  string.IsNullOrEmpty(DonViVanTai)? null:DonViVanTai,
                                        PTVanChuyen = MaLoaiPhuongTien ,
                                        LoaiHangHoa = MaLoaiHangHoa ,
                                        DonViTinh = "CHUYEN",
                                        DiemLayRong = string.IsNullOrEmpty(DiemLayRong)?null: int.Parse(DiemLayRong),
                                        DiemTraRong = string.IsNullOrEmpty(DiemTraRong)?null: int.Parse(DiemTraRong),
                                        KhoiLuong =  string.IsNullOrEmpty(KhoiLuong)?null: double.Parse(KhoiLuong) ,
                                        TheTich =  string.IsNullOrEmpty(TheTich)?null: double.Parse(TheTich) ,
                                        SoKien =  string.IsNullOrEmpty(SoKien)?null: double.Parse(SoKien) ,
                                       },
                                    }
                                });
                            }
                            else
                            {
                                list = list.Where(x => x.MaVanDonKH == MaVanDonKH).ToList();

                                foreach (var item in list)
                                {
                                    item.arrHandlings.Add(new arrHandling
                                    {
                                        GhiChu = GhiChuDP,
                                        ContNo = string.IsNullOrEmpty(Cont_No) ? null : Cont_No,
                                        DonViVanTai = string.IsNullOrEmpty(DonViVanTai) ? null : DonViVanTai,
                                        PTVanChuyen = MaLoaiPhuongTien,
                                        LoaiHangHoa = MaLoaiHangHoa,
                                        DonViTinh = "CHUYEN",
                                        DiemLayRong = string.IsNullOrEmpty(DiemLayRong) ? null : int.Parse(DiemLayRong),
                                        DiemTraRong = string.IsNullOrEmpty(DiemTraRong) ? null : int.Parse(DiemTraRong),
                                        KhoiLuong = string.IsNullOrEmpty(KhoiLuong) ? null : double.Parse(KhoiLuong),
                                        TheTich = string.IsNullOrEmpty(TheTich) ? null : double.Parse(TheTich),
                                        SoKien = string.IsNullOrEmpty(SoKien) ? null : double.Parse(SoKien),
                                    });
                                }
                            }
                        }

                        foreach (var item in list)
                        {
                            if (string.IsNullOrEmpty(item.MaVanDonKH))
                            {
                                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Mã BookingNo";
                            }
                            if (string.IsNullOrEmpty(item.LoaiVanDon))
                            {
                                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Loại Vận Đơn";
                            }
                            if (string.IsNullOrEmpty(item.MaKH))
                            {
                                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Mã Khách Hàng";
                            }
                            if (string.IsNullOrEmpty(item.MaPTVC))
                            {
                                ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống Loại Hình";
                            }

                            if (item.LoaiVanDon == "nhap")
                            {
                                if (item.MaPTVC == "FCL")
                                {
                                    foreach (var itemHandling in item.arrHandlings)
                                    {
                                        if (itemHandling.DiemTraRong == null)
                                        {
                                            ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống điểm trả rỗng,";
                                        }

                                        if (!string.IsNullOrEmpty(itemHandling.ContNo))
                                        {
                                            if (!Regex.IsMatch(itemHandling.ContNo.Trim(), "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                                            {
                                                ErrorValidate += "Booking " + item.MaVanDonKH + ": Mã ContNo không hợp lệ \r\n";
                                            }
                                        }

                                        if (item.ThoiGianHanLenh == null)
                                        {
                                            ErrorValidate += "Booking " + item.MaVanDonKH + ": Không được bỏ trống Thời Gian Hạn Lệnh";
                                        }
                                    }
                                }
                                else if (item.MaPTVC == "FTL")
                                {
                                    foreach (var itemHandling in item.arrHandlings)
                                    {
                                        itemHandling.DiemLayRong = null;
                                        itemHandling.DiemTraRong = null;
                                        itemHandling.ContNo = null;
                                    }
                                }
                                else
                                {
                                    ErrorValidate += "Booking " + item.MaVanDonKH + ": Phương thức vận chuyển này không được hỗ trợ";
                                }
                            }
                            else if (item.LoaiVanDon == "xuat")
                            {
                                if (item.MaPTVC == "FCL")
                                {
                                    foreach (var itemHandling in item.arrHandlings)
                                    {
                                        if (itemHandling.DiemLayRong == null)
                                        {
                                            ErrorValidate += "Booking " + item.MaVanDonKH + ": không được bỏ trống điểm lấy rỗng,";
                                        }

                                        if (!string.IsNullOrEmpty(itemHandling.ContNo))
                                        {
                                            if (!Regex.IsMatch(itemHandling.ContNo, "([A-Z]{3})([UJZ])(\\d{6})(\\d)", RegexOptions.IgnoreCase))
                                            {
                                                ErrorValidate += "Booking " + item.MaVanDonKH + ": Mã ContNo không hợp lệ";
                                            }
                                        }

                                        if (item.ThoiGianHaCang == null)
                                        {
                                            ErrorValidate += "Booking " + item.MaVanDonKH + ": Không được bỏ trống Thời Gian Hạ Cảng";
                                        }
                                    }
                                }
                                else if (item.MaPTVC == "FTL")
                                {
                                    foreach (var itemHandling in item.arrHandlings)
                                    {
                                        itemHandling.DiemLayRong = null;
                                        itemHandling.DiemTraRong = null;
                                        itemHandling.ContNo = null;
                                    }
                                }
                                else
                                {
                                    ErrorValidate += "Booking " + item.MaVanDonKH + ": Phương thức vận chuyển này không được hỗ trợ";
                                }
                            }
                            else
                            {
                                ErrorValidate += "Booking " + item.MaVanDonKH + ": Loại Vận Đơn này không được hỗ trợ";
                            }
                        }

                        if (ErrorValidate == "")
                        {
                            foreach (var item in list)
                            {
                                var createTransport = await CreateTransport(item);

                                if (!createTransport.isSuccess)
                                {
                                    return createTransport;
                                }
                            }
                            return new BoolActionResult { isSuccess = true, Message = "Tạo Đơn Hàng Từ Excel Thành Công!" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                        }
                    }
                }
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
                && x.TrangThai != 29
                && x.TrangThai != 39).FirstOrDefaultAsync();

                if (checkTransportByBooking != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã booking đã tồn tại trong hệ thống" };
                }

                if (request.MaPTVC == "FCL" || request.MaPTVC == "LCL")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("TRUCK")).Count() > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Sai Loại Phương Tiện" };
                    }
                }

                if (request.MaPTVC == "FLT" || request.MaPTVC == "LTL")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Sai Loại Phương Tiện" };
                    }
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

                if (!string.IsNullOrEmpty(request.AccountId))
                {
                    var checkAccount = await _context.AccountOfCustomer.Where(x => x.MaAccount == request.AccountId.Trim()).FirstOrDefaultAsync();
                    if (checkAccount == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Account không tồn tại" };
                    }
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
                    MaAccount = request.AccountId,
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

                            #region add price table

                            //if (!string.IsNullOrEmpty(item.DonViVanTai))
                            //{
                            //    var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                            //    if (checkSupplier == null)
                            //    {
                            //        return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                            //    }

                            //    var priceSup = await GetPriceTable(item.DonViVanTai, null, request.DiemDau, request.DiemCuoi, getEmptyPlace, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);
                            //    var priceCus = await GetPriceTable(request.MaKH, request.AccountId, request.DiemDau, request.DiemCuoi, getEmptyPlace, item.DonViTinh, item.LoaiHangHoa, item.PTVanChuyen, request.MaPTVC);

                            //    var fPlace = checkPlace.Where(x => x.MaDiaDiem == request.DiemDau).FirstOrDefault();
                            //    var sPlace = checkPlace.Where(x => x.MaDiaDiem == request.DiemCuoi).FirstOrDefault();

                            //    var ePlace = new DiaDiem();
                            //    if (getEmptyPlace.HasValue)
                            //    {
                            //        ePlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                            //    }

                            //    if (priceSup == null)
                            //    {
                            //        return new BoolActionResult
                            //        {
                            //            isSuccess = false,
                            //            Message = "Đơn vị vận tải: "
                            //            + await _context.KhachHang.Where(x => x.MaKh == item.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                            //        + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //        " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            //        ", Loại Hàng Hóa:" + checkGoodsType +
                            //        ", Đơn Vị Tính: " + checkDVT +
                            //        ", Phương thức vận chuyển: " + checkPTVC +
                            //         (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                            //        };
                            //    }

                            //    if (priceCus == null)
                            //    {
                            //        if (request.AccountId == null)
                            //        {
                            //            return new BoolActionResult
                            //            {
                            //                isSuccess = false,
                            //                Message = "Khách Hàng: "
                            //          + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            //      + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //      " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //      ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            //      ", Loại Hàng Hóa:" + checkGoodsType +
                            //      ", Đơn Vị Tính: " + checkDVT +
                            //      ", Phương thức vận chuyển: " + checkPTVC +
                            //       (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                            //            };
                            //        }
                            //        else
                            //        {
                            //            return new BoolActionResult
                            //            {
                            //                isSuccess = false,
                            //                Message = " Khách Hàng: "
                            //        + await _context.KhachHang.Where(x => x.MaKh == request.MaKH).Select(x => x.TenKh).FirstOrDefaultAsync()
                            //        + " và Account" + await _context.AccountOfCustomer.Where(x => x.MaAccount == request.AccountId).Select(x => x.TenAccount).FirstOrDefaultAsync()
                            //    + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //    " - " + sPlace.TenDiaDiem + "(Khu Vực:" + _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                            //    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                            //    ", Loại Hàng Hóa:" + checkGoodsType +
                            //    ", Đơn Vị Tính: " + checkDVT +
                            //    ", Phương thức vận chuyển: " + checkPTVC +
                            //     (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                            //            };
                            //        }
                            //    }
                            //    itemHandling.DonViVanTai = item.DonViVanTai;
                            //    itemHandling.BangGiaKh = priceCus.ID;
                            //    itemHandling.BangGiaNcc = priceSup.ID;
                            //    itemHandling.DonGiaKh = priceCus.DonGia;
                            //    itemHandling.DonGiaNcc = priceSup.DonGia;
                            //}

                            #endregion add price table

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
              && x.TrangThai != 29
              && x.TrangThai != 39).FirstOrDefaultAsync();

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

                if (!string.IsNullOrEmpty(request.AccountId))
                {
                    var checkAccount = await _context.AccountOfCustomer.Where(x => x.MaAccount == request.AccountId.Trim()).FirstOrDefaultAsync();
                    if (checkAccount == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Account không tồn tại" };
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
                    MaAccount = request.AccountId,
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
                    list = list.Where(x => request.TransportIds.Contains(x.vd.MaVanDon) && (x.vd.TrangThai == 8 || x.vd.TrangThai == 42));

                    foreach (var item in await list.ToListAsync())
                    {
                        if (list.Where(x => x.vd.MaVanDon == item.vd.MaVanDon).Count() > 1)
                        {
                            return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn chạy 1 chuyến" };
                        }
                    }

                    if (list.Count() != request.TransportIds.Count)
                    {
                        return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
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

                var listCheck = new List<string>();
                foreach (var item in data)
                {
                    if (data.Where(x => x.vd.MaPtvc == "FTL" || x.vd.MaPtvc == "LTL").Count() > 0)
                    {
                        if (!listCheck.Contains("LTL"))
                        {
                            listCheck.Add("LTL");
                        }
                    }

                    if (data.Where(x => x.vd.MaPtvc == "FCL" || x.vd.MaPtvc == "LCL").Count() > 0)
                    {
                        if (!listCheck.Contains("LCL"))
                        {
                            listCheck.Add("LCL");
                        }
                    }
                }

                if (listCheck.Count() > 1)
                {
                    return new LoadJoinTransports { MessageErrors = "Vui lòng chỉ ghép các vận đơn cùng loại xe" };
                }

                return new LoadJoinTransports()
                {
                    HangTau = data.Select(x => x.vd.HangTau).FirstOrDefault(),
                    TenTau = data.Select(x => x.vd.Tau).FirstOrDefault(),
                    handlingLess = data.Select(x => x.dpl).FirstOrDefault() != null ? data.Select(x => new CreateHandlingLess()
                    {
                        PTVanChuyen = x.dpl.MaLoaiPhuongTien,
                        DonViVanTai = x.dpl.DonViVanTai,
                        XeVanChuyen = x.dpl.MaSoXe,
                        TaiXe = x.dpl.MaTaiXe,
                        GhiChu = x.dpl.GhiChu,
                        Romooc = x.dpl.MaRomooc,
                    }).FirstOrDefault() : null,
                    loadTransports = data.Select(x => new LoadTransports()
                    {
                        handlingId = x.dpl == null ? null : x.dpl.Id,
                        ThuTuGiaoHang = x.dpl == null ? null : x.dpl.ThuTuGiaoHang,
                        LoaiHangHoa = x.dpl == null ? null : x.dpl.MaLoaiHangHoa,
                        SealHq = x.dpl == null ? null : x.dpl.SealHq,
                        SealNp = x.dpl == null ? null : x.dpl.SealNp,
                        ContNo = x.dpl == null ? null : x.dpl.ContNo,
                        AccountId = x.vd.MaAccount == null ? null : x.vd.MaAccount,
                        AccountName = x.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                        LoaiVanDon = x.vd.LoaiVanDon,
                        MaPTVC = x.vd.MaPtvc,
                        MaKH = _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                        MaVanDon = x.vd.MaVanDon,
                        MaVanDonKH = x.vd.MaVanDonKh,
                        DiemDau = _context.DiaDiem.Where(z => z.MaDiaDiem == x.vd.DiemDau).Select(z => z.TenDiaDiem).FirstOrDefault(),
                        DiemCuoi = _context.DiaDiem.Where(z => z.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                        DiemLayRong = x.dpl == null ? null : x.dpl.DiemLayRong,
                        DiemTraRong = x.dpl == null ? null : x.dpl.DiemTraRong,
                        TGHanLenh = x.vd.ThoiGianHanLenh,
                        TGLayRong = x.vd.ThoiGianLayRong,
                        TGTraRong = x.vd.ThoiGianTraRong,
                        TGHaCang = x.vd.ThoiGianHaCang,
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

                var listStatus = new List<int>(new int[] { 38, 31, 30, 20, 21 });

                if (listStatus.Contains(checkById.TrangThai))
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

                if (tempData.AccType == "NCC")
                {
                    var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == request.MaTaiXe).FirstOrDefaultAsync();
                    if (checkDriver == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Tài xế không tồn tại \r\n" };
                    }

                    var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.MaSoXe).FirstOrDefaultAsync();
                    if (checkVehicle == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Xe vận chuyển không tồn tại \r\n" };
                    }

                    if (!request.PTVanChuyen.Contains(checkVehicle.MaLoaiPhuongTien))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại xe không khớp với xe vận chuyển \r\n" };
                    }
                }
                else if (tempData.AccType == "NV")
                {
                    if (!string.IsNullOrEmpty(request.MaTaiXe))
                    {
                        var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == request.MaTaiXe).FirstOrDefaultAsync();
                        if (checkDriver == null)
                        {
                            return new BoolActionResult { isSuccess = false, Message = " Tài xế không tồn tại \r\n" };
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

                if (tempData.AccType == "NV")
                {
                    var priceSup = await GetPriceTable(request.DonViVanTai, null, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);
                    var priceCus = await GetPriceTable(getTransport.MaKh, getTransport.MaAccount, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, request.DonViTinh, request.LoaiHangHoa, request.PTVanChuyen, getTransport.MaPtvc);

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
                            Message = "Đơn Vị Vận Tải: "
                            + await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai).Select(x => x.TenKh).FirstOrDefaultAsync()
                        + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                        " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                        ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                        ", Loại Hàng Hóa:" + checkGoodsType +
                        ", Đơn Vị Tính: " + checkDVT +
                        ", Phương thức vận chuyển: " + getTransport.MaPtvc +
                         (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                        };
                    }

                    if (priceCus == null)
                    {
                        if (getTransport.MaAccount == null)
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = "Khách Hàng: "
                         + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                     + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                     " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                     ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                     ", Loại Hàng Hóa:" + checkGoodsType +
                     ", Đơn Vị Tính: " + checkDVT +
                     ", Phương thức vận chuyển: " + getTransport.MaPtvc +
                      (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                            };
                        }
                        else
                        {
                            return new BoolActionResult
                            {
                                isSuccess = false,
                                Message = " Khách Hàng: "
                                       + await _context.KhachHang.Where(x => x.MaKh == getTransport.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                       + " và Account:" + await _context.AccountOfCustomer.Where(x => x.MaAccount == getTransport.MaAccount).Select(x => x.TenAccount).FirstOrDefaultAsync()
                                   + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                   " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                   ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                   ", Loại Hàng Hóa:" + checkGoodsType +
                                   ", Đơn Vị Tính: " + checkDVT +
                                   ", Phương thức vận chuyển: " + getTransport.MaPtvc +
                                    (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                            };
                        }
                    }

                    if (!string.IsNullOrEmpty(request.DonViVanTai))
                    {
                        if (checkById.TrangThai == 19)
                        {
                            checkById.TrangThai = 27;
                            var listStatusTransport = new List<int>(new int[] { 27, 21, 31, 38 });
                            var getListHandlingOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == checkById.MaVanDon).ToListAsync();
                            if (getListHandlingOfTransport.Count == getListHandlingOfTransport.Where(x => listStatusTransport.Contains(x.TrangThai)).Count())
                            {
                                getTransport.TrangThai = 9;
                                getTransport.Updater = tempData.UserName;
                                getTransport.UpdatedTime = DateTime.Now;
                                _context.Update(getTransport);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(checkById.MaChuyen.Trim()))
                    {
                        checkById.MaChuyen = getTransport.MaPtvc + DateTime.Now.ToString("yyyyMMddHHmmssffff");
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
                    checkById.MaSoXe = request.MaSoXe;
                    checkById.ContNo = request.ContNo;
                    checkById.SealHq = request.SealHq;
                    checkById.SealNp = request.SealNp;
                    checkById.MaTaiXe = request.MaTaiXe;
                    checkById.MaRomooc = request.MaRomooc;
                    checkById.GhiChu = request.GhiChu;
                    checkById.UpdatedTime = DateTime.Now;
                    checkById.Updater = tempData.UserName;
                }
                else if (tempData.AccType == "NCC")
                {
                    checkById.MaSoXe = request.MaSoXe;
                    checkById.ContNo = request.ContNo;
                    checkById.SealHq = request.SealHq;
                    checkById.SealNp = request.SealNp;
                    checkById.MaTaiXe = request.MaTaiXe;
                    checkById.MaRomooc = request.MaRomooc;
                    checkById.GhiChu = request.GhiChu;
                    checkById.UpdatedTime = DateTime.Now;
                    checkById.Updater = tempData.UserName;
                }

                _context.Update(checkById);
                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    if (tempData.AccType == "NV")
                    {
                        var getSubFee = await _subFeePrice.GetListSubFeePriceActive(getTransport.MaKh, getTransport.MaAccount, checkById.MaLoaiHangHoa, getTransport.DiemDau, getTransport.DiemCuoi, getEmptyPlace, checkById.Id, checkById.MaLoaiPhuongTien);
                        foreach (var sfp in getSubFee)
                        {
                            if (sfp != null)
                            {
                                await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                                {
                                    PriceId = sfp.PriceId,
                                    MaDieuPhoi = checkById.Id,
                                    CreatedDate = DateTime.Now,
                                    Creator = tempData.UserName,
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                    var logDriverAndVehicle = await LogDriverChange(checkById.MaChuyen, request.MaTaiXe, request.MaSoXe);

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

                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon) && (x.TrangThai == 8 || x.TrangThai == 42)).ToListAsync();

                var listHandling = await _context.DieuPhoi.Where(x => loadTransports.Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                if (loadTransports.Count != request.arrTransports.Count())
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có trạng thái 'Chờ Điều Phối' " };
                }

                if (loadTransports.Select(x => x.MaPtvc).FirstOrDefault() != "LTL")
                {
                    if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                    }
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                string MaVanDonChung = loadTransports.Select(x => x.MaPtvc).FirstOrDefault().Trim() + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                int countTransport = request.arrTransports.Count();

                if (countTransport == 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không có vận đơn nào để ghép cả" };
                }

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

                            int? getEmptyPlace = null;
                            if (loadTransports.Select(x => x.MaPtvc).Contains("LCL") || loadTransports.Select(x => x.MaPtvc).Contains("FCL"))
                            {
                                if (request.PTVanChuyen.Contains("TRUCK"))
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện không hợp lệ" };
                                }

                                if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                                {
                                    if (itemRequest.TGHanLenh == null)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                                    }

                                    if (itemRequest.TGTraRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                    {
                                        if (itemRequest.TGTraRong <= itemRequest.TGLayHang)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                                        }

                                        if (itemRequest.TGTraRong <= itemRequest.TGTraHang)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                                        }
                                    }

                                    if (!itemRequest.DiemTraRong.HasValue)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                                    }
                                    getEmptyPlace = itemRequest.DiemTraRong;
                                }
                                else
                                {
                                    if (itemRequest.TGHaCang == null)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                                    }

                                    if (itemRequest.TGLayRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                    {
                                        if (itemRequest.TGLayRong >= itemRequest.TGLayHang)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                                        }
                                        if (itemRequest.TGLayRong >= itemRequest.TGTraHang)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                                        }
                                    }

                                    if (!itemRequest.DiemLayRong.HasValue)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                    }
                                    getEmptyPlace = itemRequest.DiemLayRong;
                                }

                                var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                                if (checkPlaceGetEmpty == null)
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                }
                            }
                            else
                            {
                                if (request.PTVanChuyen.Contains("CONT"))
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện không hợp lệ" };
                                }
                            }

                            if (itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                            {
                                if (itemRequest.TGLayHang >= itemRequest.TGTraHang)
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Thời gian Lấy Hàng không được lớn hơn Thời Gian Trả Hàng" };
                                }
                            }

                            var priceSup = await GetPriceTable(request.DonViVanTai, null, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                            var priceCus = await GetPriceTable(item.MaKh, item.MaAccount, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

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
                                if (item.MaAccount == null)
                                {
                                    return new BoolActionResult
                                    {
                                        isSuccess = false,
                                        Message = "Khách Hàng: "
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
                                else
                                {
                                    return new BoolActionResult
                                    {
                                        isSuccess = false,
                                        Message = " Khách Hàng: "
                                  + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                  + " và Account:" + await _context.AccountOfCustomer.Where(x => x.MaAccount == item.MaAccount).Select(x => x.TenAccount).FirstOrDefaultAsync()
                              + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                              " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                              ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                              ", Loại Hàng Hóa:" + checkGoodsType +
                              ", Đơn Vị Tính: " + checkDVT +
                              ", Phương thức vận chuyển: " + item.MaPtvc +
                               (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                    };
                                }
                            }

                            if (item.MaPtvc == "LTL" || item.MaPtvc == "FTL")
                            {
                                item.ThoiGianLayRong = null;
                                item.ThoiGianTraHang = null;
                                item.ThoiGianHaCang = null;
                                item.ThoiGianHanLenh = null;
                                request.Romooc = null;
                            }

                            if (item.MaPtvc == "LCL" || item.MaPtvc == "FCL")
                            {
                                if (item.LoaiVanDon == "nhap")
                                {
                                    item.ThoiGianTraRong = itemRequest.TGTraRong;
                                    item.ThoiGianHanLenh = itemRequest.TGHanLenh;
                                    item.ThoiGianHaCang = null;
                                }

                                if (item.LoaiVanDon == "xuat")
                                {
                                    item.ThoiGianLayRong = itemRequest.TGLayRong;
                                    item.ThoiGianHanLenh = null;
                                    item.ThoiGianHaCang = itemRequest.TGHaCang;
                                }
                            }

                            int trangthai = 19;
                            if (!string.IsNullOrEmpty(request.DonViVanTai))
                            {
                                item.TrangThai = 9;
                                trangthai = 27;
                                var handleVehicleStatus = await HandleVehicleStatus(trangthai, request.XeVanChuyen);
                            }

                            _context.VanDon.Update(item);

                            var checkHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == item.MaVanDon).FirstOrDefaultAsync();

                            if (checkHandling != null)
                            {
                                checkHandling.ThuTuGiaoHang = itemRequest.ThuTuGiaoHang;
                                checkHandling.MaChuyen = MaVanDonChung;
                                checkHandling.MaSoXe = request.XeVanChuyen;
                                checkHandling.MaTaiXe = request.TaiXe;
                                checkHandling.MaLoaiPhuongTien = request.PTVanChuyen;
                                checkHandling.DonViVanTai = request.DonViVanTai;
                                checkHandling.BangGiaKh = priceCus.ID;
                                checkHandling.BangGiaNcc = priceSup.ID;
                                checkHandling.DonGiaKh = priceCus.DonGia;
                                checkHandling.DonGiaNcc = priceSup.DonGia;
                                checkHandling.MaRomooc = request.Romooc;
                                checkHandling.ContNo = itemRequest.ContNo;
                                checkHandling.SealNp = itemRequest.SealNP;
                                checkHandling.SealHq = itemRequest.SealHQ;
                                checkHandling.SoKien = item.TongSoKien;
                                checkHandling.KhoiLuong = item.TongKhoiLuong;
                                checkHandling.TheTich = item.TongTheTich;
                                checkHandling.GhiChu = item.GhiChu;
                                checkHandling.DiemLayRong = itemRequest.DiemLayRong;
                                checkHandling.DiemTraRong = itemRequest.DiemTraRong;
                                checkHandling.TrangThai = trangthai;
                                checkHandling.CreatedTime = DateTime.Now;
                                checkHandling.Creator = tempData.UserName;
                                checkHandling.TongVanDonGhep = countTransport;

                                var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.MaKh, item.MaAccount, itemRequest.MaLoaiHangHoa, item.DiemDau, item.DiemCuoi, getEmptyPlace, null, request.PTVanChuyen);
                                foreach (var sfp in getSubFee)
                                {
                                    await _context.SubFeeByContract.AddAsync(new SubFeeByContract()
                                    {
                                        PriceId = sfp.PriceId,
                                        MaDieuPhoi = checkHandling.Id,
                                        CreatedDate = DateTime.Now,
                                        Creator = tempData.UserName,
                                    });
                                }
                            }
                            else
                            {
                                var insertHandling = await _context.DieuPhoi.AddAsync(new DieuPhoi()
                                {
                                    ThuTuGiaoHang = itemRequest.ThuTuGiaoHang,
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
                                    ContNo = itemRequest.ContNo,
                                    SealNp = itemRequest.SealNP,
                                    SealHq = itemRequest.SealHQ,
                                    SoKien = item.TongSoKien,
                                    KhoiLuong = item.TongKhoiLuong,
                                    TheTich = item.TongTheTich,
                                    GhiChu = item.GhiChu,
                                    DiemLayRong = itemRequest.DiemLayRong,
                                    DiemTraRong = itemRequest.DiemTraRong,
                                    TrangThai = trangthai,
                                    CreatedTime = DateTime.Now,
                                    Creator = tempData.UserName,
                                    TongVanDonGhep = countTransport,
                                });

                                await _context.SaveChangesAsync();

                                var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.MaKh, item.MaAccount, itemRequest.MaLoaiHangHoa, item.DiemDau, item.DiemCuoi, getEmptyPlace, null, request.PTVanChuyen);
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
                }

                var logDriverAndVehicle = await LogDriverChange(MaVanDonChung, request.TaiXe, request.XeVanChuyen);
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

        public async Task<BoolActionResult> UpdateHandlingLess(string maChuyen, UpdateHandlingLess request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var loadTransports = await _context.VanDon.Where(x => request.arrTransports.Select(x => x.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                if (loadTransports.Where(x => x.TrangThai == 22 || x.TrangThai == 29 || x.TrangThai == 39).Count() > 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể cập nhật chuyến này nữa" };
                }

                //if (loadTransports.Where(x => x.LoaiVanDon == loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault()).Count() != request.arrTransports.Count)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Vui lòng chỉ chọn vận đơn có cùng loại vận đơn (Nhập/Xuất)" };
                //}

                //if (loadTransports.Where(x => x.MaPtvc == loadTransports.Select(x => x.MaPtvc).FirstOrDefault()).Count() != request.arrTransports.Count)
                //{
                //    return new BoolActionResult { isSuccess = false, Message = "Chỉ được ghép các vận đơn có cùng phương thức vận chuyển(LCL/LTL) " };
                //}

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.PTVanChuyen).Select(x => x.TenLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkVehicleType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại phương tiện vận chuyển không tồn tại" };
                }

                if (loadTransports.Count != request.arrTransports.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Dữ liệu input không hợp lệ" };
                }

                var checkSupplier = await _context.KhachHang.Where(x => x.MaKh == request.DonViVanTai && x.MaLoaiKh == "NCC").FirstOrDefaultAsync();
                if (checkSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn vị vận tải không tồn tại" };
                }

                if (tempData.AccType == "NCC")
                {
                    var checkDriver = await _context.TaiXe.Where(x => x.MaTaiXe == request.TaiXe).FirstOrDefaultAsync();
                    if (checkDriver == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Tài xế không tồn tại \r\n" };
                    }

                    var checkVehicle = await _context.XeVanChuyen.Where(x => x.MaSoXe == request.XeVanChuyen).FirstOrDefaultAsync();
                    if (checkVehicle == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = " Xe vận chuyển không tồn tại \r\n" };
                    }

                    if (!request.PTVanChuyen.Contains(checkVehicle.MaLoaiPhuongTien))
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại xe không khớp với xe vận chuyển \r\n" };
                    }

                    var countTransport = request.arrTransports.Count;
                    var data = from vd in _context.VanDon
                               join dp in _context.DieuPhoi
                               on vd.MaVanDon equals dp.MaVanDon
                               where dp.MaChuyen == maChuyen
                               select new { vd, dp };

                    var dataHandling = await data.ToListAsync();

                    var listStatusHandling = new List<int>(new int[] { 38, 31, 30, 20, 21 });

                    foreach (var item in data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToList())
                    {
                        foreach (var itemRequest in request.arrTransports)
                        {
                            if (item.dp.MaVanDon == itemRequest.MaVanDon)
                            {
                                if (listStatusHandling.Contains(item.dp.TrangThai))
                                {
                                    continue;
                                }

                                int? getEmptyPlace = null;
                                if (loadTransports.Select(x => x.MaPtvc).Contains("LCL") || loadTransports.Select(x => x.MaPtvc).Contains("FCL"))
                                {
                                    if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                                    {
                                        if (itemRequest.TGHanLenh == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                                        }

                                        if (itemRequest.TGTraRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGTraRong <= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }

                                            if (itemRequest.TGTraRong <= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (!itemRequest.DiemTraRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemTraRong;
                                    }
                                    else
                                    {
                                        if (itemRequest.TGHaCang == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                                        }

                                        if (itemRequest.TGLayRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGLayRong >= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }
                                            if (itemRequest.TGLayRong >= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (!itemRequest.DiemLayRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemLayRong;
                                    }

                                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                                    if (checkPlaceGetEmpty == null)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                    }
                                }
                                else
                                {
                                    itemRequest.DiemLayRong = null;
                                    itemRequest.DiemTraRong = null;
                                }

                                item.dp.MaSoXe = request.XeVanChuyen;
                                item.dp.MaTaiXe = request.TaiXe;
                                item.dp.MaRomooc = request.Romooc;
                                item.dp.ContNo = itemRequest.ContNo;
                                item.dp.SealNp = itemRequest.SealNP;
                                item.dp.SealHq = itemRequest.SealHQ;
                                item.dp.Updater = tempData.UserName;
                            }
                        }
                    }
                }

                if (tempData.AccType == "NV")
                {
                    var countTransport = request.arrTransports.Count;
                    var data = from vd in _context.VanDon
                               join dp in _context.DieuPhoi
                               on vd.MaVanDon equals dp.MaVanDon
                               where dp.MaChuyen == maChuyen
                               select new { vd, dp };

                    var dataHandling = await data.ToListAsync();

                    if (dataHandling.Count != countTransport)
                    {
                        if (dataHandling.Where(x => x.dp.TrangThai != 19 && x.dp.TrangThai != 27).Count() > 0)
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Không thể Tách/Ghép chuyến nữa" };
                        }
                    }

                    if (countTransport == 0)
                    {
                        dataHandling.ForEach(x =>
                        {
                            string MaVanDonChung = x.vd.MaPtvc + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                            x.dp.MaChuyen = MaVanDonChung;
                            x.dp.TrangThai = 19;
                            x.dp.MaSoXe = null;
                            x.dp.MaTaiXe = null;
                            x.dp.DonViVanTai = null;
                            x.dp.BangGiaKh = null;
                            x.dp.BangGiaNcc = null;
                            x.dp.DonGiaKh = null;
                            x.dp.DonGiaNcc = null;
                            x.dp.MaRomooc = null;
                            x.dp.TongVanDonGhep = null;
                            x.dp.ThuTuGiaoHang = null;
                        });

                        var rs = await _context.SaveChangesAsync();

                        if (rs > 0)
                        {
                            await transaction.CommitAsync();
                            return new BoolActionResult { isSuccess = true, Message = "Đã hủy ghép chuyển tất cả vận đơn" };
                        }
                        else
                        {
                            return new BoolActionResult { isSuccess = false, Message = "Hủy ghép chuyển tất cả vận đơn thất bại" };
                        }
                    }

                    #region loại bỏ các vận đơn đã được ghép trước đó

                    var dataRemove = await data.Where(x => !request.arrTransports.Select(y => y.HandlingId).Contains(x.dp.Id) && x.dp.MaChuyen == maChuyen).ToListAsync();
                    dataRemove.ForEach(x =>
                    {
                        string MaVanDonChung = x.vd.MaPtvc + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                        x.dp.MaChuyen = MaVanDonChung;
                        x.dp.TrangThai = 19;
                        x.dp.MaSoXe = null;
                        x.dp.MaTaiXe = null;
                        x.dp.DonViVanTai = null;
                        x.dp.BangGiaKh = null;
                        x.dp.BangGiaNcc = null;
                        x.dp.DonGiaKh = null;
                        x.dp.DonGiaNcc = null;
                        x.dp.MaRomooc = null;
                        x.dp.TongVanDonGhep = null;
                        x.dp.ThuTuGiaoHang = null;
                    });

                    #endregion loại bỏ các vận đơn đã được ghép trước đó

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

                    #region Cập nhật thông tin các chuyến đã tồn tại trong Db

                    var listStatusHandling = new List<int>(new int[] { 38, 31, 30, 20, 21 });

                    foreach (var item in data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToList())
                    {
                        foreach (var itemRequest in request.arrTransports)
                        {
                            if (item.dp.MaVanDon == itemRequest.MaVanDon)
                            {
                                if (listStatusHandling.Contains(item.dp.TrangThai))
                                {
                                    continue;
                                }

                                int? getEmptyPlace = null;
                                if (loadTransports.Select(x => x.MaPtvc).Contains("LCL") || loadTransports.Select(x => x.MaPtvc).Contains("FCL"))
                                {
                                    if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                                    {
                                        if (itemRequest.TGHanLenh == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                                        }

                                        if (itemRequest.TGTraRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGTraRong <= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }

                                            if (itemRequest.TGTraRong <= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (!itemRequest.DiemTraRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemTraRong;
                                    }
                                    else
                                    {
                                        if (itemRequest.TGHaCang == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                                        }

                                        if (itemRequest.TGLayRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGLayRong >= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }
                                            if (itemRequest.TGLayRong >= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (!itemRequest.DiemLayRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemLayRong;
                                    }

                                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                                    if (checkPlaceGetEmpty == null)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                    }
                                }
                                else
                                {
                                    itemRequest.DiemLayRong = null;
                                    itemRequest.DiemTraRong = null;
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

                                if (!string.IsNullOrEmpty(request.DonViVanTai))
                                {
                                    if (item.dp.TrangThai == 19)
                                    {
                                        item.dp.TrangThai = 27;
                                    }

                                    var listStatusTransport = new List<int>(new int[] { 27, 21, 31, 38 });
                                    var getListHandlingOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == item.dp.MaVanDon).ToListAsync();
                                    if (getListHandlingOfTransport.Count == getListHandlingOfTransport.Where(x => listStatusTransport.Contains(x.TrangThai)).Count())
                                    {
                                        item.vd.TrangThai = 9;
                                        item.vd.Updater = tempData.UserName;
                                        item.vd.UpdatedTime = DateTime.Now;
                                    }

                                    var handleVehicleStatus = await HandleVehicleStatus(item.dp.TrangThai, request.XeVanChuyen, item.dp.MaSoXe);
                                }

                                var priceSup = await GetPriceTable(request.DonViVanTai, null, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);
                                var priceCus = await GetPriceTable(item.vd.MaKh, item.vd.MaAccount, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.vd.MaPtvc);

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
                                    + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                    " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                    ", Loại Hàng Hóa:" + checkGoodsType +
                                    ", Đơn Vị Tính: " + checkDVT +
                                    ", Phương thức vận chuyển: " + item.vd.MaPtvc +
                                     (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                    };
                                }

                                if (priceCus == null)
                                {
                                    if (item.vd.MaAccount == null)
                                    {
                                        return new BoolActionResult
                                        {
                                            isSuccess = false,
                                            Message = "Khách Hàng: "
                                      + await _context.KhachHang.Where(x => x.MaKh == item.vd.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                  + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                  " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                  ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                  ", Loại Hàng Hóa:" + checkGoodsType +
                                  ", Đơn Vị Tính: " + checkDVT +
                                  ", Phương thức vận chuyển: " + item.vd.MaPtvc +
                                   (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                        };
                                    }
                                    else
                                    {
                                        return new BoolActionResult
                                        {
                                            isSuccess = false,
                                            Message = " Khách Hàng: "
                                     + await _context.KhachHang.Where(x => x.MaKh == item.vd.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                     + " và Account: " + await _context.AccountOfCustomer.Where(x => x.MaAccount == item.vd.MaAccount).Select(x => x.TenAccount).FirstOrDefaultAsync()
                                 + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                 " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                 ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                 ", Loại Hàng Hóa:" + checkGoodsType +
                                 ", Đơn Vị Tính: " + checkDVT +
                                 ", Phương thức vận chuyển: " + item.vd.MaPtvc +
                                  (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                        };
                                    }
                                }

                                item.dp.ThuTuGiaoHang = itemRequest.ThuTuGiaoHang;
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
                                item.dp.ContNo = itemRequest.ContNo;
                                item.dp.SealNp = itemRequest.SealNP;
                                item.dp.SealHq = itemRequest.SealHQ;
                                item.dp.DiemLayRong = itemRequest.DiemLayRong;
                                item.dp.DiemTraRong = itemRequest.DiemTraRong;
                                item.dp.Updater = tempData.UserName;
                                item.dp.TongVanDonGhep = countTransport;

                                var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.vd.MaKh, item.vd.MaAccount, itemRequest.MaLoaiHangHoa, item.vd.DiemDau, item.vd.DiemCuoi, getEmptyPlace, item.dp.Id, item.dp.MaLoaiPhuongTien);
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

                                int? getEmptyPlace = null;
                                if (loadTransports.Select(x => x.MaPtvc).Contains("LCL") || loadTransports.Select(x => x.MaPtvc).Contains("FCL"))
                                {
                                    if (loadTransports.Select(y => y.LoaiVanDon).FirstOrDefault() == "nhap")
                                    {
                                        if (itemRequest.TGHanLenh == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạn lệnh hoặc thời gian có mặt" };
                                        }

                                        if (itemRequest.TGTraRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGTraRong <= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }

                                            if (itemRequest.TGTraRong <= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian trả rỗng không được nhỏ hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (!itemRequest.DiemTraRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm trả rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemTraRong;
                                    }
                                    else
                                    {
                                        if (itemRequest.TGHaCang == null)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Không để trống thời gian hạ cảng" };
                                        }

                                        if (itemRequest.TGLayRong != null && itemRequest.TGLayHang != null && itemRequest.TGTraHang != null)
                                        {
                                            if (itemRequest.TGLayRong >= itemRequest.TGLayHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Lấy Hàng" };
                                            }
                                            if (itemRequest.TGLayRong >= itemRequest.TGTraHang)
                                            {
                                                return new BoolActionResult { isSuccess = false, Message = "Thời gian lấy rỗng không được lớn hơn hoặc bằng Thời Gian Trả Hàng" };
                                            }
                                        }

                                        if (itemRequest.DiemLayRong.HasValue)
                                        {
                                            return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                        }
                                        getEmptyPlace = itemRequest.DiemLayRong;
                                    }

                                    var checkPlaceGetEmpty = await _context.DiaDiem.Where(x => x.MaDiaDiem == getEmptyPlace).FirstOrDefaultAsync();
                                    if (checkPlaceGetEmpty == null)
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Điểm lấy rỗng không tồn tại" };
                                    }
                                }
                                else
                                {
                                    itemRequest.DiemLayRong = null;
                                    itemRequest.DiemTraRong = null;
                                }

                                var priceSup = await GetPriceTable(request.DonViVanTai, null, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);
                                var priceCus = await GetPriceTable(item.MaKh, item.MaAccount, item.DiemDau, item.DiemCuoi, getEmptyPlace, itemRequest.MaDVT, itemRequest.MaLoaiHangHoa, request.PTVanChuyen, item.MaPtvc);

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
                                    if (item.MaAccount == null)
                                    {
                                        return new BoolActionResult
                                        {
                                            isSuccess = false,
                                            Message = "Khách Hàng: "
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
                                    else
                                    {
                                        return new BoolActionResult
                                        {
                                            isSuccess = false,
                                            Message = " Khách Hàng: "
                                     + await _context.KhachHang.Where(x => x.MaKh == item.MaKh).Select(x => x.TenKh).FirstOrDefaultAsync()
                                     + " và Account: " + await _context.AccountOfCustomer.Where(x => x.MaAccount == item.MaAccount).Select(x => x.TenAccount).FirstOrDefaultAsync()
                                    + " Chưa có bảng giá cho Cung Đường: " + fPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == fPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                    " - " + sPlace.TenDiaDiem + "(Khu Vực:" + await _context.DiaDiem.Where(x => x.MaDiaDiem == sPlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + " )" +
                                    ", Phương Tiện Vận Chuyển: " + checkVehicleType +
                                    ", Loại Hàng Hóa:" + checkGoodsType +
                                    ", Đơn Vị Tính: " + checkDVT +
                                    ", Phương thức vận chuyển: " + item.MaPtvc +
                                    (getEmptyPlace == null ? "" : ", Điểm lấy/trả rỗng: " + ePlace.TenDiaDiem + "(Khu Vực: " + await _context.DiaDiem.Where(x => x.MaDiaDiem == ePlace.DiaDiemCha).Select(x => x.TenDiaDiem).FirstOrDefaultAsync() + ")")
                                        };
                                    }
                                }

                                var listStatusTransport = new List<int>(new int[] { 19, 21, 31, 38 });
                                var getListHandlingOfTransport = await _context.DieuPhoi.Where(x => x.MaVanDon == item.MaVanDon).ToListAsync();
                                if (getListHandlingOfTransport.Count == getListHandlingOfTransport.Where(x => listStatusTransport.Contains(x.TrangThai)).Count())
                                {
                                    item.TrangThai = 9;
                                    item.Updater = tempData.UserName;
                                    item.UpdatedTime = DateTime.Now;
                                }

                                var insertHandling = await _context.DieuPhoi.AddAsync(new DieuPhoi()
                                {
                                    MaVanDon = item.MaVanDon,
                                    ThuTuGiaoHang = itemRequest.ThuTuGiaoHang,
                                    MaChuyen = maChuyen,
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
                                    ContNo = itemRequest.ContNo,
                                    SealNp = itemRequest.SealNP,
                                    SealHq = itemRequest.SealHQ,
                                    SoKien = item.TongSoKien,
                                    KhoiLuong = item.TongKhoiLuong,
                                    TheTich = item.TongTheTich,
                                    GhiChu = item.GhiChu,
                                    DiemLayRong = itemRequest.DiemLayRong,
                                    DiemTraRong = itemRequest.DiemLayRong,
                                    TongVanDonGhep = countTransport,
                                    TrangThai = 27,
                                    CreatedTime = DateTime.Now,
                                    Creator = tempData.UserName,
                                    Updater = tempData.UserName,
                                });

                                await _context.SaveChangesAsync();

                                var getSubFee = await _subFeePrice.GetListSubFeePriceActive(item.MaKh, item.MaAccount, itemRequest.MaLoaiHangHoa, item.DiemDau, item.DiemCuoi, getEmptyPlace, null, request.PTVanChuyen);
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

                    #region Cập nhật thông tin các vận đơn đã được ghép

                    var listTransport = await data.Where(x => request.arrTransports.Select(y => y.MaVanDon).Contains(x.dp.MaVanDon)).ToListAsync();
                    foreach (var item in listTransport.Select(x => x.vd).ToList())
                    {
                        foreach (var itemRequest in request.arrTransports)
                        {
                            if (itemRequest.MaVanDon == item.MaVanDon)
                            {
                                if (item.TrangThai == 11 || item.TrangThai == 22 || item.TrangThai == 29)
                                {
                                    continue;
                                }

                                item.UpdatedTime = DateTime.Now;
                                item.Updater = tempData.UserName;

                                if (item.MaPtvc == "LTL" || item.MaPtvc == "FTL")
                                {
                                    item.ThoiGianLayRong = null;
                                    item.ThoiGianTraRong = null;
                                    item.ThoiGianHaCang = null;
                                    item.ThoiGianHanLenh = null;
                                    request.Romooc = null;
                                }

                                if (item.MaPtvc == "LCL" || item.MaPtvc == "FCL")
                                {
                                    if (item.LoaiVanDon == "nhap")
                                    {
                                        item.ThoiGianHaCang = null;
                                        item.ThoiGianLayRong = null;
                                    }

                                    if (item.LoaiVanDon == "xuat")
                                    {
                                        item.ThoiGianHanLenh = null;
                                        item.ThoiGianTraRong = null;
                                    }
                                }
                                _context.VanDon.Update(item);
                            }
                        }
                    }

                    #endregion Cập nhật thông tin các vận đơn đã được ghép
                }

                var logDriverAndVehicle = await LogDriverChange(maChuyen, request.TaiXe, request.XeVanChuyen);
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
                    AccountId = transport.MaAccount,
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
                    AccountId = x.transport.MaAccount,
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

                if (request.MaPTVC == "FCL" || request.MaPTVC == "LCL")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("TRUCK")).Count() > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Sai Loại Phương Tiện" };
                    }
                }

                if (request.MaPTVC == "FLT" || request.MaPTVC == "LTL")
                {
                    if (request.arrHandlings.Where(x => x.PTVanChuyen.Contains("CONT")).Count() > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Sai Loại Phương Tiện" };
                    }
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

                checkTransport.MaAccount = request.AccountId;
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
                            if (request.MaPTVC == "FCL")
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
                            if (request.MaPTVC == "FCL")
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

                checkTransport.MaAccount = request.AccountId;
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

            var ds = listData.ToQueryString();

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

            if (listFilter.accountIds.Count > 0)
            {
                listData = listData.Where(x => listFilter.accountIds.Contains(x.transport.MaAccount));
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
                AccountName = x.transport.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.transport.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
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
                    AccountName = x.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
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
                               && ttvd.LangId == tempData.LangID
                               select new { vd, vddp, ttdp, ttvd };

                var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
                if (tempData.AccType == "NV")
                {
                    listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.vd.MaKh));

                    if (listFilter.customers.Count > 0)
                    {
                        listData = listData.Where(x => listFilter.customers.Contains(x.vd.MaKh));
                    }

                    if (listFilter.suppliers.Count > 0)
                    {
                        listData = listData.Where(x => listFilter.suppliers.Contains(x.vddp.DonViVanTai));
                    }

                    if (listFilter.accountIds.Count > 0)
                    {
                        listData = listData.Where(x => listFilter.accountIds.Contains(x.vd.MaAccount));
                    }

                    if (listFilter.users.Count > 0)
                    {
                        listData = listData.Where(x => listFilter.users.Contains(x.vddp.Creator));
                    }
                }
                else if (tempData.AccType == "NCC")
                {
                    listData = listData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.vddp.DonViVanTai));
                }

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.vd.MaVanDon.Contains(filter.Keyword) || x.vddp.MaSoXe.Contains(filter.Keyword) || x.vddp.ContNo.Contains(filter.Keyword) || x.vd.MaVanDonKh.Contains(filter.Keyword) || x.vddp.MaChuyen.Contains(filter.Keyword));
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

                var pagedData = await listData.OrderByDescending(x => x.vd.CreatedTime).ThenBy(x => x.vddp.MaChuyen).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListHandling()
                {
                    AccountName = tempData.AccType == "NCC" ? null : x.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == x.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
                    MaChuyen = x.vddp.MaChuyen,
                    TongVanDonGhep = x.vddp.TongVanDonGhep,
                    DiemDau = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    DiemCuoi = _context.DiaDiem.Where(y => y.MaDiaDiem == x.vd.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    HangTau = x.vd.HangTau,
                    MaVanDonKH = x.vd.MaVanDonKh,
                    MaKH = tempData.AccType == "NCC" ? null : _context.KhachHang.Where(y => y.MaKh == x.vd.MaKh).Select(y => y.TenKh).FirstOrDefault(),
                    DonViVanTai = tempData.AccType == "NCC" ? null : _context.KhachHang.Where(y => y.MaKh == x.vddp.DonViVanTai).Select(y => y.TenKh).FirstOrDefault(),
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
                    AccountName = data.vd.MaAccount == null ? null : _context.AccountOfCustomer.Where(y => y.MaAccount == data.vd.MaAccount).Select(y => y.TenAccount).FirstOrDefault(),
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

        public async Task<BoolActionResult> ChangeStatusHandling(int id, string maChuyen)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getListChuyen = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen && x.TrangThai != 38).ToListAsync();

                if (getListChuyen.Count == 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không có chuyến nào cả" };
                }

                string mess = "";

                if (getListChuyen.Count < 2)
                {
                    var getdata = from dp in _context.DieuPhoi
                                  join vd in _context.VanDon
                                  on dp.MaVanDon equals vd.MaVanDon
                                  where dp.Id == id
                                  select new { vd, dp };

                    var data = await getdata.FirstOrDefaultAsync();

                    if (data.dp.MaLoaiPhuongTien.Contains("CONT"))
                    {
                        if (data.vd.LoaiVanDon == "xuat")
                        {
                            switch (data.dp.TrangThai)
                            {
                                case 27:
                                    data.vd.TrangThai = 10;
                                    data.vd.UpdatedTime = DateTime.Now;
                                    data.vd.Updater = tempData.UserName;
                                    data.dp.TrangThai = 17;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianLayRongThucTe = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đang Đi Lấy Rỗng";
                                    break;

                                case 17:
                                    if (string.IsNullOrEmpty(data.dp.ContNo))
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng cập nhật ContNo" };
                                    }
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                    data.dp.TrangThai = 37;
                                    mess = "Đã thay đổi trạng thái thành: Đang Đóng Hàng";
                                    break;

                                case 37:
                                    data.dp.TrangThai = 18;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                    break;

                                case 18:
                                    data.dp.TrangThai = 36;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianTraHangThucTe = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";
                                    break;

                                case 36:
                                    data.dp.TrangThai = 20;
                                    data.vd.UpdatedTime = DateTime.Now;
                                    data.vd.Updater = tempData.UserName;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianHoanThanh = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";
                                    var saveData = await _context.SaveChangesAsync();
                                    if (saveData > 0)
                                    {
                                        var changeStatusVehicle = await HandleVehicleStatus(20, data.dp.MaSoXe);

                                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == data.dp.MaVanDon).ToListAsync();

                                        //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                        if (getListHandling.Count() == getListHandling.Where(x => x.TrangThai == 20).Count())
                                        {
                                            data.vd.TrangThai = 22;
                                            data.vd.UpdatedTime = DateTime.Now;
                                            data.vd.Updater = tempData.UserName;
                                        }

                                        //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                        var listStatus = new List<int>(new int[] { 21, 31, 38 });
                                        if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                        {
                                            if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                            {
                                                data.vd.TrangThai = 44;
                                                data.vd.UpdatedTime = DateTime.Now;
                                                data.vd.Updater = tempData.UserName;
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                        await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                        await transaction.CommitAsync();
                                        return new BoolActionResult { isSuccess = true, Message = mess };
                                    }
                                    else
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    }
                                default:
                                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    break;
                            }
                        }
                        else if (data.vd.LoaiVanDon == "nhap")
                        {
                            switch (data.dp.TrangThai)
                            {
                                case 27:
                                    data.vd.Updater = tempData.UserName;
                                    data.vd.UpdatedTime = DateTime.Now;
                                    data.vd.TrangThai = 10;

                                    data.dp.TrangThai = 40;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.ThoiGianLayHangThucTe = DateTime.Now;

                                    mess = "Đã thay đổi trạng thái thành: Đang Đi Lấy Hàng";
                                    break;

                                case 40:
                                    if (string.IsNullOrEmpty(data.dp.ContNo))
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng cập nhật ContNo" };
                                    }
                                    data.dp.TrangThai = 18;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.Updater = tempData.UserName;

                                    mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                    break;

                                case 18:
                                    data.dp.TrangThai = 36;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.Updater = tempData.UserName;
                                    mess = "Đã thay đổi trạng thái thành: Đang Giao Hàng";
                                    break;

                                case 36:
                                    data.dp.TrangThai = 35;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.ThoiGianTraRongThucTe = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đang Trả Rỗng";
                                    break;

                                case 35:
                                    data.vd.UpdatedTime = DateTime.Now;
                                    data.vd.Updater = tempData.UserName;
                                    data.dp.TrangThai = 20;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianHoanThanh = DateTime.Now;
                                    data.dp.Updater = tempData.UserName;
                                    mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";
                                    var saveData = await _context.SaveChangesAsync();
                                    if (saveData > 0)
                                    {
                                        var changeStatusVehicle = await HandleVehicleStatus(20, data.dp.MaSoXe);

                                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == data.dp.MaVanDon).ToListAsync();

                                        //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                        if (getListHandling.Count() == getListHandling.Where(x => x.TrangThai == 20).Count())
                                        {
                                            data.vd.TrangThai = 22;
                                            data.vd.UpdatedTime = DateTime.Now;
                                            data.vd.Updater = tempData.UserName;
                                        }

                                        //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                        var listStatus = new List<int>(new int[] { 21, 31, 38 });
                                        if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                        {
                                            if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                            {
                                                data.vd.TrangThai = 44;
                                                data.vd.UpdatedTime = DateTime.Now;
                                                data.vd.Updater = tempData.UserName;
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                        await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                        await transaction.CommitAsync();
                                        return new BoolActionResult { isSuccess = true, Message = mess };
                                    }
                                    else
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    }

                                default:
                                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    break;
                            }
                        }
                    }
                    else if (data.dp.MaLoaiPhuongTien.Contains("TRUCK"))
                    {
                        switch (data.dp.TrangThai)
                        {
                            case 27:
                                data.dp.TrangThai = 37;
                                data.vd.TrangThai = 10;

                                data.vd.UpdatedTime = DateTime.Now;
                                data.vd.Updater = tempData.UserName;
                                data.dp.Updater = tempData.UserName;
                                data.dp.UpdatedTime = DateTime.Now;
                                data.dp.ThoiGianLayHangThucTe = DateTime.Now;

                                mess = "Đã thay đổi trạng thái thành: Đang Đóng Hàng";
                                break;

                            case 37:
                                data.dp.TrangThai = 18;
                                data.dp.Updater = tempData.UserName;
                                data.dp.UpdatedTime = DateTime.Now;
                                mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                break;

                            case 18:
                                data.dp.TrangThai = 36;
                                data.dp.Updater = tempData.UserName;
                                data.dp.UpdatedTime = DateTime.Now;
                                data.dp.ThoiGianTraHangThucTe = DateTime.Now;
                                mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";
                                break;

                            case 36:
                                data.vd.UpdatedTime = DateTime.Now;
                                data.vd.Updater = tempData.UserName;
                                data.dp.Updater = tempData.UserName;
                                data.dp.UpdatedTime = DateTime.Now;
                                data.dp.ThoiGianHoanThanh = DateTime.Now;
                                data.dp.TrangThai = 20;
                                mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";
                                var saveData = await _context.SaveChangesAsync();
                                if (saveData > 0)
                                {
                                    var changeStatusVehicle = await HandleVehicleStatus(20, data.dp.MaSoXe);

                                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == data.dp.MaVanDon).ToListAsync();

                                    //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                    if (getListHandling.Count() == getListHandling.Where(x => x.TrangThai == 20).Count())
                                    {
                                        data.vd.TrangThai = 22;
                                        data.vd.UpdatedTime = DateTime.Now;
                                        data.vd.Updater = tempData.UserName;
                                    }

                                    //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                    var listStatus = new List<int>(new int[] { 21, 31, 38 });
                                    if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                    {
                                        if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                        {
                                            data.vd.TrangThai = 44;
                                            data.vd.UpdatedTime = DateTime.Now;
                                            data.vd.Updater = tempData.UserName;
                                        }
                                    }

                                    await _context.SaveChangesAsync();
                                    await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                    await transaction.CommitAsync();
                                    return new BoolActionResult { isSuccess = true, Message = mess };
                                }
                                else
                                {
                                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                }
                                break;

                            default:
                                return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                break;
                        }
                    }
                    _context.Update(data.vd);
                    _context.Update(data.dp);
                }
                else
                {
                    var listData = getListChuyen.Where(x => x.TrangThai != 21).ToList();
                    var listTransport = await _context.VanDon.Where(x => listData.Select(x => x.MaVanDon).Contains(x.MaVanDon)).ToListAsync();

                    if (listData.Select(x => x.MaLoaiPhuongTien).FirstOrDefault().Contains("CONT"))
                    {
                        var getdata = from dp in _context.DieuPhoi
                                      join vd in _context.VanDon
                                      on dp.MaVanDon equals vd.MaVanDon
                                      where dp.Id == id
                                      select new { vd, dp };

                        var data = await getdata.FirstOrDefaultAsync();

                        if (data.vd.LoaiVanDon == "xuat")
                        {
                            switch (data.dp.TrangThai)
                            {
                                case 27:
                                    listData.ForEach(x =>
                                    {
                                        x.TrangThai = 17;
                                        x.UpdatedTime = DateTime.Now;
                                        x.Updater = tempData.UserName;
                                        x.ThoiGianLayRongThucTe = DateTime.Now;
                                    });
                                    listTransport.Where(x => x.TrangThai == 9).ToList().ForEach(x =>
                                    {
                                        x.TrangThai = 10;
                                        x.Updater = tempData.UserName;
                                        x.UpdatedTime = DateTime.Now;
                                    });

                                    mess = "Đã thay đổi trạng thái thành: Đang Đi Lấy Rỗng";
                                    break;

                                case 17:
                                    if (string.IsNullOrEmpty(data.dp.ContNo))
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng cập nhật ContNo" };
                                    }

                                    if (data.dp.MaLoaiPhuongTien == "CONT20")
                                    {
                                        if (data.vd.MaPtvc == "LCL")
                                        {
                                            listData.Where(x => listTransport.Where(y => y.MaPtvc == "LCL").Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToList().ForEach(x =>
                                            {
                                                x.TrangThai = 45;
                                                x.UpdatedTime = DateTime.Now;
                                                x.Updater = tempData.UserName;
                                            });
                                        }
                                        else
                                        {
                                            data.dp.TrangThai = 45;
                                            data.dp.Updater = tempData.UserName;
                                            data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                            data.dp.UpdatedTime = DateTime.Now;
                                        }
                                        mess = "Đã thay đổi trạng thái thành: Đã Lấy Rỗng";

                                        if (listData.Count == listData.Where(x => x.TrangThai == 45).Count())
                                        {
                                            listData.ForEach(x =>
                                            {
                                                x.TrangThai = 37;
                                                x.UpdatedTime = DateTime.Now;
                                                x.Updater = tempData.UserName;
                                            });
                                            mess = "Đã thay đổi trạng thái thành: Đang đóng hàng";
                                        }
                                    }
                                    else
                                    {
                                        listData.ForEach(x =>
                                        {
                                            x.TrangThai = 37;
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đang đóng hàng";
                                    }
                                    break;

                                case 37:
                                    data.dp.TrangThai = 43;
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đã Đóng Hàng";

                                    if (listData.Count == listData.Where(x => x.TrangThai == 43).Count())
                                    {
                                        listData.ForEach(x =>
                                        {
                                            x.TrangThai = 18;
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đang vận Chuyển";
                                    }

                                    break;

                                case 18:
                                    if (data.dp.MaLoaiPhuongTien == "CONT20")
                                    {
                                        if (data.vd.MaPtvc == "LCL")
                                        {
                                            listData.Where(x => listTransport.Where(y => y.MaPtvc == "LCL").Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToList().ForEach(x =>
                                            {
                                                x.TrangThai = 36;
                                                x.UpdatedTime = DateTime.Now;
                                                x.Updater = tempData.UserName;
                                            });
                                        }
                                        else
                                        {
                                            data.dp.TrangThai = 36;
                                            data.dp.Updater = tempData.UserName;
                                            data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                            data.dp.UpdatedTime = DateTime.Now;
                                        }
                                        mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";
                                    }
                                    else
                                    {
                                        listData.ForEach(x =>
                                        {
                                            x.TrangThai = 36;
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                            x.ThoiGianTraHangThucTe = DateTime.Now;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";
                                    }
                                    break;

                                case 36:
                                    if (data.vd.MaPtvc == "LCL")
                                    {
                                        listData.Where(x => listTransport.Where(y => y.MaPtvc == "LCL").Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToList().ForEach(x =>
                                        {
                                            x.TrangThai = 20;
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                            x.ThoiGianHoanThanh = DateTime.Now;
                                        });
                                    }
                                    else
                                    {
                                        data.dp.TrangThai = 20;
                                        data.dp.UpdatedTime = DateTime.Now;
                                        data.dp.Updater = tempData.UserName;
                                        data.dp.ThoiGianHoanThanh = DateTime.Now;
                                    }

                                    var saveData = await _context.SaveChangesAsync();
                                    if (saveData > 0)
                                    {
                                        var changeStatusVehicle = await HandleVehicleStatus(20, data.dp.MaSoXe);

                                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();
                                        //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                        foreach (var item in getListHandling)
                                        {
                                            if (getListHandling.Where(x => x.MaVanDon == item.MaVanDon).Count() == getListHandling.Where(x => x.MaVanDon == item.MaVanDon && x.TrangThai == 20).Count())
                                            {
                                                var transport = await _context.VanDon.Where(x => x.MaVanDon == item.MaVanDon).FirstOrDefaultAsync();
                                                transport.TrangThai = 22;
                                                transport.UpdatedTime = DateTime.Now;
                                                transport.Updater = tempData.UserName;
                                            }
                                        }

                                        //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                        var listStatus = new List<int>(new int[] { 21, 31, 38 });
                                        if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                        {
                                            if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                            {
                                                data.vd.TrangThai = 44;
                                                data.vd.UpdatedTime = DateTime.Now;
                                                data.vd.Updater = tempData.UserName;
                                            }
                                        }
                                        mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";

                                        await _context.SaveChangesAsync();
                                        await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                        await transaction.CommitAsync();
                                        return new BoolActionResult { isSuccess = true, Message = mess };
                                    }
                                    else
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    }
                                    break;

                                default:
                                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    break;
                            }
                        }
                        else if (data.vd.LoaiVanDon == "nhap")
                        {
                            switch (data.dp.TrangThai)
                            {
                                case 27:
                                    listData.ForEach(x =>
                                    {
                                        x.TrangThai = 40;
                                        x.UpdatedTime = DateTime.Now;
                                        x.Updater = tempData.UserName;
                                        x.ThoiGianLayHangThucTe = DateTime.Now;
                                    });
                                    listTransport.Where(x => x.TrangThai == 9).ToList().ForEach(x =>
                                    {
                                        x.TrangThai = 10;
                                        x.UpdatedTime = DateTime.Now;
                                        x.Updater = tempData.UserName;
                                    });
                                    mess = "Đã thay đổi trạng thái thành: Đang Đi Lấy Hàng";
                                    break;

                                case 40:
                                    if (string.IsNullOrEmpty(data.dp.ContNo))
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Vui lòng cập nhật ContNo" };
                                    }

                                    if (data.dp.MaLoaiPhuongTien == "CONT20")
                                    {
                                        if (data.vd.MaPtvc == "LCL")
                                        {
                                            listData.Where(x => listTransport.Where(y => y.MaPtvc == "LCL").Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToList().ForEach(x =>
                                            {
                                                x.TrangThai = 43;
                                                x.UpdatedTime = DateTime.Now;
                                                x.Updater = tempData.UserName;
                                                x.ThoiGianLayHangThucTe = DateTime.Now;
                                            });
                                        }
                                        else
                                        {
                                            data.dp.TrangThai = 43;
                                            data.dp.Updater = tempData.UserName;
                                            data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                            data.dp.UpdatedTime = DateTime.Now;
                                        }

                                        mess = "Đã thay đổi trạng thái thành: Đã Đóng Hàng";

                                        if (listData.Count == listData.Where(x => x.TrangThai == 43).Count())
                                        {
                                            listData.ForEach(x =>
                                            {
                                                x.TrangThai = 18;
                                                x.UpdatedTime = DateTime.Now;
                                                x.Updater = tempData.UserName;
                                            });
                                            mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                        }
                                    }
                                    else
                                    {
                                        listData.ForEach(x =>
                                        {
                                            x.TrangThai = 18;
                                            x.UpdatedTime = DateTime.Now;
                                            data.dp.ThoiGianLayHangThucTe = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                    }
                                    break;

                                case 18:
                                    data.dp.Updater = tempData.UserName;
                                    data.dp.TrangThai = 36;
                                    data.dp.UpdatedTime = DateTime.Now;
                                    data.dp.ThoiGianTraHangThucTe = DateTime.Now;
                                    mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";

                                    if (listData.Count == listData.Where(x => x.TrangThai == 36).Count())
                                    {
                                        listData.ForEach(x =>
                                        {
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                            x.TrangThai = 35;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đang Trả Rỗng";
                                    }
                                    break;

                                case 36:
                                    if (listData.Count == listData.Where(x => x.TrangThai == 20 || x.TrangThai == 36).Count())
                                    {
                                        listData.Where(x => x.TrangThai == 36).ToList().ForEach(x =>
                                        {
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                            x.TrangThai = 35;
                                        });
                                        mess = "Đã thay đổi trạng thái thành: Đang Trả Rỗng";
                                    }
                                    break;

                                case 35:

                                    if (data.vd.MaPtvc == "LCL")
                                    {
                                        listData.Where(x => listTransport.Where(y => y.MaPtvc == "LCL").Select(y => y.MaVanDon).Contains(x.MaVanDon)).ToList().ForEach(x =>
                                        {
                                            x.TrangThai = 20;
                                            x.UpdatedTime = DateTime.Now;
                                            x.Updater = tempData.UserName;
                                            x.ThoiGianHoanThanh = DateTime.Now;
                                        });
                                    }
                                    else
                                    {
                                        data.dp.TrangThai = 20;
                                        data.dp.UpdatedTime = DateTime.Now;
                                        data.dp.Updater = tempData.UserName;
                                        data.dp.ThoiGianHoanThanh = DateTime.Now;
                                    }

                                    mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";

                                    var saveData = await _context.SaveChangesAsync();
                                    if (saveData > 0)
                                    {
                                        var changeStatusVehicle = await HandleVehicleStatus(20, data.dp.MaSoXe);

                                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen).ToListAsync();
                                        //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                        foreach (var item in getListHandling)
                                        {
                                            if (getListHandling.Where(x => x.MaVanDon == item.MaVanDon).Count() == getListHandling.Where(x => x.MaVanDon == item.MaVanDon && x.TrangThai == 20).Count())
                                            {
                                                var transport = await _context.VanDon.Where(x => x.MaVanDon == item.MaVanDon).FirstOrDefaultAsync();
                                                transport.TrangThai = 22;
                                                transport.UpdatedTime = DateTime.Now;
                                                transport.Updater = tempData.UserName;
                                            }
                                        }

                                        //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                        var listStatus = new List<int>(new int[] { 21, 31, 38 });
                                        if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                        {
                                            if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                            {
                                                data.vd.TrangThai = 44;
                                                data.vd.UpdatedTime = DateTime.Now;
                                                data.vd.Updater = tempData.UserName;
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                        await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                        await transaction.CommitAsync();
                                        return new BoolActionResult { isSuccess = true, Message = mess };
                                    }
                                    else
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    }

                                default:
                                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    break;
                            }
                        }
                        _context.Update(data.dp);
                        _context.Update(data.vd);
                    }
                    else if (listData.Select(x => x.MaLoaiPhuongTien).FirstOrDefault().Contains("TRUCK"))
                    {
                        var dataById = listData.Where(x => x.Id == id).FirstOrDefault();

                        switch (dataById.TrangThai)
                        {
                            case 27:
                                listData.ForEach(x =>
                                {
                                    x.TrangThai = 37;
                                    x.Updater = tempData.UserName;
                                    x.UpdatedTime = DateTime.Now;
                                    x.ThoiGianLayHangThucTe = DateTime.Now;
                                });
                                listTransport.ForEach(x =>
                                {
                                    x.TrangThai = 10;
                                    x.Updater = tempData.UserName;
                                    x.UpdatedTime = DateTime.Now;
                                });
                                mess = "Đã thay đổi trạng thái thành: Đang Đóng Hàng";
                                break;

                            case 37:
                                listData.ForEach(x =>
                                {
                                    x.TrangThai = 18;
                                    x.Updater = tempData.UserName;
                                    x.UpdatedTime = DateTime.Now;
                                });
                                mess = "Đã thay đổi trạng thái thành: Đang Vận Chuyển";
                                break;

                            case 18:
                                dataById.TrangThai = 36;
                                dataById.ThoiGianTraHangThucTe = DateTime.Now;
                                dataById.Updater = tempData.UserName;
                                dataById.UpdatedTime = DateTime.Now;

                                mess = "Đã thay đổi trạng thái thành: Đã Giao Hàng";

                                if (listData.Count() == listData.Where(x => x.TrangThai == 36).Count())
                                {
                                    listData.ForEach(x =>
                                    {
                                        x.ThoiGianHoanThanh = DateTime.Now;
                                        x.Updater = tempData.UserName;
                                        x.UpdatedTime = DateTime.Now;
                                        x.TrangThai = 20;
                                    });
                                    mess = "Đã thay đổi trạng thái thành: Hoàn Thành Chuyến";

                                    var saveData = await _context.SaveChangesAsync();
                                    if (saveData > 0)
                                    {
                                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == dataById.MaVanDon).FirstOrDefaultAsync();
                                        var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == getTransport.MaVanDon).ToListAsync();

                                        //Trường hợp toàn bộ chuyến đã hoàn thành thì hoàn thành vận đơn
                                        if (getListHandling.Count() == getListHandling.Where(x => x.TrangThai == 20).Count())
                                        {
                                            getTransport.TrangThai = 22;
                                            getTransport.UpdatedTime = DateTime.Now;
                                            getTransport.Updater = tempData.UserName;
                                        }

                                        //Trường hợp chuyến đã hoàn thành nhưng các chuyến còn lại bị hủy thì đóng vận đơn
                                        var listStatus = new List<int>(new int[] { 20, 21, 31, 38 });
                                        if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                                        {
                                            if (getListHandling.Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                                            {
                                                getTransport.TrangThai = 39;
                                                getTransport.UpdatedTime = DateTime.Now;
                                                getTransport.Updater = tempData.UserName;
                                            }
                                        }

                                        await _context.SaveChangesAsync();
                                        await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                                        await transaction.CommitAsync();
                                        return new BoolActionResult { isSuccess = true, Message = mess };
                                    }
                                    else
                                    {
                                        return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                    }
                                }
                                break;

                            default:
                                return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                                break;
                        }
                        _context.Update(dataById);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ChangeStatus", "UserId: " + tempData.UserName + " changes status handlingID= " + id + ", MaChuyen=" + maChuyen + ", status:" + mess);
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = mess };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không có gì được thực thi" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("BillOfLading", "UserId: " + tempData.UserName + "  SetRuning with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CancelHandling(int id, string note = null)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getById = from dp in _context.DieuPhoi
                              join vd in _context.VanDon
                              on dp.MaVanDon equals vd.MaVanDon
                              where dp.Id == id
                              select new { dp, vd };

                var data = await getById.FirstOrDefaultAsync();

                if (data == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lệnh điều phối không tồn tại" };
                }

                var listStatusCancel = new List<int>(new int[] { 17, 27, 37, 40 });

                var countTransport = await _context.VanDon.Where(x => x.MaVanDon == data.vd.MaVanDon).ToListAsync();

                if (listStatusCancel.Contains(data.dp.TrangThai))
                {
                    if (countTransport.Count() == 1)
                    {
                        data.vd.TrangThai = 42;
                        _context.Update(data.vd);
                    }

                    //string MaVanDonChung = data.vd.MaPtvc + DateTime.Now.ToString("yyyyMMddHHmmssffff");
                    data.dp.Updater = tempData.UserName;
                    data.dp.MaChuyen = "";
                    data.dp.MaSoXe = null;
                    data.dp.MaTaiXe = null;
                    data.dp.TrangThai = 19;
                    data.dp.BangGiaKh = null;
                    data.dp.BangGiaNcc = null;
                    data.dp.DonGiaKh = null;
                    data.dp.DonGiaNcc = null;
                    data.dp.DonViVanTai = null;
                    data.dp.MaRomooc = null;
                    data.dp.ThuTuGiaoHang = null;
                    data.dp.TongVanDonGhep = null;
                    if (!string.IsNullOrEmpty(note))
                    {
                        data.dp.GhiChu = data.dp.GhiChu + ",\r\n " + note;
                    }
                }

                var listStatusComplete = new List<int>(new int[] { 18 });
                if (listStatusComplete.Contains(data.dp.TrangThai))
                {
                    data.dp.TrangThai = 20;
                    data.dp.ThoiGianHoanThanh = DateTime.Now;
                    data.dp.UpdatedTime = DateTime.Now;
                    data.dp.Updater = tempData.UserName;
                    _context.DieuPhoi.Update(data.dp);

                    if (countTransport.Count() == 1)
                    {
                        data.vd.TrangThai = 42;
                        _context.Update(data.vd);
                    }
                }

                var result = await _context.SaveChangesAsync();
                if (result > 0)
                {
                    await _common.Log("BillOfLading", " UserId: " + tempData.UserName + " set Cancel handling " + id);
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

        public async Task<BoolActionResult> CancelHandlingByCustomer(int id, string note = null)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getById = from dp in _context.DieuPhoi
                              join vd in _context.VanDon
                              on dp.MaVanDon equals vd.MaVanDon
                              where dp.Id == id
                              select new { dp, vd };

                var data = await getById.FirstOrDefaultAsync();

                if (data == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Lệnh điều phối không tồn tại" };
                }

                if (data.dp.TrangThai != 19 && data.dp.TrangThai != 27 && data.dp.TrangThai != 30)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể hủy chuyến này nữa" };
                }

                data.dp.TrangThai = 38;
                data.dp.UpdatedTime = DateTime.Now;
                data.dp.Updater = tempData.UserName;

                if (!string.IsNullOrEmpty(note))
                {
                    data.dp.GhiChu = note;
                }

                _context.Update(data.dp);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var getListHandling = await _context.DieuPhoi.Where(x => x.MaVanDon == data.dp.MaVanDon).ToListAsync();

                    if (getListHandling.Count == getListHandling.Where(x => x.TrangThai == 38).Count())
                    {
                        data.vd.TrangThai = 39;
                        _context.Update(data.vd);
                        await _context.SaveChangesAsync();
                    }

                    var listStatus = new List<int>(new int[] { 21, 31, 38 });
                    if (getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count() > 0)
                    {
                        if (getListHandling.Where(X => X.TrangThai != 20).Count() == getListHandling.Where(x => listStatus.Contains(x.TrangThai)).Count())
                        {
                            data.vd.TrangThai = 44;
                            data.vd.UpdatedTime = DateTime.Now;
                            data.vd.Updater = tempData.UserName;
                        }
                    }

                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Đã hủy chuyến thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hủy chuyến thất bại" };
                }
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

        public async Task<List<DocumentType>> GetListImageByHandlingId(long handlingId)
        {
            var list = await _context.TepChungTu.Where(x => x.MaDieuPhoi == handlingId).ToListAsync();
            return list.Select(x => new DocumentType()
            {
                MaChungTu = x.Id,
                MaHinhAnh = x.MaHinhAnh,
                TenChungTu = x.TenChungTu,
                LoaiChungTu = x.LoaiChungTu,
                TenLoaiChungTu = _context.LoaiChungTu.Where(y => y.MaLoaiChungTu == x.LoaiChungTu).Select(y => y.TenLoaiChungTu).FirstOrDefault(),
                GhiChu = x.GhiChu,
                TrangThai = x.TrangThai,
                UpdatedTime = x.UpdatedTime,
                CreatedTime = x.CreatedTime,
                Creator = x.Creator,
            }).ToList();
        }

        public async Task<Attachment> GetImageById(long imageId)
        {
            var image = await _context.Attachment.Where(x => x.Id == imageId).FirstOrDefaultAsync();
            var name = image.FileName.Replace(image.FileType, "");

            return new Attachment
            {
                FilePath = Path.Combine(_common.GetFile(image.FilePath)),
                FileName = image.FileName.Replace(image.FileType.Trim(), ""),
                FileType = image.FileType
            };
        }

        public async Task<BoolActionResult> DeleteImageById(int docId)
        {
            try
            {
                var checkDoc = await _context.TepChungTu.Where(x => x.Id == docId).FirstOrDefaultAsync();
                if (checkDoc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hình ảnh không tồn tại" };
                }

                var image = await _context.Attachment.Where(x => x.Id == checkDoc.MaHinhAnh).FirstOrDefaultAsync();
                if (image == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hình ảnh không tồn tại" };
                }

                await _common.DeleteFileAsync(image.FileName, image.FilePath);

                _context.TepChungTu.Remove(checkDoc);
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

        public async Task<DocumentType> GetDocById(int docId)
        {
            var getDoc = await _context.TepChungTu.Where(x => x.Id == docId).FirstOrDefaultAsync();

            return new DocumentType()
            {
                MaDieuPhoi = getDoc.MaDieuPhoi,
                TenChungTu = getDoc.TenChungTu,
                LoaiChungTu = getDoc.LoaiChungTu,
                GhiChu = getDoc.GhiChu,
                TrangThai = getDoc.TrangThai,
            };
        }

        public async Task<BoolActionResult> UpdateDoc(int docId, DocumentType request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var getDoc = await _context.TepChungTu.Where(x => x.Id == docId).FirstOrDefaultAsync();

                if (getDoc == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "File chứng từ không tồn tại" };
                }

                if (request.fileImage != null)
                {
                    var image = await _context.Attachment.Where(x => x.Id == getDoc.MaHinhAnh).FirstOrDefaultAsync();
                    if (image == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Hình ảnh không tồn tại" };
                    }
                    _context.Remove(image);
                    await _common.DeleteFileAsync(image.FileName, image.FilePath);

                    var uploadfile = await UploadFile(new UploadImagesHandling()
                    {
                        file = request.fileImage,
                        handlingId = getDoc.MaDieuPhoi,
                        transportId = _context.DieuPhoi.Where(x => x.Id == getDoc.MaDieuPhoi).Select(x => x.MaVanDon).FirstOrDefault()
                    });

                    if (uploadfile.isSuccess == false)
                    {
                        return uploadfile;
                    }
                    else
                    {
                        getDoc.MaHinhAnh = long.Parse(uploadfile.DataReturn.Trim());
                    }
                }

                var checkDocType = await _context.LoaiChungTu.Where(x => x.MaLoaiChungTu == request.LoaiChungTu).FirstOrDefaultAsync();
                if (checkDocType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại chứng từ không tồn tại" };
                }

                getDoc.TenChungTu = request.TenChungTu;
                getDoc.GhiChu = request.GhiChu;
                getDoc.LoaiChungTu = request.LoaiChungTu;
                getDoc.TrangThai = request.TrangThai;
                getDoc.UpdatedTime = DateTime.Now;
                getDoc.Updater = tempData.UserName;

                _context.Update(getDoc);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thông tin chứng từ thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật thông tin chứng từ thất bại" };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> SendMailToSuppliers(GetIdHandling handlingIds)
        {
            try
            {
                var getlistHandling = await _context.DieuPhoi.Where(x => handlingIds.Ids.Contains(x.Id) && x.DonViVanTai != null && x.TrangThai == 27).ToListAsync();

                var listSup = new List<string>();
                foreach (var item in getlistHandling)
                {
                    if (!listSup.Contains(item.DonViVanTai))
                    {
                        listSup.Add(item.DonViVanTai);
                    }
                }

                foreach (var itemSup in listSup)
                {
                    var filterHandling = getlistHandling.Where(x => x.DonViVanTai == itemSup).ToList();
                    var stringhtml = "<html><body>" +
                        "<h3> <strong>Kính gửi Nhà cung cấp,<br> Vận tải TBS Logistics gửi Danh sách vận đơn cần vận chuyển ngày " + DateTime.Now.ToString("yyyy/MM/dd") + "</strong></h3>" +
                        "<table style='border-collapse:collapse; text-align:center;'>";
                    stringhtml += "<tr>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Mã Chuyến</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Booking No</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Phân Loại Vận Đơn</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Điểm Đóng Hàng</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Điểm Hạ Hàng</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Điểm Lấy rỗng</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Điểm Trả rỗng</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Loại Phương Tiện</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Phương Thức Vận Chuyển</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Mã Container</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Khối Lượng</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Số Kiện</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Thể tích</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Thời Gian Hạn Lệnh</th>" +
                        "<th style='font-weight:bold;border-style:solid; border-width:thin; padding: 5px;background-color:#99ccff'>Thời Gian CUT OFF</th>" +
                        "</tr>";

                    foreach (var item in filterHandling)
                    {
                        var getTransport = await _context.VanDon.Where(x => x.MaVanDon == item.MaVanDon).FirstOrDefaultAsync();
                        stringhtml += "<tr>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.MaChuyen + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + getTransport.MaVanDonKh + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + (getTransport.LoaiVanDon == "xuat" ? "Xuất" : "Nhập") + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + _context.DiaDiem.Where(y => y.MaDiaDiem == getTransport.DiemDau).Select(x => x.TenDiaDiem).FirstOrDefault() + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + _context.DiaDiem.Where(y => y.MaDiaDiem == getTransport.DiemCuoi).Select(x => x.TenDiaDiem).FirstOrDefault() + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + (item.DiemLayRong == null ? "" : _context.DiaDiem.Where(y => y.MaDiaDiem == item.DiemLayRong).Select(x => x.TenDiaDiem).FirstOrDefault()) + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + (item.DiemTraRong == null ? "" : _context.DiaDiem.Where(y => y.MaDiaDiem == item.DiemTraRong).Select(x => x.TenDiaDiem).FirstOrDefault()) + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.MaLoaiPhuongTien + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + getTransport.MaPtvc + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.ContNo + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.KhoiLuong + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.SoKien + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + item.TheTich + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + (getTransport.ThoiGianHanLenh == null ? "" : getTransport.ThoiGianHanLenh.Value.ToString("yyyy-MM-dd HH:mm:ss")) + "</td>" +
                        "<td style='border-style:solid; border-width:thin; padding: 5px;'>" + (getTransport.ThoiGianHaCang == null ? "" : getTransport.ThoiGianHaCang.Value.ToString("yyyy-MM-dd HH:mm:ss")) + "</td>" +
                        "</tr>";
                    }

                    stringhtml += "</table></body></html>";

                    var getSup = await _context.KhachHang.Where(x => x.MaKh == itemSup).FirstOrDefaultAsync();
                    await _common.SendEmailAsync(getSup.Email, "Danh Sách Chuyến Chờ Vận Chuyển", stringhtml);
                }
                return new BoolActionResult { isSuccess = true, Message = "Đã gửi mail cho đơn vị vận tải" };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<BoolActionResult> CreateDoc(DocumentType request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var checkHandling = await _context.DieuPhoi.Where(x => x.Id == request.MaDieuPhoi).FirstOrDefaultAsync();

                if (checkHandling == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã điều phối không tồn tại" };
                }

                var checkDocType = await _context.LoaiChungTu.Where(x => x.MaLoaiChungTu == request.LoaiChungTu).FirstOrDefaultAsync();
                if (checkDocType == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Loại chứng từ không tồn tại" };
                }

                if (request.fileImage == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được bỏ trống file hình ảnh" };
                }

                var uploadFile = await UploadFile(new UploadImagesHandling()
                {
                    handlingId = checkHandling.Id,
                    transportId = checkHandling.MaVanDon,
                    file = request.fileImage,
                });

                if (uploadFile.isSuccess == false)
                {
                    return uploadFile;
                }

                await _context.TepChungTu.AddAsync(new TepChungTu()
                {
                    MaDieuPhoi = request.MaDieuPhoi,
                    TenChungTu = "",
                    MaHinhAnh = long.Parse(uploadFile.DataReturn.Trim()),
                    LoaiChungTu = request.LoaiChungTu,
                    CreatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                    GhiChu = request.GhiChu,
                    TrangThai = 1,
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    return new BoolActionResult { isSuccess = true, Message = "Thêm mới chứng từ thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thêm mới chứng từ thất bại" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        private async Task<BoolActionResult> UploadFile(UploadImagesHandling request)
        {
            if (request.file == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "Không có file nào" };
            }
            var PathFolder = $"Transport/{request.transportId}/{request.handlingId}";

            var originalFileName = ContentDispositionHeaderValue.Parse(request.file.ContentDisposition).FileName.Trim('"');
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
                FileSize = request.file.Length,
                FileType = Path.GetExtension(fileName),
                FolderName = "Transport",
                UploadedTime = DateTime.Now,
                Creator = tempData.UserName,
            };

            var add = await _context.Attachment.AddAsync(attachment);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                await _common.SaveFileAsync(request.file.OpenReadStream(), fileName, PathFolder);
                await _common.Log("BillOfLading ", "UserId: " + tempData.UserName + " upload file with data: " + JsonSerializer.Serialize(request));
                return new BoolActionResult { isSuccess = true, DataReturn = add.Entity.Id.ToString() };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Tải file lên không thành công" };
            }
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

        private async Task<BoolActionResult> LogDriverChange(string maChuyen, string driverId, string vehicleId)
        {
            try
            {
                var listStatus = new List<int>(new int[] { 21, 31, 38 });
                var getListHandling = await _context.DieuPhoi.Where(x => x.MaChuyen == maChuyen && !listStatus.Contains(x.TrangThai)).ToListAsync();

                foreach (var item in getListHandling)
                {
                    var dataMove = await _context.TaiXeTheoChang.Where(x => x.MaDieuPhoi == item.Id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                    if (dataMove == null)
                    {
                        await _context.TaiXeTheoChang.AddAsync(new TaiXeTheoChang()
                        {
                            MaDieuPhoi = item.Id,
                            MaSoXe = vehicleId,
                            MaTaiXe = driverId,
                            SoChan = 1,
                            TrangThaiTheoChang = item.TrangThai,
                        });
                    }
                    else if (dataMove.MaTaiXe != driverId || dataMove.MaSoXe != vehicleId)
                    {
                        await _context.TaiXeTheoChang.AddAsync(new TaiXeTheoChang()
                        {
                            MaDieuPhoi = item.Id,
                            MaSoXe = vehicleId,
                            MaTaiXe = driverId,
                            SoChan = dataMove == null ? 1 : dataMove.SoChan + 1,
                            TrangThaiTheoChang = item.TrangThai,
                        });
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false };
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<GetPriceListRequest> GetPriceTable(string MaKH, string accountId, int firstPlace, int secondPlace, int? emptyPlace, string MaDVT, string LoaiHangHoa, string LoaiPhuongTien, string MaPTVC)
        {
            var checkPriceTable = from bg in _context.BangGia
                                  join hd in _context.HopDongVaPhuLuc
                                  on bg.MaHopDong equals hd.MaHopDong
                                  where hd.MaKh == MaKH
                                  && bg.TrangThai == 4
                                  && bg.MaAccount == accountId
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
            (((x.DiemDau == getFirstPlace.MaDiaDiem || x.DiemDau == getFirstPlace.DiaDiemCha)
            && (x.DiemCuoi == getSecondPlace.MaDiaDiem || x.DiemCuoi == getSecondPlace.DiaDiemCha))
            ||
            ((x.DiemDau == getSecondPlace.MaDiaDiem || x.DiemDau == getSecondPlace.DiaDiemCha)
            && (x.DiemCuoi == getFirstPlace.MaDiaDiem || x.DiemCuoi == getFirstPlace.DiaDiemCha)))
            && (x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem) || x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)));

            var dataPriceTable = await checkPriceTable.ToListAsync();

            if (dataPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                var data = dataPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem));

                if (data.Count() == 1)
                {
                    return data.Select(x => new GetPriceListRequest()
                    {
                        ID = x.Id,
                        DonGia = x.DonGia,
                        MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                        MaLoaiHangHoa = x.MaLoaiHangHoa,
                        MaLoaiDoiTac = x.MaLoaiDoiTac,
                        MaDVT = x.MaDvt,
                        MaPTVC = x.MaPtvc
                    }).FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }

            if (dataPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                var data = dataPriceTable.Where(x => x.DiemDau == getFirstPlace.MaDiaDiem && x.DiemCuoi == getSecondPlace.MaDiaDiem && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha));

                if (data.Count() == 1)
                {
                    return data.Select(x => new GetPriceListRequest()
                    {
                        ID = x.Id,
                        DonGia = x.DonGia,
                        MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                        MaLoaiHangHoa = x.MaLoaiHangHoa,
                        MaLoaiDoiTac = x.MaLoaiDoiTac,
                        MaDVT = x.MaDvt,
                        MaPTVC = x.MaPtvc
                    }).FirstOrDefault();
                }
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.DiaDiemCha
            && x.DiemCuoi == getSecondPlace.MaDiaDiem
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.DiaDiemCha
                && x.DiemCuoi == getSecondPlace.MaDiaDiem
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).ToList();
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.MaDiaDiem
            && x.DiemCuoi == getSecondPlace.DiaDiemCha
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.MaDiaDiem
                && x.DiemCuoi == getSecondPlace.DiaDiemCha
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).ToList();
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.DiaDiemCha
            && x.DiemCuoi == getSecondPlace.DiaDiemCha
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.DiaDiemCha
                && x.DiemCuoi == getSecondPlace.DiaDiemCha
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.MaDiaDiem)).ToList();
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.DiaDiemCha
            && x.DiemCuoi == getSecondPlace.MaDiaDiem
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.DiaDiemCha
                && x.DiemCuoi == getSecondPlace.MaDiaDiem
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).ToList();
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.MaDiaDiem
            && x.DiemCuoi == getSecondPlace.DiaDiemCha
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.MaDiaDiem
                && x.DiemCuoi == getSecondPlace.DiaDiemCha
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).ToList();
            }

            if (dataPriceTable.Where(x =>
            x.DiemDau == getFirstPlace.DiaDiemCha
            && x.DiemCuoi == getSecondPlace.DiaDiemCha
            && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).Count() > 0)
            {
                dataPriceTable = dataPriceTable.Where(x =>
                x.DiemDau == getFirstPlace.DiaDiemCha
                && x.DiemCuoi == getSecondPlace.DiaDiemCha
                && x.DiemLayTraRong == (getEmptyPlace == null ? null : getEmptyPlace.DiaDiemCha)).ToList();
            }

            if (dataPriceTable.Count() == 1)
            {
                return dataPriceTable.Select(x => new GetPriceListRequest()
                {
                    ID = x.Id,
                    DonGia = x.DonGia,
                    MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                    MaLoaiHangHoa = x.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.MaLoaiDoiTac,
                    MaDVT = x.MaDvt,
                    MaPTVC = x.MaPtvc
                }).FirstOrDefault();
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
    }
}