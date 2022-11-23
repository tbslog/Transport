using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
                var checkExists = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.MaHopDong).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này đã tồn tại" };
                }

                var checkCustommer = await _TMSContext.KhachHang.Where(x => x.MaKh == request.MaKh).FirstOrDefaultAsync();

                if (checkCustommer == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã khách hàng không tồn tại" };
                }

                if (string.IsNullOrEmpty(request.SoHopDongCha))
                {
                    if (request.NgayThanhToan == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không được bỏ trống ngày thanh toán" };
                    }

                    var checkContractCustommer = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaKh == request.MaKh && x.MaHopDong == null).FirstOrDefaultAsync();
                    if (checkContractCustommer != null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Một khách hàng chỉ có một hợp đồng chính" };
                    }
                }
                else
                {
                    request.NgayThanhToan = null;
                    var checkChildContract = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.SoHopDongCha && x.MaHopDongCha == null).FirstOrDefaultAsync();
                    if (checkChildContract == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Chỉ hợp đồng chính mới được có phụ lục" };
                    }
                }


                if (request.ThoiGianBatDau >= request.ThoiGianKetThuc)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thời gian bắt đầu không được lớn hơn hoặc bằng thời gian kết thúc" };
                }

                await _TMSContext.AddAsync(new HopDongVaPhuLuc()
                {
                    MaHopDong = request.MaHopDong.ToUpper(),
                    TenHienThi = request.TenHienThi,
                    MaLoaiHopDong = checkCustommer.MaLoaiKh == "KH" ? "SELL" : "BUY",
                    MaHopDongCha = request.SoHopDongCha,
                    ThoiGianBatDau = request.ThoiGianBatDau,
                    ThoiGianKetThuc = request.ThoiGianKetThuc,
                    NgayThanhToan = request.NgayThanhToan,
                    MaKh = request.MaKh,
                    GhiChu = request.GhiChu,
                    MaPhuPhi = request.PhuPhi,
                    TrangThai = 24,
                    UpdatedTime = DateTime.Now,
                    CreatedTime = DateTime.Now
                });

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    if (request.File != null)
                    {
                        await UploadFile(request.File, request.TenHienThi, request.MaHopDong);
                    }

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
                var checkExists = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == id).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Hợp đồng này không tồn tại" };
                }

                if (request.ThoiGianBatDau >= request.ThoiGianKetThuc)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thời gian bắt đầu không được lớn hơn hoặc bằng thời gian kết thúc" };
                }

                if (!string.IsNullOrEmpty(checkExists.MaHopDongCha))
                {
                    if (request.NgayThanhToan == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không được bỏ trống ngày thanh toán" };
                    }
                }
                else
                {
                    request.NgayThanhToan = null;
                }

                checkExists.NgayThanhToan = request.NgayThanhToan;
                checkExists.TenHienThi = request.TenHienThi;
                checkExists.ThoiGianBatDau = request.ThoiGianBatDau;
                checkExists.ThoiGianKetThuc = request.ThoiGianKetThuc;
                checkExists.GhiChu = request.GhiChu;
                checkExists.TrangThai = request.TrangThai;
                checkExists.UpdatedTime = DateTime.Now;

                _TMSContext.Update(checkExists);

                var result = await _TMSContext.SaveChangesAsync();

                if (result > 0)
                {
                    if (request.File != null)
                    {
                        await UploadFile(request.File, request.TenHienThi, id);
                    }

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

                var getDataFile = await _TMSContext.Attachment.Where(x => x.MaHopDong == id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();

                return new GetContractById()
                {
                    NgayThanhToan = getContractById.NgayThanhToan,
                    MaHopDong = getContractById.MaHopDong,
                    SoHopDongCha = getContractById.MaHopDongCha,
                    TenHienThi = getContractById.TenHienThi,
                    MaKh = getContractById.MaKh,
                    PhanLoaiHopDong = getContractById.MaLoaiHopDong,
                    ThoiGianBatDau = getContractById.ThoiGianBatDau,
                    ThoiGianKetThuc = getContractById.ThoiGianKetThuc,
                    GhiChu = getContractById.GhiChu,
                    PhuPhi = getContractById.MaPhuPhi,
                    TrangThai = getContractById.TrangThai,
                    File = getDataFile == null ? null : getDataFile.Id.ToString(),
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
                               join tt in _TMSContext.StatusText
                               on contract.TrangThai equals tt.StatusId
                               where tt.LangId == TempData.LangID
                               orderby contract.UpdatedTime descending
                               select new
                               {
                                   contract,
                                   cus,
                                   tt
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
                    NgayThanhToan = x.contract.NgayThanhToan,
                    MaHopDong = x.contract.MaHopDong,
                    TenHienThi = x.contract.TenHienThi,
                    SoHopDongCha = x.contract.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
                    MaKh = x.contract.MaKh,
                    TenKH = x.cus.TenKh,
                    PhanLoaiHopDong = x.contract.MaLoaiHopDong,
                    TrangThai = x.tt.StatusContent,
                    ThoiGianBatDau = x.contract.ThoiGianBatDau,
                    ThoiGianKetThuc = x.contract.ThoiGianKetThuc,
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

        public async Task<List<GetContractById>> GetListContractSelect(string MaKH, bool getChild, bool getProductService)
        {
            var getList = from ct in _TMSContext.HopDongVaPhuLuc
                          join kh in _TMSContext.KhachHang on ct.MaKh equals kh.MaKh
                          orderby ct.MaHopDong descending
                          select new { ct, kh };

            if (!string.IsNullOrEmpty(MaKH))
            {
                getList = getList.Where(x => x.ct.MaKh == MaKH);
            }

            if (getChild == false)
            {
                getList = getList.Where(x => x.ct.MaHopDongCha == null);
            }

            if (getProductService == false)
            {
                getList = getList.Where(x => x.ct.MaHopDong != "SPDV_TBSL");
            }

            var list = await getList.Select(x => new GetContractById()
            {
                MaHopDong = x.ct.MaHopDong,
                TenHienThi = x.ct.TenHienThi,
                SoHopDongCha = x.ct.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
            }).ToListAsync();

            return list;
        }

        private async Task<BoolActionResult> UploadFile(IFormFile file, string cusName, string maHopDong)
        {
            var PathFolder = $"Contract/{cusName}";

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), Guid.NewGuid().ToString());
            var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";

            var supportedTypes = new[] { "pdf", "docx", "xlsx" };
            var fileExt = Path.GetExtension(originalFileName).Substring(1);
            if (!supportedTypes.Contains(fileExt))
            {
                return new BoolActionResult { isSuccess = false, Message = "File không được hỗ trợ, chỉ hỗ trợ .pdf, .docx, .xlsx" };
            }

            var attachment = new Attachment()
            {
                FileName = fileName,
                FilePath = _common.GetFileUrl(fileName, PathFolder),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                FolderName = "Contract",
                MaHopDong = maHopDong,
                UploadedTime = DateTime.Now
            };

            var add = await _common.AddAttachment(attachment);

            if (add.isSuccess == false)
            {
                return new BoolActionResult { isSuccess = false, Message = add.Message };
            }
            await _common.SaveFileAsync(file.OpenReadStream(), fileName, PathFolder);

            return new BoolActionResult { isSuccess = true }; ;
        }
    }
}
