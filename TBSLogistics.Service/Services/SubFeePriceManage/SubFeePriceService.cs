using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.SubFeePriceManage
{
    public class SubFeePriceService: ISubFeePrice
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
                   && x.FirstPlace == request.FirstPlace && x.SecondPlace == request.SecondPlace && x.SfStateByContract == 1).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Phụ phí đã tồn tại!" };
                }
                if (request.SfId.ToString() == "")
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mã phụ phí không tồn tại!" };
                }    
                if (request.UnitPrice <= 0 )
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
                    SfStateByContract = request.SfStateByContract,
                    Description = request.Description,
                    Creator = "admin",
                    Approver = ""
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

                var checkSFP = await _context.SubFeePrice.Where(x => x.ContractId == request.ContractId && x.SfId == request.SfId && 
                x.GoodsType == request.GoodsType && x.FirstPlace == request.FirstPlace && x.SecondPlace == request.SecondPlace).FirstOrDefaultAsync();
                if (checkSFP != null && checkSFP.PriceId != id && checkSFP.SfStateByContract == 1)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đã tồn tại thông tin phụ phí cập nhật" };
                }

                if (getSubFeePrice.SfStateByContract == 2)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Không được phép thay đổi thông tin đã phê duyệt" };
                }

                getSubFeePrice.ContractId = request.ContractId;
                getSubFeePrice.SfId = request.SfId;
                getSubFeePrice.GoodsType = request.GoodsType;
                getSubFeePrice.FirstPlace = request.FirstPlace;
                getSubFeePrice.SecondPlace = request.SecondPlace;
                getSubFeePrice.UnitPrice = request.UnitPrice;
                getSubFeePrice.Description = request.Description;

                _context.Update(getSubFeePrice);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " edit SubFeePrice with id: " + request.PriceId);
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

        public async Task<BoolActionResult> ApproveSubFeePrice(long[] ids, string HDPL)
        {
            try
            {
                if ((ids.Length == 0 && HDPL == null) || (ids.Length > 0 && HDPL != null))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }
                
                List<long> ListIdError = new List<long>(); //danh sach cac ID khong duoc cap nhat hoac khong ton tai

                if (ids.Length > 0)
                {
                    foreach (long id in ids)
                    {
                        var GetSFPById = await _context.SubFeePrice.Where(x => x.PriceId == id).FirstOrDefaultAsync();
                        if (GetSFPById == null || GetSFPById.SfStateByContract == 0 || GetSFPById.SfStateByContract == 2 || GetSFPById.SfStateByContract == 3)
                        {
                            ListIdError.Add(id);
                        }
                        else
                        {
                            GetSFPById.SfStateByContract = 2;
                            GetSFPById.Approver = "admin";
                            GetSFPById.ApprovedDate = DateTime.Now;
                            _context.Update(GetSFPById);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (ListIdError.Count > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Danh sách các ID không được phê duyệt hoặc không tồn tại: " + PrintListIDs(ListIdError) };
                    }
                    else
                    {
                        await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " approve SubFeePrice with ids: " + ids);
                        return new BoolActionResult { isSuccess = true, Message = "Phê duyệt thành công" };
                    }    
                }
                else
                {
                    //Lay cac phu phi chua phe duyet theo hop dong
                    var SFPbyHDPL = await _context.SubFeePrice.Where(x => x.ContractId == HDPL && x.SfStateByContract == 1).ToListAsync();

                    if (SFPbyHDPL.Count == 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không có phụ phí nào cần phê duyệt" };
                    }

                    foreach (SubFeePrice sfp in SFPbyHDPL)
                    {
                        var checkExists = await _context.SubFeePrice.Where(x => x.SfId == sfp.SfId && x.ContractId == sfp.ContractId && x.GoodsType == sfp.GoodsType
                       && x.FirstPlace == sfp.FirstPlace && x.SecondPlace == sfp.SecondPlace && x.SfStateByContract == 2).FirstOrDefaultAsync();

                        if (checkExists != null)
                        {
                            ListIdError.Append(checkExists.PriceId);
                            checkExists.SfStateByContract = 0;
                            checkExists.DeactiveDate = DateTime.Now;
                            _context.Update(checkExists);
                            await _context.SaveChangesAsync();
                        }

                        sfp.SfStateByContract = 2;
                        sfp.Approver = "admin";
                        sfp.ApprovedDate = DateTime.Now;
                        _context.Update(sfp);
                        await _context.SaveChangesAsync();
                    }

                    //if (ListIdError.Length == 0)
                    //{
                    //    return new BoolActionResult { isSuccess = true, Message = "Cập nhật tất cả phụ phí của hợp đồng " + HDPL + " thành công" };
                    //}
                    //else
                    //{
                    //    return new BoolActionResult { isSuccess = true, Message = "Cập nhật " };
                    //}
                    return new BoolActionResult { isSuccess = true, Message = "Phê duyệt tất cả phụ phí của hợp đồng " + HDPL + " thành công" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        private string PrintListIDs(List<long> ids)
        {
            string StringResult = "";
            for (int i = 0; i < ids.Count; i++)
            {
                if (i == ids.Count - 1)
                {
                    StringResult = StringResult + ids[i].ToString();
                }    
                else StringResult = StringResult + ids[i].ToString() + ", ";
            }    
            return StringResult;
        }

        public async Task<BoolActionResult> DisableSubFeePrice(long[] ids, string HDPL)
        {
            try
            {
                if ((ids.Length == 0 && HDPL == null) || (ids.Length > 0 && HDPL != null))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }
                
                List<long> ListIdError = new List<long>(); //danh sach cac ID khong duoc cap nhat hoac khong ton tai

                if (ids.Length > 0)
                {
                    foreach (long id in ids)
                    {
                        var GetSFPById = await _context.SubFeePrice.Where(x => x.PriceId == id).FirstOrDefaultAsync();
                        if (GetSFPById == null || GetSFPById.SfStateByContract == 0 || GetSFPById.SfStateByContract == 3)
                        {
                            ListIdError.Add(id);
                        }
                        else
                        {
                            GetSFPById.SfStateByContract = 0;
                            GetSFPById.DeactiveDate = DateTime.Now;
                            _context.Update(GetSFPById);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (ListIdError.Count > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Danh sách các ID không được cập nhật hoặc không tồn tại: " + PrintListIDs(ListIdError) };
                    }
                    else
                    {
                        await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + "disable SubFeePrice with ids: " + ids);
                        return new BoolActionResult { isSuccess = true, Message = "Vô hiệu hóa phụ phí thành công" };
                    }
                }
                else
                {
                    //Lay cac phu phi co the vo hieu hoa theo hop dong
                    var SFPbyHDPL = await _context.SubFeePrice.Where(x => x.ContractId == HDPL && (x.SfStateByContract == 1 || x.SfStateByContract == 2)).ToListAsync();

                    if (SFPbyHDPL.Count == 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không có phụ phí nào có thể vô hiệu hóa" };
                    }

                    foreach (SubFeePrice sfp in SFPbyHDPL)
                    {
                        sfp.SfStateByContract = 0;
                        sfp.DeactiveDate = DateTime.Now;
                        _context.Update(sfp);
                        await _context.SaveChangesAsync();
                    }
                    return new BoolActionResult { isSuccess = true, Message = "Vô hiệu hóa tất cả phụ phí của hợp đồng " + HDPL + " thành công" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " disable SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }

        public async Task<BoolActionResult> DeleteSubFeePrice(long[] ids, string HDPL)
        {
            try
            {
                if ((ids.Length == 0 && HDPL == null) || (ids.Length > 0 && HDPL != null))
                {
                    return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
                }

                List<long> ListIdError = new List<long>(); //danh sach cac ID khong duoc cap nhat hoac khong ton tai

                if (ids.Length > 0)
                {
                    foreach (long id in ids)
                    {
                        var GetSFPById = await _context.SubFeePrice.Where(x => x.PriceId == id).FirstOrDefaultAsync();
                        if (GetSFPById == null || GetSFPById.SfStateByContract != 1)
                        {
                            ListIdError.Add(id);
                        }
                        else
                        {
                            GetSFPById.SfStateByContract = 3;
                            _context.Update(GetSFPById);
                            await _context.SaveChangesAsync();
                        }
                    }
                    if (ListIdError.Count > 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Danh sách các ID không được cập nhật hoặc không tồn tại: " + PrintListIDs(ListIdError) };
                    }
                    else
                    {
                        await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + "delete SubFeePrice with ids: " + ids);
                        return new BoolActionResult { isSuccess = true, Message = "Xóa phụ phí thành công" };
                    }
                }
                else
                {
                    //Lay cac phu phi co the xoa theo hop dong
                    var SFPbyHDPL = await _context.SubFeePrice.Where(x => x.ContractId == HDPL && x.SfStateByContract == 1).ToListAsync();

                    if (SFPbyHDPL.Count == 0)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không có phụ phí nào của hợp đồng " + HDPL + " có thể xóa" };
                    }

                    foreach (SubFeePrice sfp in SFPbyHDPL)
                    {
                        sfp.SfStateByContract = 0;
                        _context.Update(sfp);
                        await _context.SaveChangesAsync();
                    }
                    return new BoolActionResult { isSuccess = true, Message = "Xóa tất cả phụ phí của hợp đồng " + HDPL + " thành công" };
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
            try
            {
                var getSFP = await _context.SubFeePrice.Where(x => x.PriceId == id).Select(x => new GetSubFeePriceRequest()
                {
                    PriceId = x.PriceId,
                    ContractId = x.ContractId,
                    SfId = x.SfId,
                    GoodsType = x.GoodsType,
                    FirstPlace = x.FirstPlace,
                    SecondPlace = x.SecondPlace,
                    UnitPrice = x.UnitPrice,
                    SfStateByContract = x.SfStateByContract,
                    Description = x.Description,
                    Approver = x.Approver,
                    Creator = x.Creator,
                    ApprovedDate = x.ApprovedDate,
                    DeactiveDate = x.DeactiveDate
                }).FirstOrDefaultAsync();

                return getSFP;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<GetSubFeePriceRequest>> GetListSubFeePrice(string KH, string HDPL, DateTime ngayapdung, byte trangthai)
        {
            try
            {
                //if (id.tostring() != "")
                //{
                //var list = await _context.SubFeePrice.Where(x => x.SfStateByContract == 0).Select(x => new GetSubFeePriceRequest()
                //{
                //    PriceId = x.PriceId,
                //    ContractId = x.ContractId,
                //    SfId = x.SfId,
                //    GoodsType = x.GoodsType,
                //    FirstPlace = x.FirstPlace,
                //    SecondPlace = x.SecondPlace,
                //    UnitPrice = x.UnitPrice,
                //    SfStateByContract = x.SfStateByContract,
                //    Description = x.Description,
                //    Approver = x.Approver,
                //    Creator = x.Creator,
                //    ApprovedDate = x.ApprovedDate,
                //    DeactiveDate = x.DeactiveDate
                //}).ToListAsync();
                //return list;
                //}
                if (KH != null)
                {
                    var List_SFP_HDPL = from sfp in _context.SubFeePrice
                                        join hdpl in _context.HopDongVaPhuLuc on sfp.ContractId equals hdpl.MaHopDong
                                        select new
                                        {
                                            sfp,
                                            hdpl
                                        };

                    var list = await List_SFP_HDPL.Where(x => x.sfp.SfStateByContract == 2 && x.hdpl.MaKh == KH).Select(x => new GetSubFeePriceRequest()
                    {
                        PriceId = x.sfp.PriceId,
                        ContractId = x.sfp.ContractId,
                        SfId = x.sfp.SfId,
                        GoodsType = x.sfp.GoodsType,
                        FirstPlace = x.sfp.FirstPlace,
                        SecondPlace = x.sfp.SecondPlace,
                        UnitPrice = x.sfp.UnitPrice,
                        SfStateByContract = x.sfp.SfStateByContract,
                        Description = x.sfp.Description,
                        Approver = x.sfp.Approver,
                        Creator = x.sfp.Creator,
                        ApprovedDate = x.sfp.ApprovedDate,
                        DeactiveDate = x.sfp.DeactiveDate
                    }).ToListAsync();

                    if (ngayapdung != DateTime.MinValue && trangthai != 0)
                    {
                        var result = list.Where(x => x.ApprovedDate < ngayapdung && x.SfStateByContract == trangthai).ToList();
                        return result;
                    }
                    else if (ngayapdung != DateTime.MinValue)
                    {
                        var result = list.Where(x => x.ApprovedDate < ngayapdung).ToList();
                        return result;
                    }
                    else if (trangthai != 0)
                    {
                        var result = list.Where(x => x.SfStateByContract == trangthai).ToList();
                        return result;
                    }
                    return list;
                }
                else if (HDPL != null)
                {
                    if (trangthai != 0)
                    {
                        var list = await _context.SubFeePrice.Where(x => x.SfStateByContract == trangthai && x.ContractId == HDPL).Select(x => new GetSubFeePriceRequest()
                        {
                            PriceId = x.PriceId,
                            ContractId = x.ContractId,
                            SfId = x.SfId,
                            GoodsType = x.GoodsType,
                            FirstPlace = x.FirstPlace,
                            SecondPlace = x.SecondPlace,
                            UnitPrice = x.UnitPrice,
                            SfStateByContract = x.SfStateByContract,
                            Description = x.Description,
                            Approver = x.Approver,
                            Creator = x.Creator,
                            ApprovedDate = x.ApprovedDate,
                            DeactiveDate = x.DeactiveDate
                        }).ToListAsync();

                        return list;
                    }
                    else
                    {
                        var list = await _context.SubFeePrice.Where(x => x.ContractId == HDPL && (x.SfStateByContract == 1 || x.SfStateByContract == 2)).Select(x => new GetSubFeePriceRequest()
                        {
                            PriceId = x.PriceId,
                            ContractId = x.ContractId,
                            SfId = x.SfId,
                            GoodsType = x.GoodsType,
                            FirstPlace = x.FirstPlace,
                            SecondPlace = x.SecondPlace,
                            UnitPrice = x.UnitPrice,
                            SfStateByContract = x.SfStateByContract,
                            Description = x.Description,
                            Approver = x.Approver,
                            Creator = x.Creator,
                            ApprovedDate = x.ApprovedDate,
                            DeactiveDate = x.DeactiveDate
                        }).ToListAsync();

                        return list;
                    }
                }
                else if (ngayapdung != DateTime.MinValue)
                {
                    var list = await _context.SubFeePrice.Where(x => x.ApprovedDate <= ngayapdung && (x.SfStateByContract == 1 || x.SfStateByContract == 2 || (x.SfStateByContract == 0 && x.DeactiveDate > ngayapdung))).Select(x => new GetSubFeePriceRequest()
                    {
                        PriceId = x.PriceId,
                        ContractId = x.ContractId,
                        SfId = x.SfId,
                        GoodsType = x.GoodsType,
                        FirstPlace = x.FirstPlace,
                        SecondPlace = x.SecondPlace,
                        UnitPrice = x.UnitPrice,
                        SfStateByContract = x.SfStateByContract,
                        Description = x.Description,
                        Approver = x.Approver,
                        Creator = x.Creator,
                        ApprovedDate = x.ApprovedDate,
                        DeactiveDate = x.DeactiveDate
                    }).ToListAsync();
                    return list;
                }
                else
                {
                    if (trangthai == 0)
                    {
                        trangthai = 2;
                    }
                    if (trangthai == 7)
                    {
                        var list = await _context.SubFeePrice.Where(x => x.SfStateByContract == 1 || x.SfStateByContract == 2).Select(x => new GetSubFeePriceRequest()
                        {
                            PriceId = x.PriceId,
                            ContractId = x.ContractId,
                            SfId = x.SfId,
                            GoodsType = x.GoodsType,
                            FirstPlace = x.FirstPlace,
                            SecondPlace = x.SecondPlace,
                            UnitPrice = x.UnitPrice,
                            SfStateByContract = x.SfStateByContract,
                            Description = x.Description,
                            Approver = x.Approver,
                            Creator = x.Creator,
                            ApprovedDate = x.ApprovedDate,
                            DeactiveDate = x.DeactiveDate
                        }).ToListAsync();
                        return list;
                    }
                    else
                    {
                        var list = await _context.SubFeePrice.Where(x => x.SfStateByContract == trangthai).Select(x => new GetSubFeePriceRequest()
                        {
                            PriceId = x.PriceId,
                            ContractId = x.ContractId,
                            SfId = x.SfId,
                            GoodsType = x.GoodsType,
                            FirstPlace = x.FirstPlace,
                            SecondPlace = x.SecondPlace,
                            UnitPrice = x.UnitPrice,
                            SfStateByContract = x.SfStateByContract,
                            Description = x.Description,
                            Approver = x.Approver,
                            Creator = x.Creator,
                            ApprovedDate = x.ApprovedDate,
                            DeactiveDate = x.DeactiveDate
                        }).ToListAsync();

                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
