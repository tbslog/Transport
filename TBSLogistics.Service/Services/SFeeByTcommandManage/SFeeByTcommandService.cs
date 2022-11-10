using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.SFeeByTcommandManage
{
    public class SFeeByTcommandService : ISFeeByTcommand
    {

        private readonly ICommon _common;
        private readonly TMSContext _TMSContext;

        public SFeeByTcommandService(ICommon common, TMSContext context)
        {
            _common = common;
            _TMSContext = context;
        }
        public async Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request)
        {
            try
            {
                List<string> IdList = new List<string>();
                List<string> IdListFail = new List<string>();

                foreach (var i in request)
                {
                    string ErrorValidate = await Validate(i.IdTcommand, i.SfId, i.SfPriceId, i.Price, i.FinalPrice);
                    if (ErrorValidate != "")
                    {
                        IdListFail.Add(" Bản Ghi:" + i.IdTcommand + " -" + i.SfId + " -" + i.SfPriceId + " -" + i.Price + " -" + i.FinalPrice + " \r\n" + ErrorValidate + " \r\n");

                        continue;
                    }
                    var checkSFeeByTcommand = await _TMSContext.SfeeByTcommand.Where(x => x.IdTcommand == i.IdTcommand && x.SfId == i.SfId && x.ApproveStatus == 13 && x.SfPriceId == i.SfPriceId).FirstOrDefaultAsync();
                    if (checkSFeeByTcommand != null)
                    {

                        checkSFeeByTcommand.Price = i.Price;
                        checkSFeeByTcommand.FinalPrice = i.FinalPrice;
                        checkSFeeByTcommand.Note = i.Note;

                        _TMSContext.Update(checkSFeeByTcommand);
                        var result = await _TMSContext.SaveChangesAsync();

                        if (result > 0)
                        {
                            await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " Edit SFeeByTcommand with Id: " + checkSFeeByTcommand.Id);
                            IdList.Add(" Cập nhật thành công! " + checkSFeeByTcommand.Id + "-" + i.Price + "-" + i.IdTcommand + "-" + i.SfId + " \r\n");
                            continue;
                        }
                        else
                        {
                            IdListFail.Add(" Tạo mới thất bại ! " + checkSFeeByTcommand.Id + "-" + i.Price + "-" + i.IdTcommand + "-" + i.SfId + "-" + "lỗi SQL" + " \r\n");
                            continue;
                        }

                    }
                    await _TMSContext.AddAsync(new SfeeByTcommand()
                    {
                        IdTcommand = i.IdTcommand,
                        SfId = i.SfId,
                        SfPriceId = i.SfPriceId,
                        Price = i.Price,
                        FinalPrice = i.FinalPrice,
                        ApproveStatus = 13,
                        Note = i.Note,
                        CreatedDate = DateTime.Now,
                        ApprovedDate = null
                    });
                    var result1 = await _TMSContext.SaveChangesAsync();

                    if (result1 > 0)
                    {
                        await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " Edit SFeeByTcommand with Id: ");
                        IdList.Add(" Thêm phụ phí phát sinh thành công! " + i.Price + "-" + i.IdTcommand + "-" + i.SfId + "-" + i.SfPriceId + " \r\n");
                        continue;

                    }
                    else
                    {
                        IdListFail.Add("Thêm phụ phí phát sinh thất bại! " + i.Price + "-" + i.IdTcommand + "-" + i.SfId + "-" + i.SfPriceId + "-" + "lỗi SQL" + " \r\n");
                        continue;
                    }
                }
                if (IdList.Count > 0)
                {
                    string a = string.Join(",", IdList);
                    string b = string.Join(",", IdListFail);
                    await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " create new SFeeByTcommand : ");
                    return new BoolActionResult { isSuccess = true, Message = a + " \r\n" + b + " \r\n" };
                }
                else
                {
                    string b = string.Join(",", IdListFail);
                    return new BoolActionResult { isSuccess = false, Message = b };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " create new SFeeByTcommand has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<BoolActionResult> DeleteSFeeByTCommand(DeleteSFeeByTCommand request)
        {
            var checkExists = await _TMSContext.SfeeByTcommand.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (checkExists == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.Id + "không tồn tại  " };
            }
            if (checkExists.ApproveStatus != 13)
            {
                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.Id + "Phụ phát sinh không phải là trạng thái chờ duyệt, không thể xóa " };
            }
            checkExists.ApproveStatus = 16;
            _TMSContext.Update(checkExists);
            var result = await _TMSContext.SaveChangesAsync();
            if (result > 0)
            {
                await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " Delete  SFeeByTcommand with Id: " + request.Id);
                return new BoolActionResult { isSuccess = true, Message = "Phụ phát sinh thành công!" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Phụ phát sinh thất bại!" };
            }
        }
        public async Task<BoolActionResult> ApproveSFeeByTCommand(List<ApproveSFeeByTCommand> request)
        {
            List<string> IdList = new List<string>();
            List<string> IdListFail = new List<string>();
            try
            {
                foreach (var item in request)
                {

                    var checkExists = await _TMSContext.SfeeByTcommand.Where(x => x.Id == item.ID).FirstOrDefaultAsync();
                    if (checkExists == null)
                    {
                        IdListFail.Add("Phụ phí phát sinh ID:" + item + " này không tồn tại " + " \r\n");
                        continue;

                    }
                    var checkTT = await _TMSContext.SfeeByTcommand.Where(x => x.Id == item.ID && x.ApproveStatus == 13).FirstOrDefaultAsync();
                    if (checkTT == null)
                    {
                        IdListFail.Add("Phụ phí phát sinh ID:" + item.ID + " phải ở trạng thái tạo mới  " + " \r\n");
                        continue;

                    }
                    if (item.isApprove == 0)
                    {
                        checkTT.ApproveStatus = 14;
                        checkTT.ApprovedDate= DateTime.Now;
                        _TMSContext.Update(checkTT);
                        var result = await _TMSContext.SaveChangesAsync();
                        if (result > 0)
                        {
                            IdList.Add("Approve thành công Phụ phí phát sinh ID :" + item.ID + " \r\n");
                            continue;
                        }
                        else
                        {
                            IdListFail.Add("Approve thất bại Phụ phí phát sinh ID" + item.ID + " \r\n");
                            continue;

                        }
                    }
                    if (item.isApprove == 1)
                    {
                        checkTT.ApproveStatus = 15;
                        _TMSContext.Update(checkTT);
                        var result = await _TMSContext.SaveChangesAsync();
                        if (result > 0)
                        {
                            IdList.Add("Không Duyệt thành công Phụ phí phát sinh ID :" + item.ID + " \r\n");
                            continue;
                        }
                        else
                        {
                            IdListFail.Add("Không Duyệt thất bại Phụ phí phát sinh ID" + item.ID + " \r\n");
                            continue;

                        }
                    }
                }
                if (IdList.Count > 0)
                {
                    string a = string.Join(",", IdList);
                    string b = string.Join(",", IdListFail);
                    await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " Approve SFeeByTcommand Id: ");
                    return new BoolActionResult { isSuccess = true, Message = a + " \r\n" + b + " \r\n" };
                }
                else
                {
                    string a = string.Join(",", IdListFail);
                    return new BoolActionResult { isSuccess = false, Message = a + " \r\n" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("SFeeByTcommandManage", "UserId: " + TempData.UserID + " Approve SFeeByTcommand has ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
        public async Task<List<GetListSubFeeByHandling>> GetListSubFeeByHandling(long IdTcommand1)
        {
            try
            {
                var result = await _TMSContext.SfeeByTcommand.Where(x => x.IdTcommand == IdTcommand1).ToArrayAsync();
                var list = result.Select(x => new GetListSubFeeByHandling()
               
                {
                    Id = x.Id,
                    IdTcommand = x.IdTcommand,
                    SfId = x.SfId,
                    SfPriceId = x.SfPriceId,
                    Price = x.Price,
                    FinalPrice = x.FinalPrice,
                    ApproveStatus =  _TMSContext.StatusText.Where(y => y.StatusId == x.ApproveStatus).Select(x=>x.StatusContent).FirstOrDefault(),
                    Note = x.Note,
                    CreatedDate = x.CreatedDate,
                    ApprovedDate = x.ApprovedDate
                });
                return list.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        private async Task<string> Validate(long IdTcommand, long SfId, long? SfPriceId, double Price, double FinalPrice, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkSubFeeId = await _TMSContext.SubFee.Where(x => x.SubFeeId == SfId).FirstOrDefaultAsync();
            if (checkSubFeeId == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phụ Phí: " + SfId + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checkIdTcommand =await _TMSContext.DieuPhoi.Where(x => x.Id == IdTcommand).FirstOrDefaultAsync();
            if (checkIdTcommand == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Điều Phối: " + IdTcommand + " không tồn tại \r\n" + System.Environment.NewLine;
            }
            var checktt = await _TMSContext.SfeeByTcommand.Where(x=>x.IdTcommand ==IdTcommand && x.SfId == SfId && x.SfPriceId==SfPriceId && x.ApproveStatus==14).FirstOrDefaultAsync();

            if (checktt != null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " Bản Ghi:" + IdTcommand + " -" + SfId + " -" + SfPriceId + " đã tồn tại và được duyệt ! \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(Price.ToString(), "^\\d*(\\.\\d+)?$"))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Giá: " + Price + " Phải là số và không được Âm (-) ! \r\n" + System.Environment.NewLine;
            }
            if (!Regex.IsMatch(FinalPrice.ToString(), "^\\d*(\\.\\d+)?$"))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Giá: " + FinalPrice + " Phải là số và không được Âm (-) ! \r\n" + System.Environment.NewLine;
            }
            return ErrorValidate;
        }
    }
}
