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
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.LoloSubfeeManager;

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
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				if (request.Price < 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Đơn giá phải lớn hơn 0" };
				}

				if (request.GoodsType == null && request.VehicleType == null && request.getEmptyPlace == null && request.firstPlace == null && request.secondPlace == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Chọn ít nhất một điều kiện để tạo phụ phí" };
				}

				var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.ContractId).FirstOrDefaultAsync();
				if (getContract == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Mã Hợp Đồng Không Tồn Tại" };
				}

				var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == getContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();
				if (getNewestContract.MaHopDong != getContract.MaHopDong)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Chọn Hợp Đồng Mới Nhất" };
				}

				if (!string.IsNullOrEmpty(request.GoodsType))
				{
					var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == request.GoodsType).FirstOrDefaultAsync();

					if (checkGoodsType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
					}
				}

				if (!string.IsNullOrEmpty(request.VehicleType))
				{
					var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.VehicleType).FirstOrDefaultAsync();

					if (checkVehicleType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại Phương Tiện Không Tồn Tại" };
					}
				}

				if (request.firstPlace != null && request.secondPlace != null)
				{
					if (request.firstPlace == request.secondPlace)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm đóng hàng và điểm hạ hàng không được giống nhau" };
					}

					var checkFirstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.firstPlace).FirstOrDefaultAsync();
					if (checkFirstPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm đóng hàng không tồn tại" };
					}

					var checkSecondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.secondPlace).FirstOrDefaultAsync();
					if (checkSecondPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm hạ hàng không tồn tại" };
					}
				}

				if ((request.firstPlace == null && request.secondPlace != null) || (request.firstPlace != null && request.secondPlace == null))
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn đủ hai điểm Đóng Hàng và Hạ Hàng hoặc để trống cả hai" };
				}

				if (request.getEmptyPlace.HasValue)
				{
					var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.getEmptyPlace).FirstOrDefaultAsync();
					if (getEmptyPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm lấy trả rỗng không tồn tại" };
					}
				}

				var checkPriceType = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == request.priceType).FirstOrDefaultAsync();
				if (checkPriceType == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Loại tiền tệ không tồn tại" };
				}


				if (!string.IsNullOrEmpty(request.accountId))
				{
					var checkAccount = _context.AccountOfCustomer.Where(x => x.MaAccount == request.accountId).FirstOrDefaultAsync();

					if (checkAccount == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Account không tồn tại" };
					}
				}

				await _context.AddAsync(new SubFeePrice()
				{
					GetEmptyPlace = request.getEmptyPlace,
					FirstPlace = request.firstPlace,
					SecondPlace = request.secondPlace,
					VehicleType = request.VehicleType,
					CusType = request.CusType,
					ContractId = request.ContractId,
					PriceType = request.priceType,
					AccountId = request.accountId,
					SfId = request.SfId,
					GoodsType = request.GoodsType,
					Price = request.Price,
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
					await transaction.CommitAsync();
					await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Create SubFeePrice with data: " + JsonSerializer.Serialize(request));
					await _common.LogTimeUsedOfUser(tempData.Token);
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
			var transaction = await _context.Database.BeginTransactionAsync();

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

				if (request.GoodsType == null && request.VehicleType == null && request.getEmptyPlace == null && request.firstPlace == null && request.secondPlace == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Chọn ít nhất một điều kiện để tạo phụ phí" };
				}

				var getContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == request.ContractId).FirstOrDefaultAsync();
				if (getContract == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Mã Hợp Đồng Không Tồn Tại" };
				}

				var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == getContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();
				if (getNewestContract.MaHopDong != getContract.MaHopDong)
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui Lòng Chọn Hợp Đồng Mới Nhất" };
				}

				if (!string.IsNullOrEmpty(request.VehicleType))
				{
					var checkVehicleType = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.VehicleType).FirstOrDefaultAsync();

					if (checkVehicleType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại Phương Tiện Không Tồn Tại" };
					}
				}

				if (!string.IsNullOrEmpty(request.GoodsType))
				{
					var checkGoodsType = await _context.LoaiHangHoa.Where(x => x.MaLoaiHangHoa == request.GoodsType).FirstOrDefaultAsync();

					if (checkGoodsType == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Loại Hàng Hóa Không Tồn Tại" };
					}
				}

				if (request.firstPlace != null && request.secondPlace != null)
				{
					if (request.firstPlace == request.secondPlace)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm đóng hàng và điểm hạ hàng không được giống nhau" };
					}

					var checkFirstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.firstPlace).FirstOrDefaultAsync();
					if (checkFirstPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm đóng hàng không tồn tại" };
					}

					var checkSecondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.secondPlace).FirstOrDefaultAsync();
					if (checkSecondPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm hạ hàng không tồn tại" };
					}
				}

				if ((request.firstPlace == null && request.secondPlace != null) || (request.firstPlace != null && request.secondPlace == null))
				{
					return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn đủ hai điểm Đóng Hàng và Hạ Hàng hoặc để trống cả hai" };
				}

				if (request.getEmptyPlace.HasValue)
				{
					var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.getEmptyPlace).FirstOrDefaultAsync();
					if (getEmptyPlace == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Điểm lấy trả rỗng không tồn tại" };
					}
				}

				if (!string.IsNullOrEmpty(request.accountId))
				{
					var checkAccount = _context.AccountOfCustomer.Where(x => x.MaAccount == request.accountId).FirstOrDefaultAsync();

					if (checkAccount == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Account không tồn tại" };
					}
				}

				var checkPriceType = await _context.LoaiTienTe.Where(x => x.MaLoaiTienTe == request.priceType).FirstOrDefaultAsync();
				if (checkPriceType == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Loại tiền tệ không tồn tại" };
				}

				var checkExists = await _context.SubFeePrice.Where(x =>
				x.ContractId == request.ContractId
				&& x.SfId == request.SfId
				&& x.AccountId == request.accountId
				&& x.GoodsType == request.GoodsType
				&& x.FirstPlace == request.firstPlace
				&& x.SecondPlace == request.secondPlace
				&& x.GetEmptyPlace == request.getEmptyPlace
				&& x.VehicleType == request.VehicleType
				&& x.Price == request.Price && x.Status == 14
				).FirstOrDefaultAsync();

				if (checkExists != null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Phụ phí đã tồn tại" };
				}

				getSubFeePrice.PriceType = request.priceType;
				getSubFeePrice.AccountId = request.accountId;
				getSubFeePrice.FirstPlace = request.firstPlace;
				getSubFeePrice.SecondPlace = request.secondPlace;
				getSubFeePrice.GetEmptyPlace = request.getEmptyPlace;
				getSubFeePrice.VehicleType = request.VehicleType;
				getSubFeePrice.CusType = request.CusType;
				getSubFeePrice.ContractId = request.ContractId;
				getSubFeePrice.SfId = request.SfId;
				getSubFeePrice.GoodsType = request.GoodsType;
				getSubFeePrice.Price = request.Price;
				getSubFeePrice.Description = request.Description;
				getSubFeePrice.UpdatedDate = DateTime.Now;
				getSubFeePrice.Updater = tempData.UserName;

				_context.Update(getSubFeePrice);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Update SubFeePrice with data: " + JsonSerializer.Serialize(request));
					await transaction.CommitAsync();
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

						if(getContract.TrangThai != 24)
						{
							Errors += "Mã phụ phí: " + item.SubFeePriceId + ", không sử dụng Hợp Đồng/Phụ Lục đã duyệt, Vui lòng chọn lại Hợp Đồng/Phụ Lục đã duyệt \r\n";
							continue;
						}

						if (getContract != null)
						{
							var getNewestContract = await _context.HopDongVaPhuLuc.Where(x => x.MaKh == getContract.MaKh).OrderByDescending(x => x.ThoiGianBatDau).FirstOrDefaultAsync();

							if (getNewestContract.MaHopDong != getById.ContractId)
							{
								Errors += "Mã  phụ phí: " + item.SubFeePriceId + ", không sử dụng Hợp Đồng/Phụ Lục mới nhất, Vui lòng chọn lại Hợp Đồng/Phụ Lục mới nhất \r\n";
								continue;
							}
						}

						var checkExists = await _context.SubFeePrice.Where(x =>
						 x.CusType == getById.CusType
						 && x.AccountId == getById.AccountId
						 && x.FirstPlace == getById.FirstPlace
						 && x.SecondPlace == getById.SecondPlace
						 && x.GetEmptyPlace == getById.GetEmptyPlace
						 && x.ContractId == getById.ContractId
						 && x.SfId == getById.SfId
						 && x.GoodsType == getById.GoodsType
						 && x.VehicleType == getById.VehicleType
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
					await _common.LogTimeUsedOfUser(tempData.Token);
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
					await _common.LogTimeUsedOfUser(tempData.Token);
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
					await _common.LogTimeUsedOfUser(tempData.Token);
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
					accountId = x.sfp.AccountId,
					priceType = x.sfp.PriceType,
					VehicleType = x.sfp.VehicleType,
					PriceId = x.sfp.PriceId,
					CustomerType = x.sfp.CusType,
					CustomerId = x.sfkh.MaKh,
					ContractId = x.sfp.ContractId,
					SfId = x.sfp.SfId,
					GoodsType = x.sfp.GoodsType,
					firstPlace = x.sfp.FirstPlace,
					secondPlace = x.sfp.SecondPlace,
					getEmptyPlace = x.sfp.GetEmptyPlace,
					Price = x.sfp.Price,
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

		public async Task<List<ListSubFee>> GetListSubFeeSelect(int? placeId)
		{
			try
			{
				var getList = from sf in _context.SubFee
							  join
							  sft in _context.SubFeeType
							  on sf.SfType equals sft.SfTypeId
							  orderby sf.SubFeeId
							  select new { sf, sft };

				if (placeId != null)
				{
					var getPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == placeId).FirstOrDefaultAsync();
					getList = getList.Where(x => x.sft.MaLoaiDiaDiem == getPlace.NhomDiaDiem.Trim());
				}

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

		public async Task<PagedResponseCustom<ListCustomerOfPriceTable>> GetListContractOfUser(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getData = from cus in _context.KhachHang
						  select cus;

			var getContract = from hd in _context.HopDongVaPhuLuc
							  select hd;

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				getData = getData.Where(x => x.TenKh.Contains(filter.Keyword) || x.MaKh.Contains(filter.Keyword));
			}

			if (!string.IsNullOrEmpty(filter.customerType))
			{
				getData = getData.Where(x => x.MaLoaiKh == filter.customerType);
			}

			var totalRecords = await getData.CountAsync();

			var pagedData = await getData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListCustomerOfPriceTable()
			{
				TenChuoi = _context.ChuoiKhachHang.Where(c => c.MaChuoi == x.Chuoi).Select(c => c.TenChuoi).FirstOrDefault(),
				MaKH = x.MaKh,
				TenKH = x.TenKh,
				MaSoThue = x.MaSoThue,
				SoDienThoai = x.Sdt,
				listContractOfCustomers = getContract.Where(y => y.MaKh == x.MaKh).Select(y => new ListContractOfCustomer()
				{
					MaKH = y.MaKh,
					MaHopDong = y.MaHopDong,
					TenHopDong = y.TenHienThi,
					LoaiHinhHopTac = y.LoaiHinhHopTac,
					SanPhamDichVu = _context.LoaiSpdv.Where(c => c.MaLoaiSpdv == y.MaLoaiSpdv).Select(c => c.TenLoaiSpdv).FirstOrDefault(),
				}).ToList()
			}).ToListAsync();

			return new PagedResponseCustom<ListCustomerOfPriceTable>()
			{
				paginationFilter = validFilter,
				totalCount = totalRecords,
				dataResponse = pagedData
			};
		}

		public async Task<List<SubFeePrice>> GetListSubFeePriceActive(string customerId, string accountId, string goodTypes, int firstPlace, int secondPlace, int? emptyPlace, long? handlingId, string vehicleType)
		{
			var getFirstPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == firstPlace).FirstOrDefaultAsync();
			var getSecondPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == secondPlace).FirstOrDefaultAsync();
			var getEmptyPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == emptyPlace).Select(x => new DataTempAddress()
			{
				MaDiaDiem = x.MaDiaDiem,
				DiaDiemCha = x.DiaDiemCha,
			}).FirstOrDefaultAsync();

			if (getEmptyPlace == null)
			{
				getEmptyPlace = new DataTempAddress()
				{
					MaDiaDiem = null,
					DiaDiemCha = null,
				};
			}

			var getSFByCusId = from contract in _context.HopDongVaPhuLuc
							   join sfp in _context.SubFeePrice
							   on contract.MaHopDong equals sfp.ContractId
							   where
							   contract.MaKh == customerId
							   && sfp.AccountId == accountId
							   && sfp.Status == 14
							   && (sfp.VehicleType == null || sfp.VehicleType == vehicleType)
							   && (sfp.GoodsType == null || sfp.GoodsType == goodTypes)
							   &&
							   (((sfp.FirstPlace == getFirstPlace.MaDiaDiem || sfp.FirstPlace == getFirstPlace.DiaDiemCha) && (sfp.SecondPlace == getSecondPlace.MaDiaDiem || sfp.SecondPlace == getSecondPlace.DiaDiemCha))
							   ||
							   ((sfp.FirstPlace == getSecondPlace.MaDiaDiem || sfp.FirstPlace == getSecondPlace.DiaDiemCha) && (sfp.SecondPlace == getFirstPlace.MaDiaDiem || sfp.SecondPlace == getFirstPlace.DiaDiemCha))
							   ||
							   (sfp.FirstPlace == null && sfp.FirstPlace == null)
							   )
							   select new { contract, sfp };

			var listSf = new List<SubFeePrice>();

			var listDataSF = await getSFByCusId.ToListAsync();

			if (listDataSF.Count() > 0)
			{
				#region case 1 by 1
				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType != null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.GoodsType == goodTypes).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (emptyPlace != null)
				{
					if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace != null).Count() > 0)
					{
						var data = listDataSF.Where(x => x.sfp.GetEmptyPlace == emptyPlace).Select(x => x.sfp);
						listSf.AddRange(data);
					}
				}
				#endregion

				#region case 2 by 2
				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType != null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType == null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GetEmptyPlace == emptyPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType != null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.GoodsType == goodTypes && x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == x.sfp.SecondPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType != null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.GetEmptyPlace == emptyPlace && x.sfp.GoodsType == goodTypes).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace && x.sfp.GetEmptyPlace == emptyPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}
				#endregion

				#region case 3 by 3

				if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType != null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.GoodsType == goodTypes && x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace && x.sfp.GetEmptyPlace == emptyPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType != null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace == null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType == null && (x.sfp.FirstPlace != null && x.sfp.SecondPlace != null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.FirstPlace == firstPlace && x.sfp.SecondPlace == secondPlace && x.sfp.GetEmptyPlace == emptyPlace).Select(x => x.sfp);
					listSf.AddRange(data);
				}

				if (listDataSF.Where(x => x.sfp.VehicleType != null && x.sfp.GoodsType != null && (x.sfp.FirstPlace == null && x.sfp.SecondPlace == null) && x.sfp.GetEmptyPlace != null).Count() > 0)
				{
					var data = listDataSF.Where(x => x.sfp.GoodsType == goodTypes && x.sfp.GetEmptyPlace == emptyPlace && x.sfp.VehicleType == vehicleType).Select(x => x.sfp);
					listSf.AddRange(data);
				}
				#endregion

				#region
				foreach (var item in listDataSF.Select(x => x.sfp))
				{
					if (item.GoodsType == null && item.VehicleType == null && item.GetEmptyPlace != null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType != null && item.VehicleType == null && item.GetEmptyPlace != null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType != null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType == null && item.VehicleType != null && item.GetEmptyPlace != null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType != null && item.VehicleType != null && item.GetEmptyPlace != null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.MaDiaDiem).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == getEmptyPlace.DiaDiemCha).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType == null && item.VehicleType == null && item.GetEmptyPlace == null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType != null && item.VehicleType == null && item.GetEmptyPlace == null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == null && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType == null && item.VehicleType != null && item.GetEmptyPlace == null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == null && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}

					if (item.GoodsType != null && item.VehicleType != null && item.GetEmptyPlace == null)
					{
						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.MaDiaDiem && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.DiaDiemCha) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}

						if (listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Count() > 0)
						{
							var data = listDataSF.Where(x => x.sfp.VehicleType == vehicleType && x.sfp.GoodsType == goodTypes && (x.sfp.FirstPlace == getFirstPlace.DiaDiemCha && x.sfp.SecondPlace == getSecondPlace.MaDiaDiem) && x.sfp.GetEmptyPlace == null).Select(x => x.sfp);
							listSf.AddRange(data);
						}
					}
				}
				#endregion
			}

			//var getSFByCusType = await _context.SubFeePrice.Where(x =>
			//x.Status == 14
			//&& x.CusType == getCus.MaLoaiKh
			//&& x.ContractId == null
			//&& ((x.GoodsType == goodTypes)
			//|| (x.AreaId == (getPlace == null ? null : getPlace.MaKhuVuc))
			//|| (x.TripId == tripID)
			//|| (x.VehicleType == vehicleType)
			//|| (x.GoodsType == null && x.AreaId == null && x.TripId == null && x.VehicleType == null))).ToListAsync();

			//var dataGetByCusId = await getSFByCusId.Select(x => x.sfp).ToListAsync();

			//var listItemRemove = new List<SubFeePrice>();

			//foreach (var item in getSFByCusType)
			//{
			//    if (dataGetByCusId.Where(x =>
			//    x.CusType == item.CusType
			//    && x.SfId == item.SfId
			//    && x.GoodsType == item.GoodsType
			//    && x.AreaId == item.AreaId
			//    && x.VehicleType == item.VehicleType
			//    && x.TripId == item.TripId).Count() > 0)
			//    {
			//        listItemRemove.Add(item);
			//    }
			//}

			//if (listItemRemove.Count > 0)
			//{
			//    foreach (var item in listItemRemove)
			//    {
			//        getSFByCusType.Remove(item);
			//    }
			//}

			//var data = dataGetByCusId.Concat(getSFByCusType);

			if (handlingId != null)
			{
				var getListSFbyContract = await _context.SubFeeByContract.Where(x => x.MaDieuPhoi == handlingId).ToListAsync();
				_context.RemoveRange(getListSFbyContract);
				await _context.SaveChangesAsync();
			}

			var listResult = new List<SubFeePrice>();

			foreach (var item in listSf)
			{
				if (item != null)
				{
					if (listResult.Where(x => x.PriceId == item.PriceId).Count() == 0)
					{
						listResult.Add(item);
					}
				}
			}

			return listResult.ToList();
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
							  join kh in _context.KhachHang
							  on sfc.MaKh equals kh.MaKh
							  where status.LangId == tempData.LangID
							  orderby sfp.SfId descending
							  select new { sfp, sf, sft, sfc, status, kh };

				if (!string.IsNullOrEmpty(filter.Keyword))
				{
					getList = getList.Where(x => x.sfc.MaHopDong.Contains(filter.Keyword) || x.sfc.MaKh.Contains(filter.Keyword) || x.kh.TenKh.Contains(filter.Keyword));
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
					accountId = x.sfp.AccountId,
					priceType = x.sfp.PriceType,
					VehicleType = x.sfp.VehicleType,
					PriceId = x.sfp.PriceId,
					CustomerName = x.kh.TenKh,
					ContractId = x.sfc.MaHopDong,
					ContractName = x.sfc.TenHienThi,
					GoodsType = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.sfp.GoodsType).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
					getEmptyPlace = x.sfp.GetEmptyPlace == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.GetEmptyPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					firstPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.FirstPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					secondPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.SecondPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					sfName = x.sf.SfName,
					Status = x.status.StatusContent,
					UnitPrice = x.sfp.Price,
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

		public async Task<PagedResponseCustom<ListSubFeePriceRequest>> GetListSubFeePriceByCustomer(string customerId, ListFilter listFilter, PaginationFilter filter)
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
							  join kh in _context.KhachHang
							  on sfc.MaKh equals kh.MaKh
							  where status.LangId == tempData.LangID && sfc.MaKh == customerId
							  orderby sfp.SfId descending
							  select new { sfp, sf, sft, sfc, status, kh };

				if (!string.IsNullOrEmpty(filter.Keyword))
				{
					getList = getList.Where(x => x.sfc.MaHopDong.Contains(filter.Keyword));
				}

				if (!string.IsNullOrEmpty(filter.contractId))
				{
					getList = getList.Where(x => x.sfp.ContractId == filter.contractId);
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

				if (listFilter.listDiemDau.Count > 0)
				{
					getList = getList.Where(x => listFilter.listDiemDau.Contains(x.sfp.FirstPlace.Value));
				}

				if (listFilter.listDiemCuoi.Count > 0)
				{
					getList = getList.Where(x => listFilter.listDiemCuoi.Contains(x.sfp.SecondPlace.Value));
				}

				if (listFilter.accountIds.Count > 0)
				{
					getList = getList.Where(x => listFilter.accountIds.Contains(x.sfp.AccountId));
				}

				if (listFilter.listDiemLayTraRong.Count > 0)
				{
					getList = getList.Where(x => listFilter.listDiemLayTraRong.Contains(x.sfp.GetEmptyPlace));
				}

				if (!string.IsNullOrEmpty(filter.goodsType))
				{
					getList = getList.Where(x => x.sfp.GoodsType == filter.goodsType);
				}

				if (!string.IsNullOrEmpty(filter.vehicleType))
				{
					getList = getList.Where(x => x.sfp.VehicleType == filter.vehicleType);
				}

				var totalCount = await getList.CountAsync();

				var pagedData = await getList.OrderByDescending(x => x.sfp.PriceId).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListSubFeePriceRequest()
				{
					accountId = x.sfp.AccountId,
					VehicleType = x.sfp.VehicleType,
					PriceId = x.sfp.PriceId,
					priceType = x.sfp.PriceType,
					CustomerName = x.kh.TenKh,
					ContractId = x.sfc.MaHopDong,
					ContractName = x.sfc.TenHienThi,
					GoodsType = _context.LoaiHangHoa.Where(y => y.MaLoaiHangHoa == x.sfp.GoodsType).Select(x => x.TenLoaiHangHoa).FirstOrDefault(),
					getEmptyPlace = x.sfp.GetEmptyPlace == null ? null : _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.GetEmptyPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					firstPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.FirstPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					secondPlace = _context.DiaDiem.Where(y => y.MaDiaDiem == x.sfp.SecondPlace).Select(y => y.TenDiaDiem).FirstOrDefault(),
					sfName = x.sf.SfName,
					Status = x.status.StatusContent,
					UnitPrice = x.sfp.Price,
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