using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.CustommerManage
{
    public class CustomerService : ICustomer
    {
        private readonly TMSContext _TMSContext;
        private readonly ICommon _common;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public CustomerService(TMSContext TMSContext, ICommon common,  IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _TMSContext = TMSContext;
            _common = common;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<BoolActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                var checkExists = await _TMSContext.KhachHang.Where(x => x.MaKh == request.MaKH).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã Khách hàng này đã tồn tại" };
                }

                string ErrorValidate = await ValiateCustommer(request.MaKH, request.TenKh, request.MaSoThue, request.Sdt, request.Email, request.LoaiKH, request.NhomKH, request.TrangThai);

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                await _TMSContext.AddAsync(new KhachHang()
                {
                    MaKh = request.MaKH.ToUpper(),
                    Chuoi = request.LoaiKH == "NCC" ? null : request.Chuoi.ToUpper(),
                    TenKh = request.TenKh,
                    MaSoThue = request.MaSoThue,
                    Sdt = request.Sdt,
                    Email = request.Email,
                    MaLoaiKh = request.LoaiKH,
                    MaNhomKh = request.NhomKH,
                    TrangThai = request.TrangThai,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now,
                    Creator = tempData.UserName,
                });

                var result = await _TMSContext.SaveChangesAsync();

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (result > 0)
                {
                    await _common.Log("CustomerManage", "UserId: " + tempData.UserName + " create new customer with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("CustommerManage", "UserId: " + tempData.UserName + " create new custommer has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditCustomer(string CustomerId, EditCustomerRequest request)
        {
            try
            {
                var GetCustommer = await _TMSContext.KhachHang.Where(x => x.MaKh == CustomerId).FirstOrDefaultAsync();

                if (GetCustommer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                string ErrorValidate = await ValiateCustommer(CustomerId, request.TenKh, request.MaSoThue, request.Sdt, request.Email, request.LoaiKH, request.NhomKH, request.TrangThai);

                GetCustommer.Chuoi = request.LoaiKH == "NCC" ? null : request.Chuoi.ToUpper();
                GetCustommer.TenKh = request.TenKh;
                GetCustommer.MaSoThue = request.MaSoThue;
                GetCustommer.Sdt = request.Sdt;
                GetCustommer.Email = request.Email;
                GetCustommer.CreatedTime = DateTime.Now;
                GetCustommer.MaLoaiKh = request.LoaiKH;
                GetCustommer.MaNhomKh = request.NhomKH;
                GetCustommer.TrangThai = request.TrangThai;
                GetCustommer.Updater = tempData.UserName;
                _TMSContext.Update(GetCustommer);

                var result = await _TMSContext.SaveChangesAsync();

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (result > 0)
                {
                    await _common.Log("CustomerManage", "UserId: " + tempData.UserName + " create Update customer with Data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("CustommerManage", "UserId: " + tempData.UserID + " Edit custommer with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetCustomerRequest> GetCustomerById(string CustomerId)
        {
            var getCustommer = from cus in _TMSContext.KhachHang
                               where cus.MaKh == CustomerId
                               select new { cus };

            return await getCustommer.Select(x => new GetCustomerRequest()
            {
                Chuoi = x.cus.Chuoi,
                MaKh = x.cus.MaKh,
                TenKh = x.cus.TenKh,
                Email = x.cus.Email,
                MaSoThue = x.cus.MaSoThue,
                Sdt = x.cus.Sdt,
                TrangThai = x.cus.TrangThai,
                NhomKH = x.cus.MaNhomKh,
                LoaiKH = x.cus.MaLoaiKh,
            }).FirstOrDefaultAsync();
        }

        public async Task<List<GetCustomerRequest>> getListCustomerOptionSelect(string type)
        {
            var getList = await _TMSContext.KhachHang.Where(x => x.TrangThai == 1).Select(x => new GetCustomerRequest()
            {
                MaKh = x.MaKh,
                TenKh = x.TenKh,
                LoaiKH = x.MaLoaiKh,
            }).ToListAsync();

            if (!string.IsNullOrEmpty(type))
            {
                getList = getList.Where(x => x.LoaiKH == type).ToList();
            }

            return getList;
        }

        public async Task<List<GetCustomerRequest>> GetListCustomerFilter(string type)
        {
            var getListFilter = await _TMSContext.UserHasCustomer.Where(x => x.UserId == tempData.UserID).Select(x => x.CustomerId).ToListAsync();

            var getList = await _TMSContext.KhachHang.Where(x => x.TrangThai == 1 && getListFilter.Contains(x.MaKh)).Select(x => new GetCustomerRequest()
            {
                MaKh = x.MaKh,
                TenKh = x.TenKh,
                LoaiKH = x.MaLoaiKh,
            }).ToListAsync();

            if (!string.IsNullOrEmpty(type))
            {
                getList = getList.Where(x => x.LoaiKH == type).ToList();
            }

            return getList;
        }

        public async Task<List<ListChuoiSelect>> GetListChuoiSelect()
        {
            var getlist = await _TMSContext.ChuoiKhachHang.ToListAsync();

            return getlist.Select(x => new ListChuoiSelect()
            {
                MaChuoi = x.MaChuoi,
                TenChuoi = x.MaChuoi + " - " + x.TenChuoi,
            }).ToList();
        }

        public async Task<PagedResponseCustom<ListCustommerRequest>> getListCustommer(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from cus in _TMSContext.KhachHang
                               orderby cus.CreatedTime descending
                               select new { cus };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.cus.MaKh.Contains(filter.Keyword) || x.cus.TenKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.customerType))
                {
                    listData = listData.Where(x => x.cus.MaLoaiKh == filter.customerType);
                }

                if (!string.IsNullOrEmpty(filter.customerGroup))
                {
                    listData = listData.Where(x => x.cus.MaNhomKh == filter.customerGroup);
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.cus.CreatedTime.Date >= filter.fromDate && x.cus.CreatedTime.Date <= filter.toDate);
                }

                var totalCount = await listData.CountAsync();

                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListCustommerRequest()
                {
                    MaKh = x.cus.MaKh,
                    TenKh = x.cus.TenKh,
                    Chuoi = x.cus.Chuoi,
                    NhomKH = x.cus.MaNhomKh,
                    LoaiKH = x.cus.MaLoaiKh,
                    MaSoThue = x.cus.MaSoThue,
                    Sdt = x.cus.Sdt,
                    Email = x.cus.Email,
                    TrangThai = x.cus.TrangThai,
                    Createdtime = x.cus.CreatedTime,
                    UpdateTime = x.cus.UpdatedTime,
                }).ToListAsync();

                return new PagedResponseCustom<ListCustommerRequest>()
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

        //public async Task<BoolActionResult> ReadExcelFile(IFormFile formFile, CancellationToken cancellationToken)
        //{
        //    int ErrorRow = 0;
        //    int ErrorInsert = 1;

        //    string ErrorValidate = "";

        //    try
        //    {
        //        if (formFile == null || formFile.Length <= 0)
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
        //        }

        //        if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
        //        }

        //        var list = new List<CreateCustomerRequest>();

        //        using (var stream = new MemoryStream())
        //        {
        //            await formFile.CopyToAsync(stream, cancellationToken);

        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                var rowCount = worksheet.Dimension.Rows;

        //                if (rowCount == 0)
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
        //                }

        //                if (worksheet.Cells[1, 1].Value.ToString().Trim() != "Mã Khách Hàng" ||
        //                    worksheet.Cells[1, 2].Value.ToString().Trim() != "Tên Khách Hàng" ||
        //                    worksheet.Cells[1, 3].Value.ToString().Trim() != "Mã Số Thuế" ||
        //                    worksheet.Cells[1, 4].Value.ToString().Trim() != "Số Điện Thoại" ||
        //                    worksheet.Cells[1, 5].Value.ToString().Trim() != "Địa chỉ Email" ||
        //                    worksheet.Cells[1, 6].Value.ToString().Trim() != "Số Nhà" ||
        //                    worksheet.Cells[1, 7].Value.ToString().Trim() != "Mã Tỉnh" ||
        //                    worksheet.Cells[1, 8].Value.ToString().Trim() != "Mã Huyện" ||
        //                    worksheet.Cells[1, 9].Value.ToString().Trim() != "Mã Phường" ||
        //                    worksheet.Cells[1, 10].Value.ToString().Trim() != "Mã GPS" ||
        //                     worksheet.Cells[1, 11].Value.ToString().Trim() != "Nhóm Khách Hàng" ||
        //                      worksheet.Cells[1, 12].Value.ToString().Trim() != "Phân Loại Khách Hàng" ||
        //                       worksheet.Cells[1, 13].Value.ToString().Trim() != "Trạng Thái"
        //                    )
        //                {
        //                    return new BoolActionResult { isSuccess = false, Message = "File excel không đúng " };
        //                }

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    ErrorRow = row;

        //                    string MaKh = worksheet.Cells[row, 1].Value.ToString().Trim().ToUpper();
        //                    string TenKh = worksheet.Cells[row, 2].Value.ToString().Trim();
        //                    string MaSoThue = worksheet.Cells[row, 3].Value.ToString().Trim();
        //                    string Sdt = worksheet.Cells[row, 4].Value.ToString().Trim();
        //                    string Email = worksheet.Cells[row, 5].Value.ToString().Trim();
        //                    string SoNha = worksheet.Cells[row, 6].Value.ToString().Trim();
        //                    string MaGps = worksheet.Cells[row, 10].Value.ToString().Trim();
        //                    string NhomKH = worksheet.Cells[row, 11].Value.ToString().Trim();
        //                    string LoaiKH = worksheet.Cells[row, 12].Value.ToString().Trim();
        //                    int TrangThai = int.Parse(worksheet.Cells[row, 13].Value.ToString().Trim());

        //                    int MaTinh = int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
        //                    int MaHuyen = int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim());
        //                    int MaPhuong = int.Parse(worksheet.Cells[row, 9].Value.ToString().Trim());

        //                    var FullAddress = await _address.GetFullAddress(SoNha, MaTinh, MaHuyen, MaPhuong);

        //                    ErrorValidate = await ValiateCustommer(MaKh, TenKh, MaSoThue, Sdt, Email, SoNha, MaGps, LoaiKH, NhomKH, TrangThai, FullAddress, ErrorRow.ToString());

        //                    if (ErrorValidate == "")
        //                    {
        //                        list.Add(new CreateCustomerRequest
        //                        {
        //                            MaKh = MaKh,
        //                            TenKh = TenKh,
        //                            MaSoThue = MaSoThue,
        //                            Sdt = Sdt,
        //                            Email = Email,
        //                            NhomKH = NhomKH,
        //                            LoaiKH = LoaiKH,
        //                            TrangThai = TrangThai,
        //                            Address = new CreateAddressRequest
        //                            {
        //                                TenDiaDiem = TenKh,
        //                                MaQuocGia = 1,
        //                                SoNha = SoNha,
        //                                MaTinh = MaTinh,
        //                                MaHuyen = MaHuyen,
        //                                MaPhuong = MaPhuong,
        //                                DiaChiDayDu = FullAddress,
        //                                MaGps = MaGps,
        //                                MaLoaiDiaDiem = "1",
        //                            }
        //                        });
        //                    }
        //                }
        //            }
        //        }

        //        string DuplicateCus = "";

        //        foreach (var item in list)
        //        {
        //            var checkExists = await _TMSContext.KhachHang.Where(x => x.MaKh == item.MaKh).FirstOrDefaultAsync();
        //            ErrorInsert += 1;
        //            if (checkExists == null)
        //            {
        //                var addAddress = await _TMSContext.AddAsync(new DiaDiem()
        //                {
        //                    TenDiaDiem = item.TenKh,
        //                    MaQuocGia = item.Address.MaQuocGia,
        //                    MaTinh = item.Address.MaTinh,
        //                    MaHuyen = item.Address.MaHuyen,
        //                    MaPhuong = item.Address.MaPhuong,
        //                    SoNha = item.Address.SoNha,
        //                    DiaChiDayDu = item.Address.DiaChiDayDu,
        //                    MaGps = item.Address.MaGps,
        //                    MaLoaiDiaDiem = item.Address.MaLoaiDiaDiem,
        //                    CreatedTime = DateTime.Now,
        //                    UpdatedTime = DateTime.Now
        //                });

        //                await _TMSContext.SaveChangesAsync();

        //                await _TMSContext.AddAsync(new KhachHang()
        //                {
        //                    MaKh = item.MaKh,
        //                    TenKh = item.TenKh,
        //                    MaSoThue = item.MaSoThue,
        //                    Sdt = item.Sdt,
        //                    Email = item.Email,
        //                    MaNhomKh = item.NhomKH,
        //                    MaLoaiKh = item.LoaiKH,
        //                    TrangThai = item.TrangThai,
        //                    MaDiaDiem = addAddress.Entity.MaDiaDiem,
        //                    CreatedTime = DateTime.Now,
        //                    UpdatedTime = DateTime.Now
        //                });

        //                await _TMSContext.SaveChangesAsync();
        //            }
        //            else
        //            {
        //                DuplicateCus += item.MaKh + ", ";
        //            }
        //        }

        //        if (ErrorValidate != "")
        //        {
        //            return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
        //        }

        //        if (DuplicateCus.Length > 1)
        //        {
        //            return new BoolActionResult { Message = "Những khách hàng có mã " + DuplicateCus + " đã tồn tại", isSuccess = true };
        //        }
        //        else
        //        {
        //            return new BoolActionResult { Message = "OK", isSuccess = true };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return new BoolActionResult { Message = ex.ToString(), isSuccess = false, DataReturn = "Bị lỗi tại dòng: " + ErrorRow + " >>>Lỗi Insert dòng: " + ErrorInsert };
        //    }
        //}

        private async Task<string> ValiateCustommer(string MaKH, string TenKh, string MaSoThue, string Sdt, string Email, string LoaiKH, string NhomKH, int TrangThai, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (MaKH.Length <= 6)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã KH/NCC không được ít hơn 7 ký tự \r\n" + Environment.NewLine;
            }
            if (!Regex.IsMatch(MaKH, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã KH/NCC chỉ được có ký tự chữ và số \r\n" + Environment.NewLine;
            }

            var checkStatus = await _TMSContext.StatusText.Where(x => x.Id == TrangThai).FirstOrDefaultAsync();
            if (checkStatus == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Trạng thái khách hàng không tồn tại \r\n" + Environment.NewLine;
            }

            var checkCustomerType = await _TMSContext.LoaiKhachHang.Where(x => x.MaLoaiKh == LoaiKH).FirstOrDefaultAsync();
            if (checkCustomerType == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Loại khách hàng không tồn tại \r\n" + Environment.NewLine;
            }

            var checkCustomerGroup = await _TMSContext.NhomKhachHang.Where(x => x.MaNhomKh == NhomKH).FirstOrDefaultAsync();
            if (checkCustomerGroup == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Nhóm khách hàng không tồn tại \r\n" + Environment.NewLine;
            }

            if (TenKh.Length > 50 || TenKh.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên khách hàng không được rỗng hoặc nhiều hơn 50 ký tự \r\n" + Environment.NewLine;
            }

            if (!Regex.IsMatch(TenKh, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên khách hàng không được chứa ký tự đặc biệt \r\n" + Environment.NewLine;
            }

            if (NhomKH.Length > 10 || NhomKH.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Nhóm khách hàng không được rỗng hoặc nhiều hơn 10 ký tự \r\n" + Environment.NewLine;
            }

            if (!Regex.IsMatch(NhomKH, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Nhóm khách hàng không được chứa ký tự đặc biệt \r\n" + Environment.NewLine;
            }

            if (LoaiKH.Length > 10 || LoaiKH.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Loại khách hàng không được rỗng hoặc nhiều hơn 10 ký tự \r\n" + Environment.NewLine;
            }

            if (!Regex.IsMatch(LoaiKH, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Loại khách hàng không được chứa ký tự đặc biệt \r\n" + Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(MaSoThue))
            {
                if (MaSoThue.Length > 50)
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã số thuế không được rỗng hoặc nhiều hơn 50 ký tự \r\n" + Environment.NewLine;
                }

                if (!Regex.IsMatch(MaSoThue, "^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã số thuế chỉ được chứa ký tự là số \r\n" + Environment.NewLine;
                }
            }

            if (!string.IsNullOrEmpty(Sdt))
            {
                if (Sdt.Length > 20)
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + "- Số điện thoại không được rỗng hoặc nhiều hơn 20 ký tự \r\n" + Environment.NewLine;
                }

                if (!Regex.IsMatch(Sdt, "^(?![ _.])(?![_.])(?!.*[_.]{2})[0-9 ]+(?<![_. ])$", RegexOptions.IgnoreCase))
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Số điện thoại chỉ được chứa ký tự là số \r\n" + Environment.NewLine;
                }
            }

            if (!string.IsNullOrEmpty(Email))
            {
                if (Email.Length > 50)
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Địa chỉ Email không được rỗng hoặc nhiều hơn 50 ký tự" + Environment.NewLine;
                }

                if (!Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase))
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Địa chỉ Email không đúng" + Environment.NewLine;
                }
            }

            return ErrorValidate;
        }
    }
}