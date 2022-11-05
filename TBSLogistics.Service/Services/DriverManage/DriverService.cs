using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.DriverModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.DriverManage
{
    public class DriverService : IDriver
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public DriverService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }

        public async Task<BoolActionResult> CreateDriver(CreateDriverRequest request)
        {
            try
            {
                var checkExists = await _context.TaiXe.Where(x => x.MaTaiXe == request.MaTaiXe || x.Cccd == request.Cccd).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế đã tồn tại" };
                }
                string ErrorValidate = await ValidateCreat(request.MaTaiXe, request.Cccd, request.HoVaTen, request.SoDienThoai, request.NgaySinh, request.MaNhaCC, request.MaLoaiPhuongTien, request.TaiXeTBS);
                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }
                await _context.TaiXe.AddAsync(new TaiXe()
                {
                    MaTaiXe = request.MaTaiXe.ToUpper(),
                    Cccd = request.Cccd,
                    HoVaTen = request.HoVaTen,
                    SoDienThoai = request.SoDienThoai,
                    NgaySinh = request.NgaySinh,
                    GhiChu = request.GhiChu,
                    MaNhaCungCap = request.MaNhaCC,
                    MaLoaiPhuongTien = request.MaLoaiPhuongTien,
                    TaiXeTbs = request.MaNhaCC.Contains("TBSL") ? true : false,
                    TrangThai = 1,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("DriverManage", "UserId: " + TempData.UserID + " create new driver with id: " + request.MaTaiXe);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới tài xế thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới tài xế thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("DriverManage", "UserId: " + TempData.UserID + " create new driver with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<BoolActionResult> EditDriver(string driverId, EditDriverRequest request)
        {
            try
            {
                var getDriver = await _context.TaiXe.Where(x => x.MaTaiXe == driverId).FirstOrDefaultAsync();

                if (getDriver == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế không tồn tại" };
                }
                var checktt = await _context.TaiXe.Where(x => x.TrangThai == 1).FirstOrDefaultAsync();

                if (checktt == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế phải ở trạng thái hoạt động" };
                }
                string ErrorValidate = await ValidateCreat(driverId, request.Cccd, request.HoVaTen, request.SoDienThoai, request.NgaySinh, request.MaNhaCungCap, request.MaLoaiPhuongTien, request.TaiXeTBS);
                if (ErrorValidate != "")
                {
                    return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
                }

                getDriver.Cccd = request.Cccd;
                getDriver.HoVaTen = request.HoVaTen;
                getDriver.SoDienThoai = request.SoDienThoai;
                getDriver.NgaySinh = request.NgaySinh;
                getDriver.GhiChu = request.GhiChu;
                getDriver.TaiXeTbs = request.MaNhaCungCap.Contains("TBSL") ? true : false;
                getDriver.MaLoaiPhuongTien = request.MaLoaiPhuongTien;
                getDriver.UpdatedTime = DateTime.Now;

                _context.Update(getDriver);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("DriverManage", "UserId: " + TempData.UserID + " update driver with id: " + driverId);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật tài xế thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật tài xế thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("DriverManage", "UserId: " + TempData.UserID + " Edit driver with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Cập nhật tài xế thất bại" };
            }
        }
        public async Task<BoolActionResult> DeleteDriver(string driverId)
        {
            try
            {
                var getDriver = await _context.TaiXe.Where(x => x.MaTaiXe == driverId).FirstOrDefaultAsync();

                if (getDriver == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế không tồn tại" };
                }
                var checktt = await _context.TaiXe.Where(x => x.TrangThai == 1).FirstOrDefaultAsync();

                if (checktt == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế phải ở trạng thái hoạt động" };
                }
                getDriver.TrangThai = 2;


                _context.Update(getDriver);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("DriverManage", "UserId: " + TempData.UserID + " Delete driver with id: " + driverId);
                    return new BoolActionResult { isSuccess = true, Message = "Xóa tài xế thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xóa tài xế thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("DriverManage", "UserId: " + TempData.UserID + " Delete driver with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Xóa tài xế thất bại" };
            }
        }
        public async Task<GetDriverRequest> GetDriverByCardId(string cccd)
        {

            var driver = await _context.TaiXe.Where(x => x.Cccd == cccd).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaCungCap = x.MaNhaCungCap,
                MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).FirstOrDefaultAsync();

            return driver;
        }
        public async Task<GetDriverRequest> GetDriverById(string driverId)
        {
            var driver = await _context.TaiXe.Where(x => x.MaTaiXe == driverId).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaCungCap = x.MaNhaCungCap,
                MaLoaiPhuongTien = x.MaLoaiPhuongTien,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).FirstOrDefaultAsync();

            return driver;
        }
        public async Task<List<GetDriverRequest>> GetListDriverSelect()
        {
            var list = await _context.TaiXe.Where(x => x.TaiXeTbs == true).ToListAsync();

            return list.Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                HoVaTen = x.HoVaTen
            }).ToList();

        }
        public async Task<PagedResponseCustom<ListDriverRequest>> getListDriver(PaginationFilter filter)
        {
            try
            {

                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var getData = from driver in _context.TaiXe
                              orderby driver.CreatedTime descending
                              select new { driver };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.driver.MaTaiXe.ToLower().Contains(filter.Keyword.ToLower())
                    || x.driver.HoVaTen.ToLower().Contains(filter.Keyword.ToLower())
                    || x.driver.SoDienThoai.ToLower().Contains(filter.Keyword.ToLower())
                    || x.driver.Cccd.ToLower().Contains(filter.Keyword.ToLower())
                    );
                }
                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    getData = getData.Where(x => x.driver.TrangThai == int.Parse(filter.statusId));
                }
                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.driver.CreatedTime.Date >= filter.fromDate.Value.Date && x.driver.CreatedTime <= filter.toDate.Value.Date);
                }

                var totalRecords = await getData.CountAsync();

                var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListDriverRequest()
                {
                    MaTaiXe = x.driver.MaTaiXe,
                    Cccd = x.driver.Cccd,
                    HoVaTen = x.driver.HoVaTen,
                    SoDienThoai = x.driver.SoDienThoai,
                    NgaySinh = x.driver.NgaySinh,
                    GhiChu = x.driver.GhiChu,
                    MaNhaCungCap = x.driver.MaNhaCungCap,
                    MaLoaiPhuongTien = x.driver.MaLoaiPhuongTien,
                    TrangThai = x.driver.TrangThai,
                    Createdtime = x.driver.CreatedTime,
                    UpdateTime = x.driver.UpdatedTime,
                }).ToListAsync();

                return new PagedResponseCustom<ListDriverRequest>()
                {
                    paginationFilter = validFilter,
                    totalCount = totalRecords,
                    dataResponse = pagedData
                };
            }
            catch (Exception)
            {
                throw;
            }

        }
        private async Task<string> ValidateCreat(string MaTaiXe, string CCCD, string HoVaTen, string SoDienThoai, DateTime? NgaySinh, string MaNhaCungCap, string MaLoaiPhuongTien, bool TaiXeTBS, string ErrorRow = "")
        {
            string ErrorValidate = "";
            if (MaTaiXe.Length > 12 || MaTaiXe.Length < 5)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Tài Xế phải ít hơn 12 ký tự \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(MaTaiXe, "^(?![_.])(?![_.])(?!.*[_.]{2})[a-zA-Z0-9 aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ]+(?<![_.])$", RegexOptions.IgnoreCase))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Tài Xế không được chứa ký tự đặc biệt \r\n" + System.Environment.NewLine;
            }

            if (!Regex.IsMatch(SoDienThoai, "^(0|\\+84)(\\s|\\.)?((3[2-9])|(5[689])|(7[06-9])|(8[1-689])|(9[0-46-9]))(\\d)(\\s|\\.)?(\\d{3})(\\s|\\.)?(\\d{3})$"))
            {
                ErrorValidate += "Lỗi Dòng >>> " + SoDienThoai + " : Số điện thoại không hợp lệ \r\n";
            }

            if (NgaySinh != null)
            {
                if (!Regex.IsMatch(NgaySinh.Value.ToString("dd/MM/yyyy"), "^(((0[1-9]|[12][0-9]|30)[-/]?(0[13-9]|1[012])|31[-/]?(0[13578]|1[02])|(0[1-9]|1[0-9]|2[0-8])[-/]?02)[-/]?[0-9]{4}|29[-/]?02[-/]?([0-9]{2}(([2468][048]|[02468][48])|[13579][26])|([13579][26]|[02468][048]|0[0-9]|1[0-6])00))$", RegexOptions.IgnoreCase))
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Sai định dạng ngày\r\n" + System.Environment.NewLine;
                }
                if ((DateTime.Now.Date - NgaySinh.Value.Date).Days <= 6570)
                {
                    ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Tài xế chưa đủ 18 tuổi \r\n" + System.Environment.NewLine;
                }
            }
            if (!Regex.IsMatch(CCCD, "^([0-9]{12})$"))
            {
                ErrorValidate += "Lỗi Dòng >>> " + " - CCCD phải là số và đủ 12 kí tự \r\n";
            }

            var checkMaKH = await _context.KhachHang.Where(x => x.MaKh == MaNhaCungCap).FirstOrDefaultAsync();

            if (checkMaKH == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã nhà cung cấp: " + MaNhaCungCap + " không tồn tại \r\n" + System.Environment.NewLine;
            }

            var checkMapt = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == MaLoaiPhuongTien).FirstOrDefaultAsync();

            if (checkMapt == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Loại Phương Tiện: " + MaLoaiPhuongTien + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;
        }
    }
}