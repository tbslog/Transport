using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.SupplierModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.SupplierManage
{
    public class SupplierService : ISupplier
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;

        public SupplierService(TMSContext context, ICommon common)
        {
            _context = context;
            _common = common;
        }

        public async Task<BoolActionResult> CreateSupplier(CreateSupplierRequest request)
        {
            try
            {
                var checkExists = await _context.NhaCungCaps.Where(x => x.MaNhaCungCap == request.MaNhaCungCap).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Nhà cung cấp đã tồn tại" };
                }

                await _context.AddAsync(new NhaCungCap()
                {
                    MaNhaCungCap = request.MaNhaCungCap,
                    TenNhaCungCap = request.TenNhaCungCap,
                    Sdt = request.Sdt,
                    Email = request.Email,
                    LoaiDichVu = request.LoaiDichVu,
                    MaSoThue = request.MaSoThue,
                    MaDiaDiem = request.MaDiaDiem,
                    LoaiNhaCungCap = request.LoaiNhaCungCap,
                    MaHopDong = request.MaHopDong,
                    Createdtime = DateTime.Now,
                    UpdateTime = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SupplierManage", "UserId: " + TempData.UserID + " create new Supplier with id: " + request.MaNhaCungCap);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới nhà cung cấp thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới nhà cung cấp thất bại" };
                }

            }
            catch (Exception ex)
            {
                await _common.Log("SupplierManage", "UserId: " + TempData.UserID + " create new Supplier with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditSupplier(string SupplierId, UpdateSupplierRequest request)
        {
            try
            {
                var getSupplier = await _context.NhaCungCaps.Where(x => x.MaNhaCungCap == SupplierId).FirstOrDefaultAsync();

                if (getSupplier == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Nhà cung cấp không tồn tại" };
                }

                getSupplier.TenNhaCungCap = request.TenNhaCungCap;
                getSupplier.Sdt = request.Sdt;
                getSupplier.Email = request.Email;
                getSupplier.LoaiDichVu = request.LoaiDichVu;
                getSupplier.MaSoThue = request.MaSoThue;
                getSupplier.MaDiaDiem = request.MaDiaDiem;
                getSupplier.LoaiNhaCungCap = request.LoaiNhaCungCap;
                getSupplier.MaHopDong = request.MaHopDong;
                getSupplier.UpdateTime = DateTime.Now;

                _context.Update(getSupplier);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SupplierManage", "UserId: " + TempData.UserID + " Edit Supplier with id: " + SupplierId);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới nhà cung cấp thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới nhà cung cấp thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SupplierManage", "UserId: " + TempData.UserID + " Edit Supplier with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<List<GetSupplierRequest>> GetListSupplier()
        {
            var ListSupplier = await _context.NhaCungCaps.Select(x => new GetSupplierRequest()
            {
                MaNhaCungCap = x.MaNhaCungCap,
                TenNhaCungCap = x.TenNhaCungCap,
                Sdt = x.Sdt,
                Email = x.Email,
                LoaiDichVu = x.LoaiDichVu,
                MaSoThue = x.MaSoThue,
                MaDiaDiem = x.MaDiaDiem,
                LoaiNhaCungCap = x.LoaiNhaCungCap,
                MaHopDong = x.MaHopDong,
                UpdateTime = x.UpdateTime,
                Createdtime = x.Createdtime,
            }).ToListAsync();

            return ListSupplier;
        }

        public async Task<GetSupplierRequest> GetSupplierById(string SupplierId)
        {
            var getSupplier = await _context.NhaCungCaps.Where(x => x.MaNhaCungCap == SupplierId).Select(x => new GetSupplierRequest()
            {
                MaNhaCungCap = x.MaNhaCungCap,
                TenNhaCungCap = x.TenNhaCungCap,
                Sdt = x.Sdt,
                Email = x.Email,
                LoaiDichVu = x.LoaiDichVu,
                MaSoThue = x.MaSoThue,
                MaDiaDiem = x.MaDiaDiem,
                LoaiNhaCungCap = x.LoaiNhaCungCap,
                MaHopDong = x.MaHopDong,
                UpdateTime = x.UpdateTime,
                Createdtime = x.Createdtime,
            }).FirstOrDefaultAsync();

            return getSupplier;
        }
    }
}
