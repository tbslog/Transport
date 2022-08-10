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
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Repository.AddressManage
{
    public class AddressService : IAddress
    {
        private readonly TMSContext _VanChuyenContext;
        private readonly ICommon _common;

        public AddressService(TMSContext vanChuyenContext, ICommon common)
        {
            _common = common;
            _VanChuyenContext = vanChuyenContext;
        }

        public async Task<BoolActionResult> CreateAddress(CreateAddressRequest request)
        {
            try
            {
                var checkExists = await _VanChuyenContext.DiaDiems.Where(x => x.TenDiaDiem == request.TenDiaDiem).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Địa điểm này đã tồn tại" };
                }

                var getProvince = await _VanChuyenContext.TinhThanhs.Where(x => x.MaTinh == request.MaTinh).Select(x => x.TenTinh).FirstOrDefaultAsync();
                var getDistrict = await _VanChuyenContext.QuanHuyens.Where(x => x.MaHuyen == request.MaHuyen).Select(x => x.TenHuyen).FirstOrDefaultAsync();
                var getWard = await _VanChuyenContext.XaPhuongs.Where(x => x.MaPhuong == request.MaPhuong).Select(x => x.TenPhuong).FirstOrDefaultAsync();

                await _VanChuyenContext.AddAsync(new DiaDiem()
                {
                    TenDiaDiem = request.TenDiaDiem,
                    MaQuocGia = request.MaQuocGia,
                    MaTinh = request.MaTinh,
                    MaHuyen = request.MaHuyen,
                    MaPhuong = request.MaPhuong,
                    SoNha = request.SoNha,
                    DiaChiDayDu = request.SoNha + ", " + getWard + ", " + getDistrict + ", " + getProvince,
                    MaGps = request.MaGps,
                    MaLoaiDiaDiem = request.MaLoaiDiaDiem,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                var result = await _VanChuyenContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("AddressManage", "UserId: " + TempData.UserID + " create new Address with name: " + request.TenDiaDiem);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới địa điểm thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới địa điểm thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("AddressManage", "UserId: " + TempData.UserID + " create new Address with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode)
        {
            var add = await _VanChuyenContext.AddAsync(new QuanHuyen()
            {
                MaHuyen = mahuyen,
                TenHuyen = tenhuyen,
                PhanLoai = phanloai,
                ParentCode = parentcode
            });

            await _VanChuyenContext.SaveChangesAsync();

            return new BoolActionResult { isSuccess = true, Message = "OK" };
        }

        public async Task<BoolActionResult> CreateProvince(int matinh, string tentinh, string phanloai)
        {
            var add = await _VanChuyenContext.AddAsync(new TinhThanh()
            {
                MaTinh = matinh,
                TenTinh = tentinh,
                PhanLoai = phanloai
            });

            await _VanChuyenContext.SaveChangesAsync();

            return new BoolActionResult { isSuccess = true, Message = "OK" };
        }

        public async Task<BoolActionResult> CreateWard(List<WardModel> request)
        {
            var model = request.Select(x => new XaPhuong()
            {
                MaPhuong = x.maphuong,
                PhanLoai = x.phanloai,
                TenPhuong = x.tenphuong,
                ParentCode = x.parentcode
            });

            await _VanChuyenContext.XaPhuongs.AddRangeAsync(model);

            await _VanChuyenContext.SaveChangesAsync();

            return new BoolActionResult { isSuccess = true, Message = "OK" };
        }

        public async Task<BoolActionResult> EditAddress(int id, UpdateAddressRequest request)
        {
            try
            {
                var getAddress = await _VanChuyenContext.DiaDiems.Where(x => x.MaDiaDiem == id).FirstOrDefaultAsync();

                if (getAddress == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Địa điểm không tồn tại" };
                }

                getAddress.TenDiaDiem = request.TenDiaDiem;
                getAddress.MaQuocGia = request.MaQuocGia;
                getAddress.MaTinh = request.MaTinh;
                getAddress.MaHuyen = request.MaHuyen;
                getAddress.MaPhuong = request.MaPhuong;
                getAddress.SoNha = request.SoNha;
                getAddress.MaGps = request.MaGps;
                getAddress.MaLoaiDiaDiem = request.MaLoaiDiaDiem;
                getAddress.UpdatedTime = DateTime.Now;

                if (getAddress.DiaChiDayDu != request.DiaChiDayDu)
                {
                    var getProvince = await _VanChuyenContext.TinhThanhs.Where(x => x.MaTinh == request.MaTinh).Select(x => x.TenTinh).FirstOrDefaultAsync();
                    var getDistrict = await _VanChuyenContext.QuanHuyens.Where(x => x.MaHuyen == request.MaHuyen).Select(x => x.TenHuyen).FirstOrDefaultAsync();
                    var getWard = await _VanChuyenContext.XaPhuongs.Where(x => x.MaPhuong == request.MaPhuong).Select(x => x.TenPhuong).FirstOrDefaultAsync();
                    getAddress.DiaChiDayDu = request.SoNha + ", " + getWard + ", " + getDistrict + ", " + getProvince;
                }

                _VanChuyenContext.Update(getAddress);

                var result = await _VanChuyenContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("AddressManage", "UserId: " + TempData.UserID + " edit Address with name: " + request.TenDiaDiem);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật địa điểm thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật địa điểm thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("AddressManage", "UserId: " + TempData.UserID + " edit Address with ERRORS: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<DiaDiem> GetAddressById(int IdAddress)
        {
            var getAddress = await _VanChuyenContext.DiaDiems.Where(x => x.MaDiaDiem == IdAddress).FirstOrDefaultAsync();

            return getAddress;
        }

        public async Task<List<QuanHuyen>> GetDistricts(int IdProvince)
        {
            var ListDistricts = await _VanChuyenContext.QuanHuyens.Where(x => x.ParentCode == IdProvince).ToListAsync();
            return ListDistricts;
        }

        public async Task<PagedResponseCustom<AddressModel>> GetListAddress(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var getData = from ar in _VanChuyenContext.DiaDiems
                              orderby ar.CreatedTime descending
                              select new { ar };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getData = getData.Where(x => x.ar.TenDiaDiem.ToLower().Contains(filter.Keyword.ToLower()));
                }

                if(!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getData = getData.Where(x => x.ar.CreatedTime.Date >= filter.fromDate.Date && x.ar.CreatedTime <= filter.toDate.Date);
                }

                var totalRecords = await getData.CountAsync();

                var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new AddressModel()
                {
                    MaDiaDiem = x.ar.MaDiaDiem,
                    TenDiaDiem = x.ar.TenDiaDiem,
                    DiaChiDayDu = x.ar.DiaChiDayDu,
                    MaGps = x.ar.MaGps,
                    MaLoaiDiaDiem = x.ar.MaLoaiDiaDiem,
                    CreatedTime = x.ar.CreatedTime,
                    UpdatedTime = x.ar.UpdatedTime,
                }).ToListAsync();

                return new PagedResponseCustom<AddressModel>()
                {
                    paginationFilter = validFilter,
                    totalCount = totalRecords,
                    dataResponse = pagedData
                };
            }
            catch (Exception ex)
            {
                return new PagedResponseCustom<AddressModel>();
            }


        }

        public async Task<List<TinhThanh>> GetProvinces()
        {
            var ListProvinces = await _VanChuyenContext.TinhThanhs.ToListAsync();
            return ListProvinces;
        }

        public async Task<List<XaPhuong>> GetWards(int IdDistricts)
        {
            var ListWards = await _VanChuyenContext.XaPhuongs.Where(x => x.ParentCode == IdDistricts).ToListAsync();
            return ListWards;
        }
    }
}
