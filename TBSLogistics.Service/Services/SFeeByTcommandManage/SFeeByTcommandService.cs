using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.SFeeByTcommandManage
{
	public class SFeeByTcommandService : ISFeeByTcommand
	{
		private readonly ICommon _common;
		private readonly TMSContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public SFeeByTcommandService(ICommon common, TMSContext context, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_common = common;
			_context = context;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request)
		{
			try
			{
				List<string> IdListFail = new List<string>();

				var listStatus = new List<int>(new int[] { 20, 21, 19, 31, 38 });

				foreach (var i in request)
				{
					string ErrorValidate = await Validate(i.IdTcommand, i.SfId, i.FinalPrice);
					if (ErrorValidate != "")
					{
						IdListFail.Add(" Bản Ghi:" + i.IdTcommand + " - " + i.SfId + " - " + i.FinalPrice + ErrorValidate);
						continue;
					}

					var checkSFeeByTcommand = await _context.SfeeByTcommand.Where(x => x.IdTcommand == i.IdTcommand && x.PlaceId == i.PlaceId && x.SfId == i.SfId && x.ApproveStatus == 13).FirstOrDefaultAsync();
					if (checkSFeeByTcommand != null)
					{
						IdListFail.Add(" Phụ phí " + await _context.SubFee.Where(x => x.SubFeeId == i.SfId).Select(x => x.SfName).FirstOrDefaultAsync() + " đã tồn tại và đang chờ duyệt");
						continue;
					}

					if (i.PlaceId != null)
					{
						var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == i.PlaceId).FirstOrDefaultAsync();
						if (checkPlace == null)
						{
							IdListFail.Add(" Địa Điểm " + i.PlaceId + " không tồn tại trong hệ thống");
							continue;
						}
					}

					var checkHandling = await _context.DieuPhoi.Where(x => x.Id == i.IdTcommand && listStatus.Contains(x.TrangThai)).FirstOrDefaultAsync();
					if (checkHandling != null)
					{
						IdListFail.Add(" Chuyến " + i.IdTcommand + " không thể thêm phụ phí nữa");
						continue;
					}

					await _context.AddAsync(new SfeeByTcommand()
					{
						PlaceId = i.PlaceId,
						PayForId = i.sftPayfor,
						IdTcommand = i.IdTcommand,
						SfId = i.SfId,
						Price = i.FinalPrice,
						ApproveStatus = 13,
						Note = i.Note,
						CreatedDate = DateTime.Now,
						Creator = tempData.UserName,
					});
				}

				var result1 = await _context.SaveChangesAsync();
				if (result1 > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("SFeeByTcommandManage", "UserId: " + tempData.UserName + " create new SFeeByTcommand with Data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Thêm phụ phí phát sinh thành công! " + (IdListFail.Count == 0 ? "" : string.Join(",", IdListFail)) };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Thêm phụ phí phát sinh thất bại!  " + (IdListFail.Count == 0 ? "" : string.Join(",", IdListFail)) };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("SFeeByTcommandManage", "UserId: " + tempData.UserID + " create new SFeeByTcommand has ERROR: " + ex.ToString());
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
				await _common.LogTimeUsedOfUser(tempData.Token);
				await _common.Log("SFeeByTcommandManage", "UserId: " + tempData.UserID + " Delete  SFeeByTcommand with Id: " + request.Id);
				return new BoolActionResult { isSuccess = true, Message = "Phụ phát sinh thành công!" };
			}
			else
			{
				return new BoolActionResult { isSuccess = false, Message = "Phụ phát sinh thất bại!" };
			}
		}

		private async Task<string> Validate(long IdTcommand, long SfId, double FinalPrice, string ErrorRow = "")
		{
			string ErrorValidate = "";

			var checkSubFeeId = await _context.SubFee.Where(x => x.SubFeeId == SfId).FirstOrDefaultAsync();
			if (checkSubFeeId == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Phụ Phí: " + SfId + " không tồn tại ";
			}

			var checkIdTcommand = await _context.DieuPhoi.Where(x => x.Id == IdTcommand).FirstOrDefaultAsync();
			if (checkIdTcommand == null)
			{
				ErrorValidate += "Lỗi Dòng >>> " + ErrorRow + " - Mã Điều Phối: " + IdTcommand + " không tồn tại ";
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
						  && status.LangId == tempData.LangID
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
				DiaDiem = x.sfc.PlaceId == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfc.PlaceId).Select(y => y.TenDiaDiem).FirstOrDefault(),
				MaSoXe = x.dp.MaSoXe,
				TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
				SubFee = x.sf.SfName,
				Price = x.sfc.Price,
				Payfor = _context.SftPayFor.Where(y => y.PayForId == x.sfc.PayForId).Select(y => y.PfName).FirstOrDefault(),
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
			var getListSubFeeByContract = from dp in _context.DieuPhoi
										  join sfc in _context.SubFeeByContract
										  on dp.Id equals sfc.MaDieuPhoi
										  join sfp in _context.SubFeePrice
										  on sfc.PriceId equals sfp.PriceId
										  join sf in _context.SubFee
										  on sfp.SfId equals sf.SubFeeId
										  join tt in _context.StatusText
										  on sfp.Status equals tt.StatusId
										  where dp.Id == id
										  && tt.LangId == tempData.LangID
										  select new { dp, sfp, sf, tt };

			var dataSubFeeIncurred = from dp in _context.DieuPhoi
									 join sfc in _context.SfeeByTcommand
									 on dp.Id equals sfc.IdTcommand
									 join sf in _context.SubFee
									 on sfc.SfId equals sf.SubFeeId
									 join status in _context.StatusText
									 on sfc.ApproveStatus equals status.StatusId
									 where sfc.ApproveStatus == 14
									 && sfc.IdTcommand == id
									 && status.LangId == tempData.LangID
									 orderby sfc.CreatedDate descending
									 select new { dp, sfc, sf, status };

			var dataSubFeeByContract = await getListSubFeeByContract.Select(x => new ListSubFeeIncurred()
			{
				Id = x.sfp.PriceId,
				MaVanDon = x.dp.MaVanDon,
				MaSoXe = null,
				TaiXe = null,
				SubFee = x.sf.SfName,
				Type = "Phụ phí theo hợp đồng",
				Price = x.sfp.Price,
				Payfor = x.sfp.CusType == "KH" ? "Khách Hàng" : "Đơn Vị Vận Tải",
				priceType = x.sfp.PriceType,
				TrangThai = x.tt.StatusContent,
				ApprovedDate = x.sfp.ApprovedDate,
				CreatedDate = x.sfp.CreatedDate,
			}).ToListAsync();

			var listDataSubFeeIncurred = await dataSubFeeIncurred.Select(x => new ListSubFeeIncurred()
			{
				Id = x.sfc.Id,
				MaVanDon = x.dp.MaVanDon,
				MaSoXe = x.dp.MaSoXe,
				DiaDiem = x.sfc.PlaceId == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfc.PlaceId).Select(y => y.TenDiaDiem).FirstOrDefault(),
				Type = "Phụ Phí Phát Sinh",
				TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == x.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
				SubFee = x.sf.SfName,
				priceType = "VND",
				Payfor = _context.SftPayFor.Where(y => y.PayForId == x.sfc.PayForId).Select(y => y.PfName).FirstOrDefault(),
				Price = x.sfc.Price,
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
				PlaceId = result.sfc.PlaceId,
				TaiXe = _context.TaiXe.Where(y => y.MaTaiXe == result.dp.MaTaiXe).Select(x => x.HoVaTen).FirstOrDefault(),
				MaSoXe = result.dp.MaSoXe,
				sftPayfor = result.sfc.PayForId,
				Romooc = result.dp.MaRomooc,
				LoaiPhuongTien = result.dp.MaLoaiPhuongTien,
				PhuPhi = result.sf.SfName,
				FinalPrice = result.sfc.Price,
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
						getById.Approver = tempData.UserName;
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
						getById.Approver = tempData.UserName;
						_context.Update(getById);
					}
				}

				var result = await _context.SaveChangesAsync();
		

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("SFeeByTcommandManage", "UserId: " + tempData.UserID + " Approve SFeeByTcommand with data: " + JsonSerializer.Serialize(request));
					await transaction.CommitAsync();
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
				await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}
	}
}