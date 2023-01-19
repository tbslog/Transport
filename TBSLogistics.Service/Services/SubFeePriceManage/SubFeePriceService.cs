using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.SubFeePriceManage
{
    public class SubFeePriceService : ISubFeePrice
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public SubFeePriceService(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _common = common;
            _context = context;
            tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
        }

        public async Task<BoolActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request)
        {
            try
            {
                if (request.UnitPrice < 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn giá phải lớn hơn 0" };
                }

                if (!string.IsNullOrEmpty(request.GoodsType))
                {
                    var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == request.GoodsType).FirstOrDefaultAsync();

                    if (checkGoodsType == null)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa không tồn tại" };
                    }
                }
                else
                {
                    request.GoodsType = null;
                }

                await _context.AddAsync(new SubFeePrice()
                {
                    CusType = request.CusType,
                    ContractId = string.IsNullOrEmpty(request.ContractId) ? null : request.ContractId,
                    SfId = request.SfId,
                    GoodsType = request.GoodsType,
                    FirstPlace = request.FirstPlace,
                    SecondPlace = request.SecondPlace,
                    UnitPrice = request.UnitPrice,
                    SfStateByContract = 2,
                    Status = 13,
                    Description = request.Description,
                    Creator = tempData.UserName,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Create SubFeePrice with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới phụ phí thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới phụ phí thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " create new SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> UpdateSubFeePrice(long id, UpdateSubFeePriceRequest request)
        {
            try
            {
                var getSubFeePrice = await _context.SubFeePrice.Where(x => x.PriceId == id).FirstOrDefaultAsync();

                if (getSubFeePrice == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phụ phí không tồn tại" };
                }

                if (getSubFeePrice.Status != 13)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không thể chỉnh sửa phụ phí này nữa" };
                }

                var checkExists = await _context.SubFeePrice.Where(x => x.ContractId == request.ContractId && x.SfId == request.SfId
                && x.GoodsType == request.GoodsType && x.FirstPlace == request.FirstPlace && x.SecondPlace == request.SecondPlace
                && x.UnitPrice == request.UnitPrice && x.Status == 14
                ).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phụ phí đã tồn tại" };
                }

                getSubFeePrice.CusType = request.CusType;
                getSubFeePrice.ContractId = request.ContractId;
                getSubFeePrice.SfId = request.SfId;
                getSubFeePrice.GoodsType = request.GoodsType;
                getSubFeePrice.FirstPlace = request.FirstPlace;
                getSubFeePrice.SecondPlace = request.SecondPlace;
                getSubFeePrice.UnitPrice = request.UnitPrice;
                getSubFeePrice.Description = request.Description;
                getSubFeePrice.UpdatedDate = DateTime.Now;
                getSubFeePrice.Updater = tempData.UserName;

                _context.Update(getSubFeePrice);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Update SubFeePrice with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật phụ phí thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật phụ phí thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " update SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> ApproveSubFeePrice(List<ApproveSubFee> request)
        {
            try
            {
                if (request.Count < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }

                var Errors = "";

                int line = 0;

                foreach (var item in request)
                {
                    line += 1;
                    var getById = await _context.SubFeePrice.Where(x => x.PriceId == item.SubFeePriceId).FirstOrDefaultAsync();

                    if (getById == null)
                    {
                        Errors += "Mã  phụ phí: " + item.SubFeePriceId + ", không tồn tại trong hệ thống";
                        continue;
                    }

                    if (item.Selection == 1)
                    {
                        getById.Status = 15;
                        getById.Approver = tempData.UserName;
                        getById.ApprovedDate = DateTime.Now;
                        _context.Update(getById);
                    }

                    if (item.Selection == 0)
                    {
                        var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == getById.ContractId).FirstOrDefaultAsync();

                        if (getContract != null)
                        {
                            var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == getContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();

                            if (getNewestContract.MaHopDong != getById.ContractId)
                            {
                                Errors += "Mã  phụ phí: " + item.SubFeePriceId + ", không sử dụng Hợp Đồng/Phụ Lục mới nhất, Vui lòng chọn lại Hợp Đồng/Phụ Lục mới nhất \r\n";
                                continue;
                            }
                        }

                        var checkExists = await _context.SubFeePrice.Where(x => x.CusType == getById.CusType && x.ContractId == getById.ContractId && x.SfId == getById.SfId
                         && x.GoodsType == getById.GoodsType && x.FirstPlace == getById.FirstPlace && x.SecondPlace == getById.SecondPlace
                         && x.Status == 14).FirstOrDefaultAsync();

                        if (checkExists != null)
                        {
                            checkExists.Status = 12;
                            checkExists.DeactiveDate = DateTime.Now;
                            _context.Update(checkExists);
                        }

                        getById.ApprovedDate = DateTime.Now;
                        getById.Approver = tempData.UserName;
                        getById.Status = 14;
                        _context.Update(getById);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Approve SubFeePrice with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = Errors == "" ? "Duyệt phụ phí thành công!" : Errors };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Duyệt phụ phí thất bại!," + Errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> DisableSubFeePrice(List<long> ids)
        {
            try
            {
                if (ids.Count < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }

                string errors = "";

                foreach (var item in ids)
                {
                    var checkExists = await _context.SubFeePrice.Where(x => x.PriceId == item).FirstOrDefaultAsync();

                    if (checkExists == null)
                    {
                        errors += "Mã phụ phí:" + item + ", không tồn tại trong hệ thống \r\n";
                        continue;
                    }

                    if (checkExists.Status != 14)
                    {
                        errors += "Mã phụ phí:" + item + ", không được áp dụng \r\n";
                        continue;
                    }
                    checkExists.Updater = tempData.UserName;
                    checkExists.Status = 12;
                    _context.Update(checkExists);
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Disable SubFeePrice with data: " + JsonSerializer.Serialize(ids));
                    return new BoolActionResult { isSuccess = true, Message = errors == "" ? "Vô hiệu phụ phí thành công!" : errors };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vô hiệu hóa phụ phí thất bại" + errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " disable SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> DeleteSubFeePrice(List<long> ids)
        {
            try
            {
                if (ids.Count < 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }

                string errors = "";

                foreach (var item in ids)
                {
                    var checkExists = await _context.SubFeePrice.Where(x => x.PriceId == item).FirstOrDefaultAsync();

                    if (checkExists == null)
                    {
                        errors += "Mã phụ phí:" + item + ", không tồn tại trong hệ thống \r\n";
                        continue;
                    }

                    if (checkExists.Status != 12)
                    {
                        errors += "Mã phụ phí:" + item + ", không thể xóa \r\n";
                        continue;
                    }

                    checkExists.Updater = tempData.UserName;
                    checkExists.Status = 16;
                    _context.Update(checkExists);
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Disable SubFeePrice with data: " + JsonSerializer.Serialize(ids));
                    return new BoolActionResult { isSuccess = true, Message = errors == "" ? "Xóa phụ phí thành công!" : errors };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Xóa hóa phụ phí thất bại" + errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + "delete SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetSubFeePriceRequest> GetSubFeePriceById(long id)
        {
            var getById = from sf in _context.SubFee
                          join sfp in _context.SubFeePrice
                          on sf.SubFeeId equals sfp.SfId
                          join hd in _context.HopDongVaPhuLuc
                          on sfp.ContractId equals hd.MaHopDong into sfcc
                          from sfc in sfcc.DefaultIfEmpty()
                          join kh in _context.KhachHang
                          on sfc.MaKh equals kh.MaKh into tkh
                          from sfkh in tkh.DefaultIfEmpty()
                          where sfp.PriceId == id
                          select new { sf, sfp, sfc, sfkh };

            var quey = getById.ToQueryString();

            try
            {
                var getSFP = await getById.Select(x => new GetSubFeePriceRequest()
                {
                    PriceId = x.sfp.PriceId,
                    CustomerType = x.sfp.CusType,
                    CustomerId = string.IsNullOrEmpty(x.sfkh.MaKh) ? "" : x.sfkh.MaKh,
                    ContractId = string.IsNullOrEmpty(x.sfp.ContractId) ? "" : x.sfp.ContractId,
                    SfId = x.sfp.SfId,
                    GoodsType = x.sfp.GoodsType,
                    FirstPlace = x.sfp.FirstPlace,
                    SecondPlace = x.sfp.SecondPlace,
                    UnitPrice = x.sfp.UnitPrice,
                    Description = x.sfp.Description,
                    Approver = x.sfp.Approver,
                    Creator = x.sfp.Creator,
                    ApprovedDate = x.sfp.ApprovedDate,
                    DeactiveDate = x.sfp.DeactiveDate
                }).FirstOrDefaultAsync();

                return getSFP;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ListSubFee>> GetListSubFeeSelect()
        {
            try
            {
                var getList = from sf in _context.SubFee
                              join
                              sft in _context.SubFeeType
                              on sf.SfType equals sft.SfTypeId
                              orderby sf.SubFeeId
                              select new { sf, sft };

                var list = await getList.Select(x => new ListSubFee()
                {
                    SubFeeId = x.sf.SubFeeId,
                    SubFeeName = x.sf.SfName,
                    SubFeeDescription = x.sf.SfDescription,
                    SubFeeTypeName = x.sft.SfTypeName
                }).ToListAsync();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<SubFeePrice>> GetListSubFeePriceActive(string customerId, string goodTypes, int? getEmptyPlace, string road)
        {
            var getRoad = await _context.CungDuong.Where(x => x.MaCungDuong == road).FirstOrDefaultAsync();
            var getCus = await _context.KhachHang.Where(x => x.MaKh == customerId).FirstOrDefaultAsync();

            var getSFByCusId = from contract in _context.HopDongVaPhuLuc
                             join sfp in _context.SubFeePrice
                             on contract.MaHopDong equals sfp.ContractId
                             where contract.MaKh == customerId && sfp.Status == 14
                             && ((sfp.GoodsType == goodTypes)
                             || (sfp.FirstPlace == getEmptyPlace && sfp.SecondPlace == null)
                             || (sfp.FirstPlace == getRoad.DiemDau && sfp.SecondPlace == getRoad.DiemCuoi)
                             || (sfp.GoodsType == null && sfp.FirstPlace == null && sfp.SecondPlace == null))
                             select new { contract, sfp };

            var getSFByCusType = await _context.SubFeePrice.Where(x => x.Status == 14 && x.CusType == getCus.MaLoaiKh
            && x.ContractId == null && ((x.GoodsType == goodTypes)
            || (x.FirstPlace == getEmptyPlace && x.SecondPlace == null)
            || (x.FirstPlace == getRoad.DiemDau && x.SecondPlace == getRoad.DiemCuoi)
            || (x.GoodsType == null && x.FirstPlace == null && x.SecondPlace == null))).ToListAsync();

            var dataGetByCusId = await getSFByCusId.Select(x => x.sfp).ToListAsync();

            var listItemRemove = new List<SubFeePrice>();

            foreach (var item in getSFByCusType)
            {
                if (dataGetByCusId.Where(x =>
                x.CusType == item.CusType
                && x.SfId == item.SfId
                && (x.GoodsType == item.GoodsType)
                && ((x.FirstPlace == item.FirstPlace && x.SecondPlace == null)
                || (x.FirstPlace == item.FirstPlace && x.SecondPlace == item.SecondPlace))).Count() > 0)
                {
                    listItemRemove.Add(item);
                }
            }

            if (listItemRemove.Count > 0)
            {
                foreach (var item in listItemRemove)
                {
                    getSFByCusType.Remove(item);
                }
            }

            var data = dataGetByCusId.Concat(getSFByCusType);
            return data.ToList();
        }

        public async Task<PagedResponseCustom<ListSubFeePriceRequest>> GetListSubFeePrice(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var getList = from sfp in _context.SubFeePrice
                              join sf in _context.SubFee
                              on sfp.SfId equals sf.SubFeeId
                              join sft in _context.SubFeeType
                              on sf.SfType equals sft.SfTypeId
                              join hd in _context.HopDongVaPhuLuc
                              on sfp.ContractId equals hd.MaHopDong into sfcc
                              from sfc in sfcc.DefaultIfEmpty()
                              join status in _context.StatusText
                              on sfp.Status equals status.StatusId
                              where status.LangId == tempData.LangID
                              orderby sfp.SfId descending
                              select new { sfp, sf, sft, sfc, status };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getList = getList.Where(x => x.sfc.MaHopDong.Contains(filter.Keyword) || x.sfc.MaKh.Contains(filter.Keyword));
                }

                if (filter.fromDate.HasValue && filter.toDate.HasValue)
                {
                    getList = getList.Where(x => x.sfp.ApprovedDate.Value.Date >= filter.fromDate.Value.Date && x.sfp.ApprovedDate.Value.Date <= filter.toDate.Value.Date);
                }

                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    getList = getList.Where(x => x.sfp.Status == int.Parse(filter.statusId));
                }
                else
                {
                    getList = getList.Where(x => x.sfp.Status != 16 && x.sfp.Status != 15);
                }

                var totalCount = await getList.CountAsync();

                var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListSubFeePriceRequest()
                {
                    PriceId = x.sfp.PriceId,
                    CustomerName = x.sfc.MaKh,
                    ContractId = x.sfc.MaHopDong,
                    ContractName = x.sfc.TenHienThi,
                    GoodsType = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.sfp.GoodsType).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
                    FirstPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.FirstPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    SecondPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.SecondPlace).Select(x => x.TenDiaDiem).FirstOrDefault(),
                    sfName = x.sf.SfName,
                    Status = x.status.StatusContent,
                    UnitPrice = x.sfp.UnitPrice,
                    SfStateByContract = x.sfp.SfStateByContract,
                    Approver = x.sfp.Approver,
                    ApprovedDate = x.sfp.ApprovedDate,
                    DeactiveDate = x.sfp.DeactiveDate
                }).ToListAsync();

                return new PagedResponseCustom<ListSubFeePriceRequest>()
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