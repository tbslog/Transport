using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

                string fullAddress = await _address.GetFullAddress(request.Address.SoNha, request.Address.MaTinh, request.Address.MaHuyen, request.Address.MaHuyen);

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

                var getAddress = await _TMSContext.DiaDiems.Where(x => x.MaDiaDiem == GetCustommer.MaDiaDiem).FirstOrDefaultAsync();

                getAddress.TenDiaDiem = request.Address.TenDiaDiem;
                getAddress.MaQuocGia = request.Address.MaQuocGia;
                getAddress.MaTinh = request.Address.MaTinh;
                getAddress.MaHuyen = request.Address.MaHuyen;
                getAddress.MaPhuong = request.Address.MaPhuong;
                getAddress.SoNha = request.Address.SoNha;
                getAddress.DiaChiDayDu = request.Address.DiaChiDayDu;
                getAddress.MaGps = request.Address.MaGps;
                getAddress.MaLoaiDiaDiem = request.Address.MaLoaiDiaDiem;
                getAddress.UpdatedTime = DateTime.Now;


                GetCustommer.TenKh = request.TenKh;
                GetCustommer.MaSoThue = request.MaSoThue;
                GetCustommer.Sdt = request.Sdt;
                GetCustommer.Email = request.Email;
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
                    MaLoaiDiaDiem = x.address.MaLoaiDiaDiem,
                    TenDiaDiem = x.address.TenDiaDiem,
                    MaGps = x.address.MaGps,
                    sonha = x.address.SoNha,
                    mahuyen = x.address.MaHuyen,
                    maphuong = x.address.MaPhuong,
                    maquocgia = x.address.MaQuocGia,
                    matinh = x.address.MaTinh,
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
                               orderby cus.Createdtime descending
                               select new { cus, address };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x => x.cus.MaKh.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    listData = listData.Where(x => x.cus.Createdtime.Date >= filter.fromDate && x.cus.Createdtime.Date <= filter.toDate);
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
