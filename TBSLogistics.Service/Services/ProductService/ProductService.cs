using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.ProductServiceManage
{
    public class ProductService : IProduct
    {
        private readonly TMSContext _TMSContext;
        private readonly ICommon _common;
        private readonly ProductService _productService;
        public ProductService(TMSContext TMSContext, ICommon common)
        {

            _TMSContext = TMSContext;
            _common = common;
        }
        public async Task<BoolActionResult> CreateProductService(List<CreateProductServiceRequest> request)
        {
            try
            {
                foreach (var i in request)
                {
                    string ErrorValidate = await ValiateProductService(i.MaHopDong, i.MaPTVC, i.MaCungDuong, i.MaLoaiPhuongTien, i.DonGia, i.MaDVT, i.MaLoaiHangHoa, i.MaLoaiHopDong, i.NgayHetHieuLuc);
                    if (ErrorValidate != "")
                    {
                        return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                    }

                    var a = from j in _TMSContext.HopDongVaPhuLuc
                            where j.MaHopDong == i.MaHopDong
                            select j.ThoiGianBatDau;                 
                    await _TMSContext.AddAsync(new BangGia()
                    {
                        // ngày hiệu lực phải bằng ngày kí trong bảng phụ lục hợp đồng join bảng lấy ra
                        MaHopDong = i.MaHopDong,
                        MaPtvc = i.MaPTVC,
                        MaCungDuong = i.MaCungDuong,
                        MaLoaiPhuongTien = i.MaLoaiPhuongTien,
                        DonGia = i.DonGia,
                        MaDvt = i.MaDVT,
                        MaLoaiHangHoa = i.MaLoaiHangHoa,
                        MaLoaiDoiTac = i.MaLoaiHopDong,
                        NgayApDung = a.FirstOrDefault(),
                        NgayHetHieuLuc = i.NgayHetHieuLuc,
                        TrangThai = 1,
                        UpdatedTime = DateTime.Now,
                        CreatedTime = DateTime.Now
                    });
                }
                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService with Id: ");
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới  thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới SPDV thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<BoolActionResult> EditProductServiceRequest(int id, EditProductServiceRequest request)
        {
            try
            {               // bổ sung trang thái =1 vào đk
                var checkExists = await _TMSContext.BangGia.Where(x => x.Id == id && x.MaHopDong == request.MaHopDong ).FirstOrDefaultAsync();
                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này không tồn tại " };
                }
                //string ValiateEdit = await ValiateEdit(request.MaHopDong, i.MaPTVC, i.MaCungDuong, i.MaLoaiPhuongTien, i.DonGia, i.MaDVT, i.MaLoaiHangHoa, i.MaLoaiHopDong, i.NgayApDung, i.NgayHetHieuLuc);
                //if (ErrorValidate != "")
                //{
                //    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                //
                checkExists.MaPtvc = request.MaPTVC;
                checkExists.MaCungDuong = request.MaHopDong;
                checkExists.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                checkExists.DonGia = request.DonGia;
                checkExists.MaDvt = request.MaDVT;
                checkExists.MaLoaiHangHoa = request.MaLoaiHangHoa;
                checkExists.MaLoaiDoiTac = request.MaLoaiHopDong;
                checkExists.NgayApDung = request.NgayApDung;
                checkExists.NgayHetHieuLuc = request.NgayHetHieuLuc;
                checkExists.UpdatedTime = DateTime.Now;
                var checkExists2 = await _TMSContext.BangGia.Where( x =>x.Id!= id && x.MaHopDong == request.MaHopDong && x.MaCungDuong== request.MaCungDuong && x.MaPtvc == request.MaPTVC && x.MaLoaiPhuongTien== request.MaLoaiPhuongTien ).FirstOrDefaultAsync();
                if (checkExists2 == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Bảng giá đã tồn tại hoặc  " };
                }
                _TMSContext.Update(checkExists);

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ContractManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + id);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật Bảng Giá thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật  Bảng Giá thất bại!" };
                }

            }
            catch (Exception ex)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }


        }
        public async Task<BoolActionResult> DeleteProductServiceRequest(int id, DeleteProductServiceRequest request)
        {
            var checkExists = await _TMSContext.BangGia.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (checkExists == null)
            {

                return new BoolActionResult { isSuccess = false, Message = "ID không tồn tại  " };
            }

            else
            {
                await _common.Log("ContractManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + id);
                return new BoolActionResult { isSuccess = true, Message = "Cập nhật Bảng Giá thành công!" };
            }
        }


            //public async Task<GetProductServiceRequest> GetProductServiceByIdRequest(string id)
            //{
            //    try
            //    {
            //        var getProductServiceById = await _TMSContext.SanPhamDichVu.Where(x => x.MaSpdv == id).FirstOrDefaultAsync();

            //        return new GetProductServiceRequest()
            //        {
            //            MaSPDV = getProductServiceById.MaSpdv,
            //            MaCungDuong = getProductServiceById.MaCungDuong,
            //            MaLoaiPhuongTien = getProductServiceById.MaLoaiPhuongTien,
            //            DonGia = getProductServiceById.DonGia,
            //            MaDVT = getProductServiceById.MaDvt,
            //            SoLuong = getProductServiceById.SoLuong,
            //            MaLoaiHangHoa = getProductServiceById.MaLoaiHangHoa,
            //            MaPTVC = getProductServiceById.MaPtvc,
            //            TrangThai = getProductServiceById.TrangThai,
            //            UpdatedTime = getProductServiceById.UpdatedTime,
            //            CreatedTime = getProductServiceById.CreatedTime
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
            //public async Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductService(PaginationFilter filter)
            //{
            //    try
            //    {
            //        var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            //        var listData = from pro in _TMSContext.SanPhamDichVu
            //                       select new { pro };

            //        if (!string.IsNullOrEmpty(filter.Keyword))
            //        {
            //            listData = listData.Where(x => x.pro.MaSpdv.Contains(filter.Keyword));
            //        }
            //        var totalCount = await listData.CountAsync();
            //        var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListProductServiceRequest()
            //        {
            //            MaSPDV = x.pro.MaSpdv,
            //            MaCungDuong = x.pro.MaCungDuong,
            //            MaLoaiPhuongTien = x.pro.MaLoaiPhuongTien,
            //            DonGia = x.pro.DonGia,
            //            MaDVT = x.pro.MaDvt,
            //            SoLuong = x.pro.SoLuong,
            //            MaLoaiHangHoa = x.pro.MaLoaiHangHoa,
            //            MaPTVC = x.pro.MaPtvc,
            //            TrangThai = x.pro.TrangThai,
            //            UpdatedTime = x.pro.UpdatedTime,
            //            CreatedTime = x.pro.CreatedTime

            //        }).ToListAsync();
            //        return new PagedResponseCustom<ListProductServiceRequest>()
            //        {
            //            dataResponse = pagedData,
            //            totalCount = totalCount,
            //            paginationFilter = validFilter
            //        };
            //    }
            //    catch (Exception ex)
            //    {
            //        throw;
            //    }
            //}
        private async Task<string> ValiateProductService(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, string MaLoaiHopDong, DateTime NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkMaHopDong = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == MaHopDong).FirstOrDefaultAsync();

            if (checkMaHopDong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Hợp đồng không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate  MaCungDuong
            var checkMaCungDuong = await _TMSContext.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

            if (checkMaCungDuong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Cung đường không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate LoaiPhuongTien
            var checkMaLoaiPhuongTien = await _TMSContext.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

            if (checkMaLoaiPhuongTien == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện không tồn tại \r\n" + System.Environment.NewLine;
            }

            //if (Regex.IsMatch(DonGia.ToString(), "^\\d*(\\.\\d+)?$", RegexOptions.IgnoreCase))
            //{
            //    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Không phải dạng số nguyên dương\r\n" + System.Environment.NewLine;
            //}
            if (DonGia < 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đơn giá không được để trong và phải lớn hơn 0  \r\n" + System.Environment.NewLine;
            }
            //validate MaDVT
            var checkMaDVT = await _TMSContext.DonViTinh.Where(x => x.MaDvt == MaDVT).FirstOrDefaultAsync();

            if (checkMaDVT == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Đơn VỊ Tính không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate LoaiHangHoa
            var checkMaLoaiHangHoa = await _TMSContext.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == MaLoaiHangHoa).FirstOrDefaultAsync();

            if (checkMaLoaiHangHoa == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Hàng Hóa không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaPTVC = await _TMSContext.PhuongThucVanChuyen.Where(x => x.MaPtvc == MaPTVC).FirstOrDefaultAsync();

            if (checkMaPTVC == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phương Thức Vận Chuyển không tồn tại \r\n" + System.Environment.NewLine;
            }
            //if (!Regex.IsMatch(TrangThai.ToString(), "^[0-3]$", RegexOptions.IgnoreCase))
            //{
            //    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - sai trạng thái\r\n0: hết hiệu lực\r\n1: mới \r\n2: đã duyệt\r\n3: xóa khỏi BG\"\r\n \r\n" + System.Environment.NewLine;
            //}

            //if (TrangThai.ToString().Length == 0)
            //{
            //    ErrorValidate += " Không được để trống";
            var NgayApDung = from j in _TMSContext.HopDongVaPhuLuc
                    where j.MaHopDong == MaHopDong
                    select j.ThoiGianBatDau;
            if (!Regex.IsMatch(NgayHetHieuLuc.ToString("dd/MM/yyyy"), "^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Sai định dạng ngày\r\n" + System.Environment.NewLine;
            }
            if (NgayApDung.FirstOrDefault() >= NgayHetHieuLuc)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Ngày Hết hạn không được có trước ngày áp dụng \r\n" + System.Environment.NewLine;
            }
            var checkDateApplication = await _TMSContext.HopDongVaPhuLuc.Where(x => x.ThoiGianBatDau <= NgayApDung.FirstOrDefault()).FirstOrDefaultAsync();

            if (checkDateApplication == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Ngày Áp dụng không được sớm hơn ngày kí hợp đồng  \r\n" + System.Environment.NewLine;
            }
            //}
            var checkExist = await _TMSContext.BangGia.Where(x => x.MaHopDong == MaHopDong && x.MaCungDuong == MaCungDuong && x.MaPtvc == MaPTVC && x.MaDvt == MaDVT && x.MaLoaiDoiTac == MaLoaiHopDong).FirstOrDefaultAsync();
            if (checkExist != null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đã tồn tại thêm mới thất bại \r\n Vui lòng tạo mới hoặc vào mục Update \r\n" + System.Environment.NewLine;
            }

            return ErrorValidate;

        }
        private async Task<string> ValiateEdit(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, string MaLoaiHopDong, DateTime NgayApDung, DateTime NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";
            var checkTrangThai = await _TMSContext.BangGia.Where(x => x.TrangThai == 1).FirstOrDefaultAsync();
            if (checkTrangThai == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Trạng thải phải là tạo mới \r\n" + System.Environment.NewLine;
            }
            if (NgayApDung >= NgayHetHieuLuc)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Ngày Áp dụng không được sớm hơn ngày kí hợp đồng  \r\n" + System.Environment.NewLine;
            }
            var checkMaCungDuong = await _TMSContext.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

            if (checkMaCungDuong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Cung đường không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkMaLoaiPhuongTien = await _TMSContext.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

            if (checkMaLoaiPhuongTien == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện không tồn tại \r\n" + System.Environment.NewLine;
            }

            if (DonGia < 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đơn giá không được để trong và phải lớn hơn 0  \r\n" + System.Environment.NewLine;
            }
            //validate MaDVT
            var checkMaDVT = await _TMSContext.DonViTinh.Where(x => x.MaDvt == MaDVT).FirstOrDefaultAsync();

            if (checkMaDVT == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Đơn VỊ Tính không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate LoaiHangHoa
            var checkMaLoaiHangHoa = await _TMSContext.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == MaLoaiHangHoa).FirstOrDefaultAsync();

            if (checkMaLoaiHangHoa == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Hàng Hóa không tồn tại \r\n" + System.Environment.NewLine;
            }

            var checkMaPTVC = await _TMSContext.PhuongThucVanChuyen.Where(x => x.MaPtvc == MaPTVC).FirstOrDefaultAsync();

            if (checkMaPTVC == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phương Thức Vận Chuyển không tồn tại \r\n" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(NgayApDung.ToString("dd/MM/yyyy"), "^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Sai định dạng ngày\r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(NgayHetHieuLuc.ToString("dd/MM/yyyy"), "^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Sai định dạng ngày\r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;

        }
    }
}