using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.SFeeByTcommandManage
{
    public class SFeeByTcommandService : ISFeeByTcommand
    {

        private readonly ICommon _common;
        private readonly TMSContext _context;

        public SFeeByTcommandService(ICommon common, TMSContext context)
        {
            _common = common;
            _context = context;
        }
        public async Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request)
        {
            try
            {
                List<string> IdListFail = new List<string>();

                foreach (var i in request)
                {
                    string ErrorValidate = await Validate(i.IdTcommand, i.SfId, i.SfPriceId, i.Price, i.FinalPrice);
                    if (ErrorValidate != "")
                    {
                        IdListFail.Add(" Bản Ghi:" + i.IdTcommand + " -" + i.SfId + " -" + i.SfPriceId + " -" + i.Price + " -" + i.FinalPrice + " </br>" + ErrorValidate + " </br>");
                        continue;
                    }
                    var checkSFeeByTcommand = await _context.SfeeByTcommand.Where(x => x.IdTcommand == i.IdTcommand && x.SfId == i.SfId && x.ApproveStatus == 13 && x.SfPriceId == i.SfPriceId).FirstOrDefaultAsync();
                    if (checkSFeeByTcommand != null)
                    {
                        IdListFail.Add(" Phụ phí " + await _context.SubFee.Where(x => x.SubFeeId == i.SfId).Select(x => x.SfName).FirstOrDefaultAsync() + " đã tồn tại và đang chờ duyệt </br>");
                        continue;
                    }
                    await _context.AddAsync(new SfeeByTcommand()
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
                }

                var result1 = await _context.SaveChangesAsync();
                if (result1 > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Thêm phụ phí phát sinh thành công! </br>" + (IdListFail.Count == 0 ? "" : string.Join(",", IdListFail)) };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thêm phụ phí phát sinh thất bại!  </br>" + (IdListFail.Count == 0 ? "" : string.Join(",", IdListFail)) };
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
            var checkExists = await _context.SfeeByTcommand.Where(x => x.Id == request.Id).FirstOrDefaultAsync();
            if (checkExists == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.Id + "không tồn tại  " };
            }
            if (checkExists.ApproveStatus != 13)
            {
                return new BoolActionResult { isSuccess = false, Message = "ID: " + request.Id + "Phụ phát sinh không phải là trạng thái chờ duyệt, không thể xóa " };
            }
            checkExists.ApproveStatus = 16;
            _context.Update(checkExists);
            var result = await _context.SaveChangesAsync();
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
        private async Task<string> Validate(long IdTcommand, long SfId, long? SfPriceId, double Price, double FinalPrice, string ErrorRow = "")
        {
            string ErrorValidate = "";

            var checkSubFeeId = await _context.SubFee.Where(x => x.SubFeeId == SfId).FirstOrDefaultAsync();
            if (checkSubFeeId == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phụ Phí: " + SfId + " không tồn tại </br>";
            }

            var checkIdTcommand = await _context.DieuPhoi.Where(x => x.Id == IdTcommand).FirstOrDefaultAsync();
            if (checkIdTcommand == null)
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Điều Phối: " + IdTcommand + " không tồn tại </br>";
            }

            if (!Regex.IsMatch(FinalPrice.ToString(), "^\\d*(\\.\\d+)?$"))
            {
                ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Giá: " + FinalPrice + " Phải là số và không được Âm (-) ! </br>";
            }
            return ErrorValidate;
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
            var getHandling = await _context.DieuPhoi.Where(x => x.Id == id).FirstOrDefaultAsync();
            var getTransport = await _context.VanDon.Where(x => x.MaVanDon == getHandling.MaVanDon).FirstOrDefaultAsync();
            var getRoad = await _context.CungDuong.Where(x => x.MaCungDuong == getTransport.MaCungDuong).FirstOrDefaultAsync();
            var getListSubFeeByContract = from kh in _context.KhachHang
                                          join hd in _context.HopDongVaPhuLuc
                                          on kh.MaKh equals hd.MaKh
                                          join sfPice in _context.SubFeePrice
                                          on hd.MaHopDong equals sfPice.ContractId
                                          join sf in _context.SubFee
                                          on sfPice.SfId equals sf.SubFeeId
                                          join tt in _context.StatusText
                                          on sfPice.Status equals tt.StatusId
                                          where sfPice.Status == 14
                                          && tt.LangId == TempData.LangID
                                          && kh.MaKh == getTransport.MaKh
                                          select new { kh, hd, sfPice, sf, tt };

            getListSubFeeByContract = getListSubFeeByContract.Where(y => 
                           (y.sfPice.GoodsType == getHandling.MaLoaiHangHoa)
                        || (y.sfPice.FirstPlace == getHandling.DiemLayTraRong && y.sfPice.SecondPlace == null)
                        || (y.sfPice.FirstPlace == getRoad.DiemDau && y.sfPice.SecondPlace == getRoad.DiemCuoi)
                        || (y.sfPice.GoodsType == null && y.sfPice.FirstPlace == null && y.sfPice.SecondPlace == null));


            var dataSubFeeIncurred = from dp in _context.DieuPhoi
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


            var dataSubFeeByContract = await getListSubFeeByContract.Select(x => new ListSubFeeIncurred()
            {
                Id = x.sfPice.PriceId,
                MaVanDon = getHandling.MaVanDon,
                MaSoXe = null,
                TaiXe = null,
                SubFee = x.sf.SfName,
                Type = "Phụ phí theo hợp đồng",
                Price = x.sfPice.UnitPrice,
                TrangThai = x.tt.StatusContent,
                ApprovedDate = x.sfPice.ApprovedDate,
                CreatedDate = x.sfPice.CreatedDate,

            }).ToListAsync();


            var listDataSubFeeIncurred = await dataSubFeeIncurred.Select(x => new ListSubFeeIncurred()
            {
                Id = x.sfc.Id,
                MaVanDon = x.dp.MaVanDon,
                MaSoXe = x.dp.MaSoXe,
                Type = "Phụ Phí Phát Sinh",
                TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
                SubFee = x.sf.SfName,
                Price = x.sfc.FinalPrice,
                TrangThai = x.status.StatusContent,
                ApprovedDate = x.sfc.ApprovedDate.Value,
                CreatedDate = x.sfc.CreatedDate,
            }).ToListAsync();

            var data = dataSubFeeByContract.Concat(listDataSubFeeIncurred).ToList();

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
        public async Task<BoolActionResult> ApproveSubFeeIncurred(List<ApproveSFeeByTCommand> request)
        {
            var transaction = _context.Database.BeginTransaction();
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
                    var getById = await _context.SfeeByTcommand.Where(x => x.Id == item.ID).FirstOrDefaultAsync();

                    if (getById == null)
                    {
                        Errors += "Mã  phụ phí: " + item.ID + ", không tồn tại trong hệ thống";
                        continue;
                    }

                    if (item.isApprove == 1)
                    {
                        getById.ApproveStatus = 15;
                        _context.Update(getById);
                    }

                    if (item.isApprove == 0)
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
                await transaction.CommitAsync();

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
                await transaction.RollbackAsync();
                await _common.Log("SubFeePriceManage", "UserId: " + TempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
            }
        }
    }
}
