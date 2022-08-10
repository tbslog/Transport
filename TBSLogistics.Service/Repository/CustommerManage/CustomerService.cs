using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.CustommerManage
{
    public class CustomerService : ICustomer
    {
        private readonly TMSContext _TMSContext;
        private readonly ICommon _common;

        public CustomerService(TMSContext TMSContext, ICommon common)
        {
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

                await _TMSContext.AddAsync(new KhachHang()
                {
                    MaKh = request.MaKh,
                    TenKh = request.TenKh,
                    MaSoThue = request.MaSoThue,
                    Sdt = request.Sdt,
                    Email = request.Email,
                    MaDiaDiem = request.MaDiaDiem,
                    Createdtime = DateTime.Now,
                    UpdateTime = DateTime.Now
                });

                var result = await _TMSContext.SaveChangesAsync();

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

                GetCustommer.TenKh = request.TenKh;
                GetCustommer.MaSoThue = request.MaSoThue;
                GetCustommer.Sdt = request.Sdt;
                GetCustommer.Email = request.Email;
                GetCustommer.MaDiaDiem = request.MaDiaDiem;
                GetCustommer.Createdtime = DateTime.Now;

                _TMSContext.Update(GetCustommer);

                var result = await _TMSContext.SaveChangesAsync();

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
            var getCustommer = await _TMSContext.KhachHangs.Where(x => x.MaKh == CustomerId).Select(x => new GetCustomerRequest()
            {
                MaKh = x.MaKh,
                TenKh = x.TenKh,
                MaSoThue = x.MaSoThue,
                Sdt = x.Sdt,
                Email = x.Email,
                MaDiaDiem = x.MaDiaDiem,
            }).FirstOrDefaultAsync();

            return getCustommer;
        }

        public async Task<List<GetCustomerRequest>> GetListCustomer()
        {
            var getListCustommer = await _TMSContext.KhachHangs.Select(x => new GetCustomerRequest()
            {
                MaKh = x.MaKh,
                TenKh = x.TenKh,
                MaSoThue = x.MaSoThue,
                Sdt = x.Sdt,
                Email = x.Email,
                MaDiaDiem = x.MaDiaDiem,
            }).ToListAsync();

            return getListCustommer;
        }

        public async Task<PagedResponseCustom<ListCustommerRequest>> getListCustommer(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from cus in _TMSContext.KhachHangs
                               join address in _TMSContext.DiaDiems
                               on cus.MaDiaDiem equals address.MaDiaDiem
                               orderby cus.Createdtime descending
                               select new { cus, address };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.cus.MaKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.cus.Createdtime.Date >= filter.fromDate.Date && x.cus.Createdtime.Date <= filter.toDate.Date);
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
                    Createdtime = x.cus.Createdtime,
                    UpdateTime = x.cus.UpdateTime,
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
    }
}
