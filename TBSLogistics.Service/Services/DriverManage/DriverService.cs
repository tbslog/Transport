using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var checkExists = await _context.TaiXes.Where(x => x.MaTaiXe == request.MaTaiXe || x.Cccd == request.Cccd).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế đã tồn tại" };
                }

                await _context.TaiXes.AddAsync(new TaiXe()
                {
                    MaTaiXe = request.MaTaiXe,
                    Cccd = request.Cccd,
                    HoVaTen = request.HoVaTen,
                    SoDienThoai = request.SoDienThoai,
                    NgaySinh = request.NgaySinh,
                    GhiChu = request.GhiChu,
                    MaNhaThau = request.MaNhaThau,
                    TaiXeTbs = request.TaiXeTBS,
                    LoaiXe = request.LoaiXe,
                    TrangThai = request.TrangThai,
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
                var getDriver = await _context.TaiXes.Where(x => x.MaTaiXe == driverId).FirstOrDefaultAsync();

                if (getDriver == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài xế không tồn tại" };
                }

                getDriver.Cccd = request.Cccd;
                getDriver.HoVaTen = request.HoVaTen;
                getDriver.SoDienThoai = request.SoDienThoai;
                getDriver.NgaySinh = request.NgaySinh;
                getDriver.GhiChu = request.GhiChu;
                getDriver.MaNhaThau = request.MaNhaThau;
                getDriver.LoaiXe = request.LoaiXe;
                getDriver.TrangThai = request.TrangThai;
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

        public async Task<GetDriverRequest> GetDriverByCardId(string cccd)
        {
            var driver = await _context.TaiXes.Where(x => x.Cccd == cccd).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaThau = x.MaNhaThau,
                LoaiXe = x.LoaiXe,
                    TaiXeTBSL = x.TaiXeTbs,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).FirstOrDefaultAsync();

            return driver;
        }

        public async Task<GetDriverRequest> GetDriverById(string driverId)
        {
            var driver = await _context.TaiXes.Where(x => x.MaTaiXe == driverId).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaThau = x.MaNhaThau,
                LoaiXe = x.LoaiXe,
                TaiXeTBSL = x.TaiXeTbs,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).FirstOrDefaultAsync();

            return driver;
        }

        public async Task<List<GetDriverRequest>> GetListByStatus(string status)
        {
            var driver = await _context.TaiXes.Where(x => x.TrangThai == status).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaThau = x.MaNhaThau,
                LoaiXe = x.LoaiXe,
                TaiXeTBSL = x.TaiXeTbs,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).ToListAsync();

            return driver;
        }

        public async Task<List<GetDriverRequest>> GetListByVehicleType(string vehicleType)
        {
            var driver = await _context.TaiXes.Where(x => x.LoaiXe == vehicleType).Select(x => new GetDriverRequest()
            {
                MaTaiXe = x.MaTaiXe,
                Cccd = x.Cccd,
                HoVaTen = x.HoVaTen,
                SoDienThoai = x.SoDienThoai,
                NgaySinh = x.NgaySinh,
                GhiChu = x.GhiChu,
                MaNhaThau = x.MaNhaThau,
                LoaiXe = x.LoaiXe,
                TaiXeTBSL = x.TaiXeTbs,
                TrangThai = x.TrangThai,
                Createdtime = x.CreatedTime,
                UpdateTime = x.UpdatedTime,
            }).ToListAsync();

            return driver;
        }

        public async Task<PagedResponseCustom<ListDriverRequest>> getListDriver(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var getData = from driver in _context.TaiXes
                              orderby driver.CreatedTime descending
                              select new { driver };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.driver.MaTaiXe.ToLower().Contains(filter.Keyword.ToLower()));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.driver.CreatedTime.Date >= filter.fromDate && x.driver.CreatedTime <= filter.toDate);
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
                    MaNhaThau = x.driver.MaNhaThau,
                    LoaiXe = x.driver.LoaiXe,
                    TaiXeTBSL = x.driver.TaiXeTbs,
                    TrangThai = x.driver.TrangThai,
                    UpdateTime = x.driver.UpdatedTime,
                    Createdtime = x.driver.CreatedTime,
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
    }
}