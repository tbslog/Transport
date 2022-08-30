using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.AddressManage;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.CustommerManage
{
    public class CustomerService : ICustomer
    {
        private readonly TMSContext _TMSContext;
        private readonly ICommon _common;
        private readonly IAddress _address;

        public CustomerService(TMSContext TMSContext, ICommon common, IAddress address)
        {
            _address = address;
            _TMSContext = TMSContext;
            _common = common;
        }

        public async Task<BoolActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                var checkExists = await _TMSContext.KhachHangs.Where(x => x.MaKh == request.MaKh || x.TenKh == request.TenKh).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng này đã tồn tại" };
                }

                string fullAddress = await _address.GetFullAddress(request.Address.SoNha, request.Address.MaTinh, request.Address.MaHuyen, request.Address.MaPhuong);

                string ErrorValidate = ValiateCustommer(request.MaKh, request.TenKh, request.MaSoThue, request.Sdt, request.Email, request.Address.SoNha, request.Address.MaGps, fullAddress);

                var addAddress = await _TMSContext.AddAsync(new DiaDiem()
                {
                    TenDiaDiem = request.TenKh,
                    MaQuocGia = request.Address.MaQuocGia,
                    MaTinh = request.Address.MaTinh,
                    MaHuyen = request.Address.MaHuyen,
                    MaPhuong = request.Address.MaPhuong,
                    SoNha = request.Address.SoNha,
                    DiaChiDayDu = fullAddress,
                    MaGps = request.Address.MaGps,
                    MaLoaiDiaDiem = request.Address.MaLoaiDiaDiem,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                await _TMSContext.SaveChangesAsync();

                await _TMSContext.AddAsync(new KhachHang()
                {
                    MaKh = request.MaKh,
                    TenKh = request.TenKh,
                    MaSoThue = request.MaSoThue,
                    Sdt = request.Sdt,
                    Email = request.Email,
                    MaDiaDiem = addAddress.Entity.MaDiaDiem,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                var result = await _TMSContext.SaveChangesAsync();

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (result > 0)
                {
                    await _common.Log("CustommerManage", "UserId: " + TempData.UserID + " create new custommer with Id: " + request.MaKh);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("CustommerManage", "UserId: " + TempData.UserID + " create new custommer has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditCustomer(string CustomerId, EditCustomerRequest request)
        {
            try
            {
                var GetCustommer = await _TMSContext.KhachHangs.Where(x => x.MaKh == CustomerId).FirstOrDefaultAsync();

                if (GetCustommer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
                }

                var getAddress = await _TMSContext.DiaDiems.Where(x => x.MaDiaDiem == GetCustommer.MaDiaDiem).FirstOrDefaultAsync();

                string FullAddress = await _address.GetFullAddress(request.Address.SoNha, request.Address.MaTinh, request.Address.MaHuyen, request.Address.MaPhuong);
                string ErrorValidate = ValiateCustommer(CustomerId, request.TenKh, request.MaSoThue, request.Sdt, request.Email, request.Address.SoNha, request.Address.MaGps, FullAddress);

                getAddress.TenDiaDiem = request.Address.TenDiaDiem;
                getAddress.MaQuocGia = request.Address.MaQuocGia;
                getAddress.MaTinh = request.Address.MaTinh;
                getAddress.MaHuyen = request.Address.MaHuyen;
                getAddress.MaPhuong = request.Address.MaPhuong;
                getAddress.SoNha = request.Address.SoNha;
                getAddress.DiaChiDayDu = FullAddress;
                getAddress.MaGps = request.Address.MaGps;
                getAddress.MaLoaiDiaDiem = request.Address.MaLoaiDiaDiem;
                getAddress.UpdatedTime = DateTime.Now;

                GetCustommer.TenKh = request.TenKh;
                GetCustommer.MaSoThue = request.MaSoThue;
                GetCustommer.Sdt = request.Sdt;
                GetCustommer.Email = request.Email;
                GetCustommer.CreatedTime = DateTime.Now;

                _TMSContext.Update(GetCustommer);

                var result = await _TMSContext.SaveChangesAsync();

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (result > 0)
                {
                    await _common.Log("CustommerManage", "UserId: " + TempData.UserID + " Edit custommer with Id: " + CustomerId);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("CustommerManage", "UserId: " + TempData.UserID + " Edit custommer with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetCustomerRequest> GetCustomerById(string CustomerId)
        {
            var getCustommer = from cus in _TMSContext.KhachHangs
                               join address in _TMSContext.DiaDiems
                               on cus.MaDiaDiem equals address.MaDiaDiem
                               where cus.MaKh == CustomerId
                               select new { cus, address };

            return await getCustommer.Select(x => new GetCustomerRequest()
            {
                MaKh = x.cus.MaKh,
                TenKh = x.cus.TenKh,
                Email = x.cus.Email,
                MaSoThue = x.cus.MaSoThue,
                Sdt = x.cus.Sdt,
                address = new GetAddressModel()
                {
                    MaDiaDiem = x.address.MaDiaDiem,
                    DiaChiDayDu = x.address.DiaChiDayDu,
                    TenDiaDiem = x.address.TenDiaDiem,
                    MaGps = x.address.MaGps,
                    CreatedTime = x.address.CreatedTime,
                    UpdatedTime = x.address.UpdatedTime
                }
            }).FirstOrDefaultAsync();
        }

        public async Task<PagedResponseCustom<ListCustommerRequest>> getListCustommer(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from cus in _TMSContext.KhachHangs
                               join address in _TMSContext.DiaDiems
                               on cus.MaDiaDiem equals address.MaDiaDiem
                               orderby cus.CreatedTime descending
                               select new { cus, address };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.cus.MaKh.Contains(filter.Keyword));
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
                    MaSoThue = x.cus.MaSoThue,
                    Sdt = x.cus.Sdt,
                    Email = x.cus.Email,
                    MaDiaDiem = x.cus.MaDiaDiem,
                    DiaDiem = x.address.DiaChiDayDu,
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

        public async Task<BoolActionResult> ReadExcelFile(IFormFile formFile, CancellationToken cancellationToken)
        {
            int ErrorRow = 0;
            int ErrorInsert = 1;

            string ErrorValidate = "";

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

                var list = new List<CreateCustomerRequest>();

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

                        if (worksheet.Cells[1, 1].Value.ToString().Trim() != "Mã Khách Hàng" ||
                            worksheet.Cells[1, 2].Value.ToString().Trim() != "Tên Khách Hàng" ||
                            worksheet.Cells[1, 3].Value.ToString().Trim() != "Mã Số Thuế" ||
                            worksheet.Cells[1, 4].Value.ToString().Trim() != "Số Điện Thoại" ||
                            worksheet.Cells[1, 5].Value.ToString().Trim() != "Địa chỉ Email" ||
                            worksheet.Cells[1, 6].Value.ToString().Trim() != "Số Nhà" ||
                            worksheet.Cells[1, 7].Value.ToString().Trim() != "Mã Tỉnh" ||
                            worksheet.Cells[1, 8].Value.ToString().Trim() != "Mã Huyện" ||
                            worksheet.Cells[1, 9].Value.ToString().Trim() != "Mã Phường" ||
                            worksheet.Cells[1, 10].Value.ToString().Trim() != "Mã GPS"
                            )
                        {
                            return new BoolActionResult { isSuccess = false, Message = "File excel không đúng " };
                        }

                        for (int row = 2; row <= rowCount; row++)
                        {
                            ErrorRow = row;

                            string MaKh = worksheet.Cells[row, 1].Value.ToString().Trim().ToUpper();
                            string TenKh = worksheet.Cells[row, 2].Value.ToString().Trim();
                            string MaSoThue = worksheet.Cells[row, 3].Value.ToString().Trim();
                            string Sdt = worksheet.Cells[row, 4].Value.ToString().Trim();
                            string Email = worksheet.Cells[row, 5].Value.ToString().Trim();
                            string SoNha = worksheet.Cells[row, 6].Value.ToString().Trim();
                            string MaGps = worksheet.Cells[row, 10].Value.ToString().Trim();

                            int MaTinh = int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                            int MaHuyen = int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim());
                            int MaPhuong = int.Parse(worksheet.Cells[row, 9].Value.ToString().Trim());

                            var FullAddress = await _address.GetFullAddress(SoNha, MaTinh, MaHuyen, MaPhuong);

                            ErrorValidate = ValiateCustommer(MaKh, TenKh, MaSoThue, Sdt, Email, SoNha, MaGps, FullAddress, ErrorRow.ToString());

                            if (ErrorValidate == "")
                            {
                                list.Add(new CreateCustomerRequest
                                {
                                    MaKh = MaKh,
                                    TenKh = TenKh,
                                    MaSoThue = MaSoThue,
                                    Sdt = Sdt,
                                    Email = Email,
                                    Address = new CreateAddressRequest
                                    {
                                        TenDiaDiem = TenKh,
                                        MaQuocGia = 1,
                                        SoNha = SoNha,
                                        MaTinh = MaTinh,
                                        MaHuyen = MaHuyen,
                                        MaPhuong = MaPhuong,
                                        DiaChiDayDu = FullAddress,
                                        MaGps = MaGps,
                                        MaLoaiDiaDiem = "1",
                                    }
                                });
                            }
                        }
                    }
                }

                string DuplicateCus = "";

                foreach (var item in list)
                {
                    var checkExists = await _TMSContext.KhachHangs.Where(x => x.MaKh == item.MaKh).FirstOrDefaultAsync();
                    ErrorInsert += 1;
                    if (checkExists == null)
                    {
                        var addAddress = await _TMSContext.AddAsync(new DiaDiem()
                        {
                            TenDiaDiem = item.TenKh,
                            MaQuocGia = item.Address.MaQuocGia,
                            MaTinh = item.Address.MaTinh,
                            MaHuyen = item.Address.MaHuyen,
                            MaPhuong = item.Address.MaPhuong,
                            SoNha = item.Address.SoNha,
                            DiaChiDayDu = item.Address.DiaChiDayDu,
                            MaGps = item.Address.MaGps,
                            MaLoaiDiaDiem = item.Address.MaLoaiDiaDiem,
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        });

                        await _TMSContext.SaveChangesAsync();

                        await _TMSContext.AddAsync(new KhachHang()
                        {
                            MaKh = item.MaKh,
                            TenKh = item.TenKh,
                            MaSoThue = item.MaSoThue,
                            Sdt = item.Sdt,
                            Email = item.Email,
                            MaDiaDiem = addAddress.Entity.MaDiaDiem,
                            CreatedTime = DateTime.Now,
                            UpdatedTime = DateTime.Now
                        });

                        await _TMSContext.SaveChangesAsync();
                    }
                    else
                    {
                        DuplicateCus += item.MaKh + ", ";
                    }
                }

                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                if (DuplicateCus.Length > 1)
                {
                    return new BoolActionResult { Message = "Những khách hàng có mã " + DuplicateCus + " đã tồn tại", isSuccess = true };
                }
                else
                {
                    return new BoolActionResult { Message = "OK", isSuccess = true };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { Message = ex.ToString(), isSuccess = false, DataReturn = "Bị lỗi tại dòng: " + ErrorRow + " >>>Lỗi Insert dòng: " + ErrorInsert };
            }
        }

        private string ValiateCustommer(string MaKh, string TenKh, string MaSoThue, string Sdt, string Email, string SoNha, string MaGps, string FullAddress, string ErrorRow = "")
        {
            string ErrorValidate = "";

            if (MaKh.Length != 8)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã khách hàng phải dài 8 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaKh, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã khách hàng không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }

            if (TenKh.Length > 50 || TenKh.Length == 0)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên khách hàng không được rỗng hoặc nhiều hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(TenKh, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tên khách hàng không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }

            if (MaSoThue.Length == 0 || MaSoThue.Length > 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã số thuế không được rỗng hoặc nhiều hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(MaSoThue, "^(?![_.])(?![_.])(?!.*[_.]{2})[0-9]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã số thuế chỉ được chứa ký tự là số \r\n" + System.Environment.NewLine;
            }

            if (Sdt.Length == 0 || Sdt.Length > 20)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + "- Số điện thoại không được rỗng hoặc nhiều hơn 20 ký tự \r\n" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(Sdt, "^(?![ _.])(?![_.])(?!.*[_.]{2})[0-9 ]+(?<![_. ])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Số điện thoại chỉ được chứa ký tự là số \r\n" + System.Environment.NewLine;
            }

            if (Email.Length == 0 || Email.Length > 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Địa chỉ Email không được rỗng hoặc nhiều hơn 50 ký tự" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(Email, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Địa chỉ Email không đúng" + System.Environment.NewLine;
            }

            if (SoNha.Length == 0 || SoNha.Length > 100)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Số nhà không được rỗng hoặc nhiều hơn 100 ký tự \r\n" + System.Environment.NewLine;
            }

            if (MaGps.Length == 0 || MaGps.Length > 50)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã GPS không được rỗng hoặc nhiều hơn 50 ký tự \r\n" + System.Environment.NewLine;
            }

            if (FullAddress == "")
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã tỉnh, mã huyện, mã phường không khớp, vui lòng kiểm tra lại \r\n" + System.Environment.NewLine;
            }

            return ErrorValidate;
        }
    }
}