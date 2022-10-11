using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public class BillOfLadingService : IBillOfLading
    {
        private readonly ICommon _common;
        private readonly TMSContext _TMSContext;

        public BillOfLadingService(ICommon common, TMSContext context)
        {
            _common = common;
            _TMSContext = context;
        }

        public async Task<LoadDataTransPort> getListRoadBillOfLading(string RoadId)
        {
            var getListRoad = from cd in _context.CungDuong
                              join bg in _context.BangGia
                              on cd.MaCungDuong equals bg.MaCungDuong
                              join hd in _context.HopDongVaPhuLuc
                              on bg.MaHopDong equals hd.MaHopDong
                              join kh in _context.KhachHang
                              on hd.MaKh equals kh.MaKh
                              where
                                 cd.TrangThai == 1 &&
                                 bg.NgayApDung.Date <= DateTime.Now.Date
                                 && bg.NgayHetHieuLuc.Date >= DateTime.Now.Date
                                 && bg.TrangThai == 4
                                 && cd.MaCungDuong == RoadId
                              orderby bg.Id descending
                              select new { cd, bg, hd, kh };

            var ls = getListRoad.ToList();

            var gr = from t in getListRoad
                     group t by new { t.bg.MaCungDuong, t.bg.MaDvt, t.bg.MaLoaiHangHoa, t.bg.MaLoaiPhuongTien, t.bg.MaPtvc, t.bg.MaLoaiDoiTac }
                    into g
                     select new
                     {
                         MaCungDuong = g.Key.MaCungDuong,
                         MaDvt = g.Key.MaDvt,
                         MaLoaiHangHoa = g.Key.MaLoaiHangHoa,
                         MaLoaiPhuongTien = g.Key.MaLoaiPhuongTien,
                         MaPtvc = g.Key.MaPtvc,
                         MaLoaiDoiTac = g.Key.MaLoaiDoiTac,
                         Id = (from t2 in g select t2.bg.Id).Max(),
                     };

            getListRoad = getListRoad.Where(x => gr.Select(y => y.Id).Contains(x.bg.Id));



            var result = new LoadDataTransPort()
            {
                ListNhaPhanPhoi = await getListRoad.Where(x => x.bg.MaLoaiDoiTac == "NCC").Select(x => new NhaPhanPhoiSelect()
                {
                    MaNPP = x.kh.MaKh,
                    TenNPP = x.kh.TenKh
                }).ToListAsync(),
                ListKhachHang = await getListRoad.Where(x => x.bg.MaLoaiDoiTac == "KH").Select(x => new KhachHangSelect()
                {
                    MaKH = x.kh.MaKh,
                    TenKH = x.kh.TenKh
                }).ToListAsync(),
                BangGiaVanDon = await getListRoad.Select(x => new BangGiaVanDon
                {
                    MaNPP = x.kh.MaKh,
                    PTVC = x.bg.MaPtvc,
                    DVT = x.bg.MaDvt,
                    PTVanChuyen = x.bg.MaLoaiPhuongTien,
                    LoaiHangHoa = x.bg.MaLoaiHangHoa,
                    Price = x.bg.DonGia,
                    MaCungDuong = x.bg.MaCungDuong
                }).ToListAsync()
            };
            return result;
        }
        public async Task<BoolActionResult> DeleteBillOfLading(DeleteBillOfLading request)
        {

            var checkExists = await _TMSContext.VanDon.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();
            if (checkExists == null)
            {

                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.MaVanDon + "không tồn tại  " };
            }
            if (checkExists.TrangThai != 1)
            {

                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.MaVanDon + " Vận Đơn phải là mới chưa được duyệt  " };
            }
            checkExists.TrangThai = 3;
            _TMSContext.Update(checkExists);
            var result = await _TMSContext.SaveChangesAsync();
            if (result > 0)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + request.MaVanDon);
                return new BoolActionResult { isSuccess = true, Message = "Xóa Vận Đơn  thành công!" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Xóa Vận Đơn  thất bại!" };
            }
        }

        public async Task<PagedResponseCustom<GetBillOfLadingRequest>> GetListBillOfLading(PaginationFilter filter)
        {
            try
            {

                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from bill in _TMSContext.VanDon                              
                               select new { bill };
                //if (trangthai == 7)
                //{
                //    listData = from bill in _TMSContext.VanDon
                //               where bill.TrangThai == 2 || bill.TrangThai == 1
                //               select new { bill };
                //}
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bill.TrangThai.ToString().Contains(filter.Keyword));
                }
                //if (!string.IsNullOrEmpty(filter.Keyword))
                //{
                //    listData = listData.Where(x => x.bill.MaKh.ToString().Contains(filter.Keyword));
                //}
                var totalCount = await listData.CountAsync();
                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetBillOfLadingRequest()
                {
                    
                    MaVanDon = x.bill.MaVanDon,
                    //MaKh = x.bill.MaKh,
                    //IDBangGia = x.bill.IdbangGia,
                    //MaSoXe = x.bill.MaSoXe,
                    //MaTaiXe = x.bill.MaTaiXe,
                    //MaRomooc = x.bill.MaRomooc,
                    //MaDonHang = x.bill.MaDonHang,
                    //ClpNo = x.bill.ClpNo,
                    //ContNo = x.bill.ContNo,
                    //SealNP = x.bill.SealNp,
                    //SealHq = x.bill.SealHq,
                    //TrongLuong = x.bill.TrongLuong,
                    //TheTich = x.bill.TheTich,
                    //DiemLayHang = x.bill.DiemLayHang,
                    //DiemNhapHang = x.bill.DiemNhapHang,
                    //DiemGioHang = x.bill.DiemGioHang,
                    //DiemTraRong = x.bill.DiemTraRong,
                    //ThoiGianLayRong = x.bill.ThoiGianLayRong.Value.Date,
                    //ThoiGianHaCong = x.bill.ThoiGianHaCong,
                    //ThoiGianKeoCong = x.bill.ThoiGianKeoCong.Value.Date,
                    //ThoiGianHanLech = x.bill.ThoiGianHanLech,
                    //ThoiGianCoMat = x.bill.ThoiGianCoMat,//.Value.Date
                    //ThoiGianCatMang = x.bill.ThoiGianCatMang,
                    //ThoiGianTraRong = x.bill.ThoiGianTraRong,
                    HangTau = x.bill.HangTau,
                    Tau = x.bill.Tau,
                    CangChuyenTai = x.bill.CangChuyenTai,
                    CangDich = x.bill.CangDich,
                    TrangThai = x.bill.TrangThai,
                    NgayTaoDon = x.bill.NgayTaoDon,
                    UpdatedTime = x.bill.UpdatedTime,
                    CreatedTime = x.bill.CreatedTime

                }).ToListAsync();
                return new PagedResponseCustom<GetBillOfLadingRequest>()
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
        private async Task<string> ValiateBillOfLading(string MaVanDon, string MaKH, int IDBangGia, string MaSoXe, string MaTaiXe, string MaRomooc, string MaDonHang, string CLP_No, string Cont_No, string SEAL_NP, string SEAL_HQ, float TrongLuong, float TheTich, int DiemLayHang, int DiemNhapHang, int DiemGioHang, int DiemTraRong, DateTime ThoiGianLayRong, DateTime ThoiGianHaCong, DateTime ThoiGianKeoCong,int ThoiGianHanLech, DateTime ThoiGianCoMat, DateTime ThoiGianTraRong, string HangTau, string Tau, string CangChuyenTai, string CangDich, int trangthai, DateTime NgayTaoDon, string ErrorRow = "")
        {
            string ErrorValidate = "";
            if (MaVanDon.Length != 8)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Vận đơn phải dài 8 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaVanDon, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Vận đơn không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaVanDon, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " - Mã vận đơn phải viết hoa   \r\n";
            }
            var checkMaKH = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaKh == MaKH).FirstOrDefaultAsync();

            if (checkMaKH == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Khách Hàng: " + MaKH + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkIdBangGia = await _TMSContext.BangGia.Where(x => x.Id == IDBangGia).FirstOrDefaultAsync();

            if (checkIdBangGia == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - ID bảng giá : " + IDBangGia + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaSoXe = await _TMSContext.XeVanChuyen.Where(x => x.MaSoXe == MaSoXe).FirstOrDefaultAsync();

            if (checkMaSoXe == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã số xe  : " + IDBangGia + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaTaiXe = await _TMSContext.TaiXe.Where(x => x.MaTaiXe == MaTaiXe).FirstOrDefaultAsync();

            if (checkMaTaiXe == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã tài xế  : " + MaTaiXe + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaRomooc = await _TMSContext.Romooc.Where(x => x.MaRomooc == MaRomooc).FirstOrDefaultAsync();

            if (checkMaRomooc == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Romooc  : " + MaRomooc + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            if (MaDonHang.Length <= 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Vận đơn phải ít hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaDonHang, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã đơn hàng không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaDonHang, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " -  Mã đơn hàng phải viết hoa   \r\n";
            }
            if (CLP_No.Length <= 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No phải ít hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(CLP_No, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(CLP_No, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " - CLP_No phải viết hoa   \r\n";
            }
            if (Cont_No.Length <= 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No phải ít hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(Cont_No, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(Cont_No, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " - CLP_No phải viết hoa   \r\n";
            }
            if (SEAL_NP.Length <= 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No phải ít hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(SEAL_NP, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - CLP_No không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(SEAL_NP, "^(?![_.])(?![_.])(?!.*[_.]{2})[A-Z0-9]+(?<![_.])$"))
            {
                ErrorValidate += " - CLP_No phải viết hoa   \r\n";
            }


            return ErrorValidate;
        }
        }
}
