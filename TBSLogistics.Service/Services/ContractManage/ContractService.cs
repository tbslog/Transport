using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.ContractManage
{
    public class ContractService : IContract
    {
        private readonly TMSContext _TMSContext;
        private readonly ICommon _common;

        public ContractService(TMSContext tMSContext, ICommon common)
        {
            _TMSContext = tMSContext;
            _common = common;
        }

        public async Task<BoolActionResult> CreateContract(CreateContract request)
        {
            try
            {
                var checkExists = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.MaHopDong || x.TenHienThi == request.TenHienThi).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này đã tồn tại" };
                }

                var checkCustommer = await _TMSContext.KhachHang.Where(x => x.MaKh == request.MaKh).FirstOrDefaultAsync();

                if (checkCustommer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã khách hàng không tồn tại" };
                }

                if (request.ThoiGianBatDau >= request.ThoiGianKetThuc)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thời gian bắt đầu không được lớn hơn hoặc bằng thời gian kết thúc" };
                }


                await _TMSContext.AddAsync(new HopDongVaPhuLuc()
                {
                    MaHopDong = request.MaHopDong,
                    TenHienThi = request.TenHienThi,
                    MaLoaiHopDong = request.PhanLoaiHopDong,
                    SoHopDongCha = request.SoHopDongCha,
                    ThoiGianBatDau = request.ThoiGianBatDau,
                    ThoiGianKetThuc = request.ThoiGianKetThuc,
                    MaKh = request.MaKh,
                    MaPtvc = request.MaPtvc,
                    GhiChu = request.GhiChu,
                    PhuPhi = request.PhuPhi,
                    TrangThai = request.TrangThai,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ContractManage", "UserId: " + TempData.UserID + " create new Contract with Id: " + request.MaHopDong);
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới hợp đồng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới hợp đồng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("ContractManage", "UserId: " + TempData.UserID + " create new Contract has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> EditContract(string id, EditContract request)
        {
            try
            {
                var checkExists = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == id || x.TenHienThi == request.TenHienThi).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này không tồn tại" };
                }

                if (request.ThoiGianBatDau >= request.ThoiGianKetThuc)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thời gian bắt đầu không được lớn hơn hoặc bằng thời gian kết thúc" };
                }

                checkExists.TenHienThi = request.TenHienThi;
                checkExists.MaLoaiHopDong = request.PhanLoaiHopDong;
                checkExists.SoHopDongCha = request.SoHopDongCha;
                checkExists.ThoiGianBatDau = request.ThoiGianBatDau;
                checkExists.ThoiGianKetThuc = request.ThoiGianKetThuc;
                checkExists.MaPtvc = request.MaPtvc;
                checkExists.GhiChu = request.GhiChu;
                checkExists.PhuPhi = request.PhuPhi;
                checkExists.TrangThai = request.TrangThai;
                checkExists.UpdatedTime = DateTime.Now;

                _TMSContext.Update(checkExists);

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("ContractManage", "UserId: " + TempData.UserID + " Update  Contract with Id: " + id);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật hợp đồng thành công!" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật hợp đồng thất bại!" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("ContractManage", "UserId: " + TempData.UserID + " Update Contract has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetContractById> GetContractById(string id)
        {
            try
            {
                var getContractById = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == id).FirstOrDefaultAsync();

                var getDataFile = await _TMSContext.Attachment.Where(x => x.FileName.Contains(id)).FirstOrDefaultAsync();

                var file = getDataFile == null ? 0 : getDataFile.Id;

                return new GetContractById()
                {
                    MaHopDong = getContractById.MaHopDong,
                    SoHopDongCha = getContractById.SoHopDongCha,
                    TenHienThi = getContractById.TenHienThi,
                    MaKh = getContractById.MaKh,
                    MaPtvc = getContractById.MaPtvc,
                    PhanLoaiHopDong = getContractById.MaLoaiHopDong,
                    ThoiGianBatDau = getContractById.ThoiGianBatDau,
                    ThoiGianKetThuc = getContractById.ThoiGianKetThuc,
                    GhiChu = getContractById.GhiChu,
                    PhuPhi = getContractById.PhuPhi,
                    TrangThai = getContractById.TrangThai,
                    File = file.ToString()
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PagedResponseCustom<ListContract>> GetListContract(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

                var listData = from contract in _TMSContext.HopDongVaPhuLuc
                               join cus in _TMSContext.KhachHang
                               on contract.MaKh equals cus.MaKh
                               join pricetable in _TMSContext.BangGia
                               on contract.MaHopDong equals pricetable.MaHopDong
                               into cp
                               from contractPriceTbl in cp.DefaultIfEmpty()
                               orderby contract.UpdatedTime descending
                               select new
                               {
                                   contract,
                                   cus,
                                   contractPriceTbl
                               };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    listData = listData.Where(x =>
                       x.contract.MaHopDong.Contains(filter.Keyword.ToLower())
                    || x.contract.MaKh.Contains(filter.Keyword.ToLower())
                    || x.contract.TenHienThi.Contains(filter.Keyword.ToLower())

                    );
                }

                if (!string.IsNullOrEmpty(filter.contractType))
                {
                    listData = listData.Where(x => x.contract.MaLoaiHopDong == filter.contractType);
                }


                if (!string.IsNullOrEmpty(filter.customerType))
                {
                    listData = listData.Where(x => x.cus.MaLoaiKh == filter.customerType);
                }


                if (!string.IsNullOrEmpty(filter.fromDate.ToString().Trim()) && !string.IsNullOrEmpty(filter.toDate.ToString().Trim()))
                {
                    listData = listData.Where(x => x.contract.UpdatedTime >= filter.fromDate && x.contract.UpdatedTime <= filter.toDate);
                }

                var totalCount = await listData.CountAsync();

                var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListContract()
                {
                    MaHopDong = x.contract.MaHopDong,
                    TenHienThi = x.contract.TenHienThi,
                    MaKh = x.contract.MaKh,
                    TenKH = x.cus.TenKh,
                    PhanLoaiHopDong = x.contract.MaLoaiHopDong,
                    TrangThai = x.contract.TrangThai,
                    ThoiGianBatDau = x.contract.ThoiGianBatDau,
                    ThoiGianKetThuc = x.contract.ThoiGianKetThuc,
                    MaBangGia = x.contractPriceTbl.MaBangGia
                }).ToListAsync();

                return new PagedResponseCustom<ListContract>()
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

        public async Task<List<GetContractById>> GetListContractSelect(string MaKH)
        {
            var getList = from ct in _TMSContext.HopDongVaPhuLuc
                          join kh in _TMSContext.KhachHang on ct.MaKh equals kh.MaKh
                          orderby ct.MaHopDong descending
                          where ct.SoHopDongCha ==null
                          select new { ct, kh };

            if (!string.IsNullOrEmpty(MaKH))
            {
                getList = getList.Where(x => x.ct.MaKh == MaKH);
            }

            var list = await getList.Select(x => new GetContractById() { 
                MaHopDong = x.ct.MaHopDong,
                TenHienThi = x.ct.TenHienThi,
            }).ToListAsync();

            return list;
        }
    }
}
