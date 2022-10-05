using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
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
                List<string> IdList = new List<string>();
                List<string> IdListFail = new List<string>();
          
                foreach (var i in request)
                {
                    string ErrorValidate = await ValiateProductService(i.MaHopDong, i.MaPTVC, i.MaCungDuong, i.MaLoaiPhuongTien, i.DonGia, i.MaDVT, i.MaLoaiHangHoa, i.MaLoaiHopDong, i.NgayHetHieuLuc);
                    if (ErrorValidate != "")
                    {
                        IdListFail.Add(i.MaHopDong + " \r\n" + ErrorValidate + " \r\n");
                        continue;                      
                    }
                    var checkExists2 = await _TMSContext.BangGia.Where(x => x.MaHopDong == i.MaHopDong && x.MaCungDuong == i.MaCungDuong && x.MaPtvc == i.MaPTVC && x.MaLoaiPhuongTien == i.MaLoaiPhuongTien).FirstOrDefaultAsync();
                    if (checkExists2 != null)
                    {
                        IdListFail.Add(" hợp đồng này đã tồn tại" + " \r\n" +i.MaHopDong+ i.MaPTVC+ i.MaCungDuong+ i.MaLoaiPhuongTien);
                        continue;
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
                    
                    var result = await _TMSContext.SaveChangesAsync();
                    if (result > 0)
                    {
                        await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService with Id: ");                                            
                        IdList.Add(" Tạo mới  thành công! " + i.MaHopDong + "-" + i.MaPTVC + "-" + i.MaCungDuong + "-" + i.MaLoaiPhuongTien+ " \r\n");
                        continue;
                    }
                    else
                    {
                        IdListFail.Add(" Tạo mới thất bại ! " + i.MaHopDong +"-"+ i.MaPTVC + "-" + i.MaCungDuong +"-"+  i.MaLoaiPhuongTien + "-" + "lỗi SQL" +" \r\n");
                        continue;
                    }
                }
                if( IdList.Count > 0)
                {
                    string a = string.Join(",", IdList);
                    string b = string.Join(",", IdListFail);
                    await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService with Id: ");
                    return new BoolActionResult { isSuccess = true, Message = a + " \r\n" + b + " \r\n" };
                }
                else
                {
                    string b = string.Join(",", IdListFail);
                    return new BoolActionResult { isSuccess = false, Message = b };
                }

            }
            catch (Exception ex)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<BoolActionResult> EditProductServiceRequest( EditProductServiceRequest request)
        {
            try
            {               // bổ sung trang thái =1 vào đk
                var checkExists = await _TMSContext.BangGia.Where(x => x.Id == request.id && x.MaHopDong == request.MaHopDong ).FirstOrDefaultAsync();
                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này không tồn tại " };
                }
                string ErrorValidate = await ValiateEdit(request.MaHopDong, request.MaPTVC, request.MaCungDuong, request.MaLoaiPhuongTien, request.DonGia, request.MaDVT, request.MaLoaiHangHoa, request.MaLoaiHopDong, request.NgayHetHieuLuc);
                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                var checkExists2 = await _TMSContext.BangGia.Where(x => x.Id != request.id && x.MaHopDong == request.MaHopDong && x.MaCungDuong == request.MaCungDuong && x.MaPtvc == request.MaPTVC && x.MaLoaiPhuongTien == request.MaLoaiPhuongTien).FirstOrDefaultAsync();
                if (checkExists2 != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Bảng giá đã tồn tại   " };
                }
                var a = from j in _TMSContext.HopDongVaPhuLuc
                        where j.MaHopDong == request.MaHopDong
                        select j.ThoiGianBatDau;

                checkExists.MaPtvc = request.MaPTVC;
                checkExists.MaCungDuong = request.MaCungDuong;
                checkExists.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                checkExists.DonGia = request.DonGia;
                checkExists.MaDvt = request.MaDVT;
                checkExists.MaLoaiHangHoa = request.MaLoaiHangHoa;
                checkExists.MaLoaiDoiTac = request.MaLoaiHopDong;
                checkExists.NgayApDung = a.FirstOrDefault();
                checkExists.NgayHetHieuLuc = request.NgayHetHieuLuc;
                checkExists.UpdatedTime = DateTime.Now;
                
                _TMSContext.Update(checkExists);

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + request.id);
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
        public async Task<BoolActionResult> DeleteProductServiceRequest(DeleteProductServiceRequest request)
        {

            var checkExists = await _TMSContext.BangGia.Where(x => x.Id == request.id).FirstOrDefaultAsync();
            if (checkExists == null)
            {

                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.id+"không tồn tại  " };
            }
            if (checkExists.TrangThai != 1)
            {

                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.id + " Bảng Giá phải là bảng giá mới chưa được duyệt  " };
            }
            checkExists.TrangThai = 3;
            _TMSContext.Update(checkExists);
            var result = await _TMSContext.SaveChangesAsync();
            if (result > 0)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + request.id);
                return new BoolActionResult { isSuccess = true, Message = "Xóa Bảng Giá thành công!" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Xóa Bảng Giá thất bại!" };
            }
        }
        public async Task<BoolActionResult> ApproveProductServiceRequestById(List<ApproveProductServiceRequestById>  request)
        {
            List<string> IdList = new List<string>();
            List<string> IdListFail = new List<string>();
                    
            try
            {
                foreach (var item in request)
                {
                    var checkExists = await _TMSContext.BangGia.Where(x => x.Id == item.ID).FirstOrDefaultAsync();
                    if (checkExists == null)
                    {
                        IdListFail.Add("Bảng giá có ID:" + item.ID + " này không tồn tại " + " \r\n");
                        continue;

                    }
                    var checkTT = await _TMSContext.BangGia.Where(x => x.Id == item.ID && x.TrangThai == 1).FirstOrDefaultAsync();
                    if (checkTT == null)
                    {
                        IdListFail.Add("Bảng giá có id " + item.ID + " phải ở trạng thái tạo mới  " +" \r\n");
                        continue;

                    }

                    checkTT.TrangThai = 2;
                    _TMSContext.Update(checkTT);
                    var result = await _TMSContext.SaveChangesAsync();

                    if (result > 0)
                    {

                        var editTT = from x in (from bg in _TMSContext.BangGia
                                                where (from mhd in _TMSContext.HopDongVaPhuLuc
                                                       where (from bg1 in _TMSContext.BangGia
                                                              join hd in _TMSContext.HopDongVaPhuLuc
                                                              on bg1.MaHopDong equals hd.MaHopDong
                                                              where hd.MaHopDong == checkTT.MaHopDong
                                                              select hd.MaKh).Contains(mhd.MaKh)
                                                       select mhd.MaHopDong).Contains(bg.MaHopDong)
                                                select bg)
                                     where x.Id != checkTT.Id && x.MaCungDuong == checkTT.MaCungDuong && x.MaPtvc == checkTT.MaPtvc && x.MaLoaiPhuongTien == checkTT.MaLoaiPhuongTien && x.TrangThai == 2 && x.MaLoaiHangHoa == checkTT.MaLoaiHangHoa && x.MaDvt == checkTT.MaDvt
                                     select x;
                        if (editTT != null)
                        {
                            var bg = await _TMSContext.BangGia.Where(x => editTT.Select(y => y.Id).Contains(x.Id)).ToListAsync();
                            foreach (var i in bg)
                            {
                                i.TrangThai = 0;
                                i.NgayHetHieuLuc = checkTT.NgayApDung;
                                _TMSContext.BangGia.Update(i);
                                var result1 = await _TMSContext.SaveChangesAsync();
                                if (result1 > 0)
                                {
                                    IdList.Add("chuyển bảng giá có ID: " + i.Id + " về hết hiệu lực thành công" + " \r\n");
                                    continue;
                                }
                                else
                                {
                                    IdListFail.Add("chuyển bảng giá có ID" + i.Id + " về hết hiệu lực Thất Bại" + " \r\n");
                                    continue;

                                }
                            }
                        }
                        IdList.Add("Approve thành công bảng giá có ID :" + item.ID + " \r\n");
                        continue;
                    }
                    else
                    {
                        IdListFail.Add( "Approve Bảng Giá thất bại (sql)!  "+item.ID  +" \r\n");
                        continue;
                    }
                }
                if (IdList.Count>0)
                {
                    string a = string.Join(",", IdList);
                    string b = string.Join(",", IdListFail);
                    await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService with Id: ");
                    return new BoolActionResult { isSuccess = true, Message =  a  + " \r\n"+b  +" \r\n" };
                }
                else
                {
                    string a =string.Join(",", IdListFail);
                    return new BoolActionResult { isSuccess = false, Message = a  +" \r\n" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }

        }
        public async Task<BoolActionResult> ApproveProductServiceRequestByMaHD(ApproveProductServiceRequestByMaHD request)
        {
            try
            {
                List<string> IdList = new List<string>();
                List<string> IdListFail = new List<string>();
                var checkExists1 = await _TMSContext.BangGia.Where(x => x.MaHopDong == request.MaHopDong).FirstOrDefaultAsync();
                if (checkExists1 == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng" + request.MaHopDong + " không tồn tại " };

                }           

                var checkExists = await _TMSContext.BangGia.Where(x => x.MaHopDong == request.MaHopDong && x.TrangThai == 1).ToListAsync();
                if (checkExists != null)
                {
                    foreach (var i in checkExists)
                    {
                        i.TrangThai = 2;
                        _TMSContext.Update(i);
                        var result = await _TMSContext.SaveChangesAsync();

                        if (result > 0)
                        {                        

                            var editTT = from x in (from bg in _TMSContext.BangGia
                                           where (from mhd in _TMSContext.HopDongVaPhuLuc
                                                                       where (from bg1 in _TMSContext.BangGia
                                                                                               join hd in _TMSContext.HopDongVaPhuLuc
                                                                                               on bg1.MaHopDong equals hd.MaHopDong
                                                                                               where hd.MaHopDong == i.MaHopDong
                                                                                               select hd.MaKh).Contains(mhd.MaKh)
                                                                       select mhd.MaHopDong).Contains(bg.MaHopDong)
                                           select bg)
                                           where x.Id != i.Id  && x.MaCungDuong == i.MaCungDuong && x.MaPtvc == i.MaPtvc && x.MaLoaiPhuongTien == i.MaLoaiPhuongTien && x.TrangThai == 2 && x.MaLoaiHangHoa ==i.MaLoaiHangHoa && x.MaDvt == i.MaDvt
                                         select x;
                            
                            if (editTT != null)
                            {

                                var bg = await _TMSContext.BangGia.Where(x=> editTT.Select(y=>y.Id).Contains(x.Id)).ToListAsync();
                                // chuyển kiểu dữ liệu từ queryable về list

                                foreach (var j in bg)
                                {
                                    j.TrangThai = 0;
                                    j.NgayHetHieuLuc = i.NgayApDung;
                                    _TMSContext.BangGia.Update(j);
                                    var result1 = await _TMSContext.SaveChangesAsync();
                                    if (result1 > 0)
                                    {
                                        IdList.Add("chuyển bảng giá có ID: " + j.Id + " về hết hiệu lực thành công" + " \r\n");
                                        continue;
                                    }
                                    else
                                    {

                                        IdList.Add("chuyển bảng giá có ID" + j.Id + " về hết hiệu lực Thất Bại");
                                        continue;
                                    }
                                }
                            }
                            IdList.Add(" Approve thành công bảng giá có ID :" + i.Id + " \r\n");
                            continue;
                        }
                        else
                        {
                            IdListFail.Add("Approve Bảng Giá thất bại!  " + i.Id + " \r\n");
                            continue;
                        }
                    }
                    if (IdList.Count > 0)
                    {
                        string a = string.Join(",", IdList);
                        string b = string.Join(",", IdListFail);
                        await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService with Id: ");
                        return new BoolActionResult { isSuccess = true, Message = a + " \r\n" + b + " \r\n" };
                    }
                    else
                    {
                        string a = string.Join(",", IdListFail);
                        return new BoolActionResult { isSuccess = false, Message = a + " \r\n" };
                    }
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng" + request.MaHopDong + " không có bản tạo mới cần Approve " };
                }
                
            }

            catch (Exception ex)
            {
                await _common.Log("ProductServiceManage", "UserId: " + TempData.UserID + " create new ProductService has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<ListProductServiceRequest> GetProductServiceByIdRequest(int id)
        {
            try
            {
                var getProductServiceById = await _TMSContext.BangGia.Where(x => x.Id == id).FirstOrDefaultAsync();
                return new ListProductServiceRequest()
                {
                    ID = getProductServiceById.Id,
                    MaHopDong = getProductServiceById.MaHopDong,
                    MaPTVC = getProductServiceById.MaPtvc,
                    MaCungDuong = getProductServiceById.MaCungDuong,
                    MaLoaiPhuongTien = getProductServiceById.MaLoaiPhuongTien,
                    DonGia = getProductServiceById.DonGia,
                    MaDVT = getProductServiceById.MaDvt,
                    MaLoaiHangHoa = getProductServiceById.MaLoaiHangHoa,
                    MaLoaiDoiTac = getProductServiceById.MaLoaiDoiTac,
                    NgayApDung = getProductServiceById.NgayApDung,
                    NgayHetHieuLuc = getProductServiceById.NgayHetHieuLuc,
                    TrangThai = getProductServiceById.TrangThai,
                    UpdatedTime = getProductServiceById.UpdatedTime,
                    CreatedTime = getProductServiceById.CreatedTime
                };
            }   
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaKH(PaginationFilter filter, string MaKH)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from bg in _TMSContext.BangGia
                               where (from pro in _TMSContext.HopDongVaPhuLuc
                                      join bg1 in _TMSContext.BangGia
                                      on pro.MaHopDong equals bg1.MaHopDong
                                      where pro.MaKh == MaKH
                                      select pro.MaHopDong).Contains(bg.MaHopDong) && bg.TrangThai == 2 //&& bg.NgayApDung<= ngaylay && bg.NgayHetHieuLuc >ngaylay&& 
                               select new { bg };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.NgayApDung.ToString().Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.TrangThai.ToString().Contains(filter.Keyword));
                }
                var totalCount = await listData.CountAsync();
                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListProductServiceRequest()
                {
                    ID = x.bg.Id,
                    MaHopDong = x.bg.MaHopDong,
                    MaPTVC = x.bg.MaPtvc,
                    MaCungDuong = x.bg.MaCungDuong,
                    MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                    DonGia = x.bg.DonGia,
                    MaDVT = x.bg.MaDvt,
                    MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                    NgayApDung = x.bg.NgayApDung,
                    NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                    TrangThai = x.bg.TrangThai,
                    UpdatedTime = x.bg.UpdatedTime,
                    CreatedTime = x.bg.CreatedTime

                }).ToListAsync();
                return new PagedResponseCustom<ListProductServiceRequest>()
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
        public async Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaHD(PaginationFilter filter, string MaHD)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from bg in _TMSContext.BangGia
                               where bg.MaHopDong == MaHD &&( bg.TrangThai == 2|| bg.TrangThai == 1)
                               //&& bg.NgayApDung <= ngaylay && bg.NgayHetHieuLuc > ngaylay && bg.TrangThai == 2
                               select new { bg };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.NgayApDung.ToString().Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.TrangThai.ToString().Contains(filter.Keyword));
                }
                var totalCount = await listData.CountAsync();
                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListProductServiceRequest()
                {
                    ID = x.bg.Id,
                    MaHopDong = x.bg.MaHopDong,
                    MaPTVC = x.bg.MaPtvc,
                    MaCungDuong = x.bg.MaCungDuong,
                    MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                    DonGia = x.bg.DonGia,
                    MaDVT = x.bg.MaDvt,
                    MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                    NgayApDung = x.bg.NgayApDung,
                    NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                    TrangThai = x.bg.TrangThai,
                    UpdatedTime = x.bg.UpdatedTime,
                    CreatedTime = x.bg.CreatedTime

                }).ToListAsync();
                return new PagedResponseCustom<ListProductServiceRequest>()
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
        public async Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductService(PaginationFilter filter, int trangthai)
        {
            try
            {
               
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from bg in _TMSContext.BangGia
                                   where bg.TrangThai == trangthai 
                                   select new { bg };                       
                if(trangthai == 7)
                {
                    listData = from bg in _TMSContext.BangGia
                               where bg.TrangThai == 2 || bg.TrangThai == 1
                               select new { bg };
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.NgayApDung.ToString().Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.TrangThai.ToString().Contains(filter.Keyword));
                }
                var totalCount = await listData.CountAsync();
                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListProductServiceRequest()
                {
                    ID = x.bg.Id,
                    MaHopDong = x.bg.MaHopDong,
                    MaPTVC = x.bg.MaPtvc,
                    MaCungDuong = x.bg.MaCungDuong,
                    MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                    DonGia = x.bg.DonGia,
                    MaDVT = x.bg.MaDvt,
                    MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                    NgayApDung = x.bg.NgayApDung,
                    NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                    TrangThai = x.bg.TrangThai,
                    UpdatedTime = x.bg.UpdatedTime,
                    CreatedTime = x.bg.CreatedTime

                }).ToListAsync();
                return new PagedResponseCustom<ListProductServiceRequest>()
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
        public async Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByDate(PaginationFilter filter, DateTime date)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var listData = from bg in _TMSContext.BangGia
                               where  bg.NgayApDung <= date && bg.NgayHetHieuLuc > date && bg.TrangThai != 3
                               select new { bg };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.MaHopDong.Contains(filter.Keyword));
                }
                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.bg.TrangThai.ToString().Contains(filter.Keyword));
                }
                var totalCount = await listData.CountAsync();
                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListProductServiceRequest()
                {
                    ID = x.bg.Id,
                    MaHopDong = x.bg.MaHopDong,
                    MaPTVC = x.bg.MaPtvc,
                    MaCungDuong = x.bg.MaCungDuong,
                    MaLoaiPhuongTien = x.bg.MaLoaiPhuongTien,
                    DonGia = x.bg.DonGia,
                    MaDVT = x.bg.MaDvt,
                    MaLoaiHangHoa = x.bg.MaLoaiHangHoa,
                    MaLoaiDoiTac = x.bg.MaLoaiDoiTac,
                    NgayApDung = x.bg.NgayApDung,
                    NgayHetHieuLuc = x.bg.NgayHetHieuLuc,
                    TrangThai = x.bg.TrangThai,
                    UpdatedTime = x.bg.UpdatedTime,
                    CreatedTime = x.bg.CreatedTime

                }).ToListAsync();
                return new PagedResponseCustom<ListProductServiceRequest>()
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
        private async Task<string> ValiateProductService(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, string MaLoaiHopDong, DateTime NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkMaHopDong = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == MaHopDong).FirstOrDefaultAsync();

            if (checkMaHopDong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Hợp đồng: "+ MaHopDong+" không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate  MaCungDuong
            var checkMaCungDuong = await _TMSContext.CungDuong.Where(x => x.MaCungDuong == MaCungDuong).FirstOrDefaultAsync();

            if (checkMaCungDuong == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Cung đường:"+ MaCungDuong + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            //validate LoaiPhuongTien
            var checkMaLoaiPhuongTien = await _TMSContext.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

            if (checkMaLoaiPhuongTien == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện:"+ MaLoaiPhuongTien+" không tồn tại \r\n" + System.Environment.NewLine;
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
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Đã tồn tại thêm mới thất bại \r\n Vui lòng tạo mới bảng giá khác hoặc vào mục Update \r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;
        }
        private async Task<string> ValiateEdit(string MaHopDong, string MaPTVC, string MaCungDuong, string MaLoaiPhuongTien, decimal DonGia, string MaDVT, string MaLoaiHangHoa, string MaLoaiHopDong, DateTime NgayHetHieuLuc, string ErrorRow = "")
        {
            string ErrorValidate = "";
            var checkTrangThai = await _TMSContext.BangGia.Where(x => x.TrangThai == 1).FirstOrDefaultAsync();
            if (checkTrangThai == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Trạng thải phải là tạo mới \r\n" + System.Environment.NewLine;
            }
            var NgayApDung = from j in _TMSContext.HopDongVaPhuLuc
                             where j.MaHopDong == MaHopDong
                             select j.ThoiGianBatDau;
            if (NgayApDung.FirstOrDefault()>= NgayHetHieuLuc)
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
     
            if (!Regex.IsMatch(NgayHetHieuLuc.ToString("dd/MM/yyyy"), "^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Sai định dạng ngày\r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;

        }
    }
}