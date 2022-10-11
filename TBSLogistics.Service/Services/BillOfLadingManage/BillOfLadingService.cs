using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.ProductServiceModel;
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


        public async Task<BoolActionResult> CreateBillOfLading(CreateBillOfLadingRequest request)
        {
            try
            {
                var checkExists = await _TMSContext.VanDon.Where(x => x.MaVanDon == request.MaVanDon).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã vận đơn này đã tồn tại" };
                }

                string ErrorValidate = await ValiateBillOfLading(request.MaVanDon, request.MaKh, request.IDBangGia, request.MaSoXe, request.MaTaiXe, request.MaRomooc, request.MaDonHang, request.ClpNo, request.ContNo, request.SealNP, request.SealHq, request.TrongLuong, request.TheTich, request.DiemLayHang, request.DiemNhapHang, request.DiemGioHang, request.DiemTraRong, request.ThoiGianLayRong, request.ThoiGianHaCong, request.ThoiGianKeoCong, request.ThoiGianHanLech, request.ThoiGianCoMat, request.ThoiGianTraRong, request.HangTau, request.Tau, request.CangChuyenTai, request.CangDich, request.TrangThai, request.NgayTaoDon);
                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                await _TMSContext.VanDon.AddAsync(new VanDon()
                {
                    MaVanDon = request.MaVanDon,
                    //MaKh = request.MaKh,
                    //IdbangGia =request.IDBangGia,
                    //MaSoXe = request.MaSoXe,
                    //MaTaiXe = request.MaTaiXe,
                    //MaRomooc = request.MaRomooc,
                    //MaDonHang = request.MaDonHang,
                    //ClpNo = request.ClpNo,
                    //ContNo = request.ContNo,
                    //SealNp = request. SealNP,
                    //SealHq = request.SealHq,
                    //TrongLuong = request.TrongLuong,            
                    //TheTich = request.TheTich,                 
                    //DiemLayHang = request.DiemLayHang,
                    //DiemNhapHang = request.DiemNhapHang,
                    //DiemGioHang = request.DiemGioHang,
                    //DiemTraRong = request.DiemTraRong,
                    //ThoiGianLayRong= request.ThoiGianLayRong,
                    //ThoiGianHaCong= request.ThoiGianHaCong,
                    //ThoiGianKeoCong= request.ThoiGianKeoCong,
                    //ThoiGianHanLech = request.ThoiGianHanLech,
                    //ThoiGianCoMat = request.ThoiGianCoMat,//.Value.Date
                    //ThoiGianCatMang = request.ThoiGianCatMang,
                    //ThoiGianTraRong = request.ThoiGianTraRong,
                    HangTau = request.HangTau,
                    Tau = request.Tau,
                    CangChuyenTai = request.CangChuyenTai,
                    CangDich = request.CangDich,
                    TrangThai = request.TrangThai,
                    NgayTaoDon = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now,
                });

                var result = await _TMSContext.SaveChangesAsync();

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
                var getBillOfLading = await _TMSContext.VanDon.Where(x => x.MaVanDon == billOfLadingId && x.TrangThai!=1).FirstOrDefaultAsync();

                if (getBillOfLading == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = billOfLadingId +"Vận đơn này không tồn tại hoặc không phải trảng thái tạo mới" };
                }

                //getBillOfLading.MaKh = request.MaKh;
                //getBillOfLading.IdbangGia = request.IDBangGia;
                //getBillOfLading.MaSoXe = request.MaSoXe;
                //getBillOfLading.MaTaiXe = request.MaTaiXe;
                //getBillOfLading.MaRomooc = request.MaRomooc;
                //getBillOfLading.MaDonHang = request.MaDonHang;
                //getBillOfLading.ClpNo = request.ClpNo;
                //getBillOfLading.ContNo = request.ContNo;
                //getBillOfLading.SealNp = request.SealNP;
                //getBillOfLading.SealHq = request.SealHq;
                //getBillOfLading.TrongLuong = request.TrongLuong;
                //getBillOfLading.TheTich = request.TheTich;
                //getBillOfLading.DiemLayHang = request.DiemLayHang;
                //getBillOfLading.DiemNhapHang = request.DiemNhapHang;
                //getBillOfLading.DiemGioHang = request.DiemGioHang;
                //getBillOfLading.DiemTraRong = request.DiemTraRong;
                //getBillOfLading.ThoiGianLayRong = request.ThoiGianTraRong;
                //getBillOfLading.ThoiGianHaCong = request.ThoiGianHaCong;
                //getBillOfLading.ThoiGianKeoCong = request.ThoiGianKeoCong;
                //getBillOfLading.ThoiGianHanLech = request.ThoiGianHanLech;
                //getBillOfLading.ThoiGianCoMat = request.ThoiGianCoMat;
                //getBillOfLading.ThoiGianCatMang = request.ThoiGianCatMang;
                //getBillOfLading.ThoiGianTraRong = request.ThoiGianTraRong;
                getBillOfLading.HangTau = request.HangTau;
                getBillOfLading.Tau = request.Tau;
                getBillOfLading.CangChuyenTai = request.CangChuyenTai;
                getBillOfLading.CangDich = request.CangDich;
                getBillOfLading.TrangThai = request.TrangThai;
                getBillOfLading.NgayTaoDon = DateTime.Now;
                getBillOfLading.UpdatedTime = DateTime.Now;
                getBillOfLading.CreatedTime = DateTime.Now;

             _TMSContext.Update(getBillOfLading);

                var result = await _TMSContext.SaveChangesAsync();

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
            var getBillOfLading = await _TMSContext.VanDon.Where(x => x.MaVanDon == billOfLadingId).Select(x => new GetBillOfLadingRequest()
            {

                MaVanDon = x.MaVanDon,
                //MaKh = x.MaKh,
                //IDBangGia = x.IdbangGia,
                //MaSoXe = x.MaSoXe,
                //MaTaiXe = x.MaTaiXe,
                //MaRomooc = x.MaRomooc,
                //MaDonHang = x.MaDonHang,
                //ClpNo = x.ClpNo,
                //ContNo = x.ContNo,
                //SealNP = x.SealNp,
                //SealHq = x.SealHq,
                //TrongLuong = x.TrongLuong,
                //TheTich = x.TheTich,
                //DiemLayHang = x.DiemLayHang,
                //DiemNhapHang = x.DiemNhapHang,
                //DiemGioHang = x.DiemGioHang,
                //DiemTraRong = x.DiemTraRong,
                //ThoiGianLayRong = x.ThoiGianLayRong.Value.Date,
                //ThoiGianHaCong = x.ThoiGianHaCong,
                //ThoiGianKeoCong = x.ThoiGianKeoCong.Value.Date,
                //ThoiGianHanLech = x.ThoiGianHanLech,
                //ThoiGianCoMat = x.ThoiGianCoMat,//.Value.Date
                //ThoiGianCatMang = x.ThoiGianCatMang,
                //ThoiGianTraRong = x.ThoiGianTraRong,
                HangTau = x.HangTau,
                Tau = x.Tau,
                CangChuyenTai = x.CangChuyenTai,
                CangDich = x.CangDich,
                TrangThai = x.TrangThai,
                NgayTaoDon = x.NgayTaoDon,
                UpdatedTime = x.UpdatedTime,
                CreatedTime = x.CreatedTime
            }).FirstOrDefaultAsync();

            return getBillOfLading;
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
