using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.CustommerModel;

namespace TBSLogistics.Service.Repository.CustommerManage
{
    public class CustomerService : ICustomer
    {
        private readonly TMSContext _TMSContext;

        public CustomerService(TMSContext TMSContext)
        {
            _TMSContext = TMSContext;
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
                    MaBangGia = request.MaBangGia,
                    Createdtime = DateTime.Now,
                    UpdateTime = DateTime.Now
                });

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
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
                GetCustommer.MaBangGia = request.MaBangGia;
                GetCustommer.Createdtime = DateTime.Now;

                _TMSContext.Update(GetCustommer);

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật khách hàng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật khách hàng thất bại!" };
                }
            }
            catch (Exception ex)
            {
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
                MaBangGia = x.MaBangGia
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
                MaBangGia = x.MaBangGia
            }).ToListAsync();

            return getListCustommer;
        }
    }
}
