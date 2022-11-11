﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.SubFeePriceManage
{
    public class SubFeePriceService : ISubFeePrice
    {
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public SubFeePriceService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }

        public async Task<BoolActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request)
        {
            try
            {
                var checkExists = await _context.SubFeePrice.Where(x => x.SfId == request.SfId && x.ContractId == request.ContractId && x.GoodsType == request.GoodsType
                   && x.FirstPlace == request.FirstPlace && x.SecondPlace == request.SecondPlace && x.UnitPrice == request.UnitPrice).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phụ phí đã tồn tại!" };
                }
                if (request.UnitPrice < 0)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đơn giá phải lớn hơn 0" };
                }
                await _context.AddAsync(new SubFeePrice()
                {
                    ContractId = request.ContractId,
                    SfId = request.SfId,
                    GoodsType = request.GoodsType,
                    FirstPlace = request.FirstPlace,
                    SecondPlace = request.SecondPlace,
                    UnitPrice = request.UnitPrice,
                    SfStateByContract = 2,
                    Status = 13,
                    Description = request.Description,
                    Creator = "admin",
                    Approver = "",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    var newid = _context.SubFeePrice.Where(x => x.SfId == request.SfId && x.ContractId == request.ContractId && x.GoodsType == request.GoodsType
                   && x.FirstPlace == request.FirstPlace && x.SecondPlace == request.SecondPlace && x.SfStateByContract == 1).Select(x => x.PriceId).FirstOrDefault();
                    await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " create new SubFeePrice with id: " + newid.ToString());
                    return new BoolActionResult { isSuccess = true, Message = "Tạo mới phụ phí thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tạo mới phụ phí thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " create new SubFeePrice with ERROR: " + ex.ToString());
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

                getSubFeePrice.ContractId = request.ContractId;
                getSubFeePrice.SfId = request.SfId;
                getSubFeePrice.GoodsType = request.GoodsType;
                getSubFeePrice.FirstPlace = request.FirstPlace;
                getSubFeePrice.SecondPlace = request.SecondPlace;
                getSubFeePrice.UnitPrice = request.UnitPrice;
                getSubFeePrice.Description = request.Description;
                getSubFeePrice.UpdatedDate = DateTime.Now;

                _context.Update(getSubFeePrice);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " edit SubFeePrice with id: " + id);
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật phụ phí thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật phụ phí thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " edit SubFeePrice with ERROR: " + ex.ToString());
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
                        _context.Update(getById);
                    }

                    if (item.Selection == 0)
                    {
                        var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == getById.ContractId).FirstOrDefaultAsync();
                        var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == getContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();

                        if (getNewestContract.MaHopDong != getById.ContractId)
                        {
                            Errors += "Mã  phụ phí: " + item.SubFeePriceId + ", không sử dụng Hợp Đồng/Phụ Lục mới nhất, Vui lòng chọn lại Hợp Đồng/Phụ Lục mới nhất \r\n";
                            continue;
                        }

                        var checkExists = await _context.SubFeePrice.Where(x => x.ContractId == getById.ContractId && x.SfId == getById.SfId
                         && x.GoodsType == getById.GoodsType && x.FirstPlace == getById.FirstPlace && x.SecondPlace == getById.SecondPlace
                         && x.Status == 14).FirstOrDefaultAsync();

                        if (checkExists != null)
                        {
                            checkExists.Status = 12;
                            checkExists.DeactiveDate = DateTime.Now;
                            _context.Update(checkExists);
                        }

                        getById.ApprovedDate = DateTime.Now;
                        getById.Approver = "haile";
                        getById.Status = 14;
                        _context.Update(getById);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = Errors == "" ? "Duyệt phụ phí thành công!" : Errors };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Duyệt phụ phí thất bại!," + Errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
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

                    checkExists.Status = 12;
                    _context.Update(checkExists);
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = errors == "" ? "Vô hiệu phụ phí thành công!" : errors };
                }
                else
                {

                    return new BoolActionResult { isSuccess = false, Message = "Vô hiệu hóa phụ phí thất bại" + errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " disable SubFeePrice with ERROR: " + ex.ToString());
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

                    checkExists.Status = 16;
                    _context.Update(checkExists);
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = errors == "" ? "Xóa phụ phí thành công!" : errors };
                }
                else
                {

                    return new BoolActionResult { isSuccess = false, Message = "Xóa hóa phụ phí thất bại" + errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + "delete SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<GetSubFeePriceRequest> GetSubFeePriceById(long id)
        {
            var getById = from sf in _context.SubFee
                          join sfp in _context.SubFeePrice
                          on sf.SubFeeId equals sfp.SfId
                          join hd in _context.HopDongVaPhuLuc
                          on sfp.ContractId equals hd.MaHopDong
                          join kh in _context.KhachHang
                          on hd.MaKh equals kh.MaKh
                          where sfp.PriceId == id
                          select new { sf, sfp, hd, kh };


            try
            {
                var getSFP = await getById.Select(x => new GetSubFeePriceRequest()
                {
                    PriceId = x.sfp.PriceId,
                    CustomerType = x.kh.MaLoaiKh,
                    CustomerId = x.kh.MaKh,
                    ContractId = x.sfp.ContractId,
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
                              on sfp.ContractId equals hd.MaHopDong
                              join status in _context.StatusText
                              on sfp.Status equals status.StatusId
                              where status.LangId == TempData.LangID
                              orderby sfp.SfId descending
                              select new { sfp, sf, sft, hd, status };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getList = getList.Where(x => x.hd.MaHopDong.Contains(filter.Keyword) || x.hd.MaKh.Contains(filter.Keyword));
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
                    CustomerName = x.hd.MaKh,
                    ContractId = x.hd.MaHopDong,
                    ContractName = x.hd.TenHienThi,
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

        public async Task<PagedResponseCustom<ListSubFeeIncurred>> GetListSubFeeIncurredApprove(PaginationFilter filter)
        {
            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

            var getList = from dp in _context.DieuPhoi
                          join sfc in _context.SfeeByTcommand
                          on dp.Id equals sfc.IdTcommand
                          join sf in _context.SubFee
                          on sfc.SfId equals sf.SubFeeId
                          join status in _context.StatusText
                          on sfc.ApproveStatus equals status.StatusId
                          where sfc.ApproveStatus == 13
                          && status.LangId == TempData.LangID
                          orderby sfc.CreatedDate descending
                          select new { dp, sfc, sf, status };

            if (!string.IsNullOrEmpty(filter.Keyword))
            {
                getList = getList.Where(x => x.dp.MaVanDon.Contains(filter.Keyword) || x.dp.MaSoXe.Contains(filter.Keyword));
            }

            if (filter.fromDate.HasValue && filter.toDate.HasValue)
            {
                getList = getList.Where(x => x.sfc.CreatedDate.Date >= filter.fromDate.Value.Date && x.sfc.CreatedDate.Date <= filter.toDate.Value.Date);
            }

            var totalCount = await getList.CountAsync();

            var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListSubFeeIncurred()
            {
                Id = x.sfc.Id,
                MaVanDon = x.dp.MaVanDon,
                MaSoXe = x.dp.MaSoXe,
                TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                SubFee = x.sf.SfName,
                Price = x.sfc.FinalPrice,
                TrangThai = x.status.StatusContent,
                CreatedDate = x.sfc.CreatedDate
            }).ToListAsync();

            return new PagedResponseCustom<ListSubFeeIncurred>()
            {
                dataResponse = pagedData,
                totalCount = totalCount,
                paginationFilter = validFilter
            };
        }

        public async Task<List<ListSubFeeIncurred>> GetListSubFeeIncurredByHandling(int id)
        {
            var getList = from dp in _context.DieuPhoi
                          join sfc in _context.SfeeByTcommand
                          on dp.Id equals sfc.IdTcommand
                          join sf in _context.SubFee
                          on sfc.SfId equals sf.SubFeeId
                          join status in _context.StatusText
                          on sfc.ApproveStatus equals status.StatusId
                          where sfc.ApproveStatus == 14 && sfc.IdTcommand == id
                          && status.LangId == TempData.LangID
                          orderby sfc.CreatedDate descending
                          select new { dp, sfc, sf, status };

            var data = await getList.Select(x => new ListSubFeeIncurred()
            {
                Id = x.sfc.Id,
                MaVanDon = x.dp.MaVanDon,
                MaSoXe = x.dp.MaSoXe,
                TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                SubFee = x.sf.SfName,
                Price = x.sfc.FinalPrice,
                TrangThai = x.status.StatusContent,
                ApprovedDate = x.sfc.ApprovedDate.Value,
                CreatedDate = x.sfc.CreatedDate,
            }).ToListAsync();

            return data;
        }

        public async Task<GetSubFeeIncurred> GetSubFeeIncurredById(int id)
        {
            var data = from dp in _context.DieuPhoi
                       join sfc in _context.SfeeByTcommand
                       on dp.Id equals sfc.IdTcommand
                       join sf in _context.SubFee
                       on sfc.SfId equals sf.SubFeeId
                       where sfc.Id == id
                       orderby sfc.CreatedDate descending
                       select new { dp, sfc, sf };

            var result = await data.FirstOrDefaultAsync();

            return new GetSubFeeIncurred
            {
                MaVanDon = result.dp.MaVanDon,
                TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == result.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                MaSoXe = result.dp.MaSoXe,
                Romooc = result.dp.MaRomooc,
                LoaiPhuongTien = result.dp.MaLoaiPhuongTien,
                PhuPhi = result.sf.SfName,
                FinalPrice = result.sfc.FinalPrice,
                Note = result.sfc.Note,
                CreatedDate = result.sfc.CreatedDate,
            };
        }

        public async Task<BoolActionResult> ApproveSubFeeIncurred(List<ApproveSubFee> request)
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
                    var getById = await _context.SfeeByTcommand.Where(x => x.Id == item.SubFeePriceId).FirstOrDefaultAsync();

                    if (getById == null)
                    {
                        Errors += "Mã  phụ phí: " + item.SubFeePriceId + ", không tồn tại trong hệ thống";
                        continue;
                    }

                    if (item.Selection == 1)
                    {
                        getById.ApproveStatus = 15;
                        _context.Update(getById);
                    }

                    if (item.Selection == 0)
                    {
                        //var checkExists = await _context.SfeeByTcommand.Where(x => x.IdTcommand == getById.IdTcommand
                        //&& x.SfId == getById.SfId
                        //&& x.SfPriceId == getById.SfPriceId).FirstOrDefaultAsync();

                        //if (checkExists != null)
                        //{
                        //    checkExists.ApproveStatus = 12;
                        //    checkExists.ApprovedDate = DateTime.Now;
                        //    _context.Update(checkExists);
                        //}

                        getById.ApprovedDate = DateTime.Now;
                        getById.ApproveStatus = 14;
                        _context.Update(getById);
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = Errors == "" ? "Duyệt phụ phí phát sinh thành công!" : Errors };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Duyệt phụ phí phát sinh thất bại!," + Errors };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
    }
}