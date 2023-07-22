using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

namespace TBSLogistics.Service.Services.ContractManage
{
	public class ContractService : IContract
	{
		private readonly TMSContext _TMSContext;
		private readonly IPriceTable _priceTable;
		private readonly ICommon _common;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public ContractService(TMSContext tMSContext, ICommon common, IPriceTable priceTable, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_TMSContext = tMSContext;
			_priceTable = priceTable;
			_common = common;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<BoolActionResult> CreateContract(CreateContract request)
		{
			var transaction = await _TMSContext.Database.BeginTransactionAsync();
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

					if (request.NgayCongNo == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Không được bỏ trống ngày công nợ" };
					}

					//var checkContractCustommer = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaKh == request.MaKh && x.MaHopDongCha == null).FirstOrDefaultAsync();
					//if (checkContractCustommer != null)
					//{
					//	return new BoolActionResult { isSuccess = false, Message = "Một khách hàng chỉ có một hợp đồng chính" };
					//}
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
					NgayCongNo = request.NgayCongNo,
					Account = request.Account,
					MaHopDong = request.MaHopDong.ToUpper(),
					LoaiHinhHopTac = request.LoaiHinhHopTac,
					MaLoaiSpdv = request.MaLoaiSPDV,
					MaLoaiHinh = request.MaLoaiHinh,
					HinhThucThue = request.HinhThucThue,
					TenHienThi = request.TenHienThi,
					MaLoaiHopDong = checkCustommer.MaLoaiKh == "KH" ? "SELL" : "BUY",
					MaHopDongCha = request.SoHopDongCha,
					ThoiGianBatDau = request.ThoiGianBatDau,
					ThoiGianKetThuc = request.ThoiGianKetThuc,
					NgayThanhToan = request.NgayThanhToan,
					MaKh = request.MaKh,
					GhiChu = request.GhiChu,
					TrangThai = 50,
					UpdatedTime = DateTime.Now,
					CreatedTime = DateTime.Now,
					Creator = tempData.UserName,
				});

				var result = await _TMSContext.SaveChangesAsync();

				if (result > 0)
				{
					if (request.FileContract != null)
					{
						var rs = await UploadFile(request.FileContract, request.MaHopDong.ToUpper());
						if (rs.isSuccess == false)
						{
							return rs;
						}
					}

					if (request.FileCosting != null)
					{
						var rs_Costing = await UploadFile(request.FileCosting, request.MaHopDong.ToUpper() + "_Costing");
						if (rs_Costing.isSuccess == false)
						{
							return rs_Costing;
						}
					}

					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("ContractManage", "UserId: " + tempData.UserName + " create new contract with Data: " + JsonSerializer.Serialize(request));
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = "Tạo mới hợp đồng thành công!" };
				}
				else
				{
					await transaction.RollbackAsync();
					return new BoolActionResult { isSuccess = false, Message = "Tạo mới hợp đồng thất bại!" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("ContractManage", "UserId: " + tempData.UserName + " create new Contract has ERROR: " + ex.ToString());
				await transaction.RollbackAsync();
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}

		public async Task<BoolActionResult> EditContract(string id, EditContract request)
		{
			var transaction = await _TMSContext.Database.BeginTransactionAsync();
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

				if (string.IsNullOrEmpty(checkExists.MaHopDongCha))
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

				if (request.MaLoaiSPDV != "KHO")
				{
					request.MaLoaiHinh = null;
					request.HinhThucThue = null;
				}

				if (checkExists.TrangThai == 50 && request.TrangThai != 50)
				{
					if (request.TrangThai != 49)
					{
						return new BoolActionResult { isSuccess = false, Message = "Vui lòng chọn trạng thái Chờ Duyệt" };
					}

					checkExists.TrangThai = 49;
				}
				else
				{
					if (checkExists.TrangThai == 49 && request.TrangThai == 50)
					{
						checkExists.TrangThai = request.TrangThai;
					}

					if (checkExists.TrangThai == 24 && request.TrangThai == 26)
					{
						checkExists.TrangThai = request.TrangThai;
					}

					if (checkExists.TrangThai == 26)
					{
						return new BoolActionResult { isSuccess = false, Message = "Không thể cập nhật" };
					}
				}

				checkExists.NgayCongNo = request.NgayCongNo;
				checkExists.LoaiHinhHopTac = request.LoaiHinhHopTac;
				checkExists.MaLoaiSpdv = request.MaLoaiSPDV;
				checkExists.MaLoaiHinh = request.MaLoaiHinh;
				checkExists.HinhThucThue = request.HinhThucThue;
				checkExists.NgayThanhToan = request.NgayThanhToan;
				checkExists.TenHienThi = request.TenHienThi;
				checkExists.ThoiGianBatDau = request.ThoiGianBatDau;
				checkExists.ThoiGianKetThuc = request.ThoiGianKetThuc;
				checkExists.GhiChu = request.GhiChu;

				checkExists.UpdatedTime = DateTime.Now;
				checkExists.Updater = tempData.UserName;

				_TMSContext.Update(checkExists);

				var result = await _TMSContext.SaveChangesAsync();

				if (result > 0)
				{
					if (request.FileContract != null)
					{
						var rs = await UploadFile(request.FileContract, id.ToUpper());
						if (rs.isSuccess == false)
						{
							return rs;
						}
					}

					if (request.FileCosting != null)
					{
						var rs_Costing = await UploadFile(request.FileCosting, id.ToUpper() + "_Costing");
						if (rs_Costing.isSuccess == false)
						{
							return rs_Costing;
						}
					}
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("ContractManage", "UserId: " + tempData.UserName + " Update contract with Data: " + JsonSerializer.Serialize(request));
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật hợp đồng thành công!" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Cập nhật hợp đồng thất bại!" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("ContractManage", "UserId: " + tempData.UserName + " Update Contract has ERROR: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}

		public async Task<GetContractById> GetContractById(string id)
		{
			try
			{
				var getContractById = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == id).FirstOrDefaultAsync();

				return new GetContractById()
				{
					Account = getContractById.Account,
					NgayCongNo = getContractById.NgayCongNo,
					LoaiHinhHopTac = getContractById.LoaiHinhHopTac,
					LoaiSPDV = getContractById.MaLoaiSpdv,
					LoaiHinhKho = getContractById.MaLoaiHinh,
					HinhThucThueKho = getContractById.HinhThucThue,
					NgayThanhToan = getContractById.NgayThanhToan,
					MaHopDong = getContractById.MaHopDong,
					SoHopDongCha = getContractById.MaHopDongCha,
					TenHienThi = getContractById.TenHienThi,
					MaKh = getContractById.MaKh,
					TenKh = _TMSContext.KhachHang.Where(x => x.MaKh == getContractById.MaKh).Select(x => x.TenKh).FirstOrDefault(),
					PhanLoaiHopDong = getContractById.MaLoaiHopDong,
					ThoiGianBatDau = getContractById.ThoiGianBatDau,
					ThoiGianKetThuc = getContractById.ThoiGianKetThuc,
					GhiChu = getContractById.GhiChu,
					TrangThai = getContractById.TrangThai,
					FileContract = _TMSContext.Attachment.Where(y => y.MaHopDong == getContractById.MaHopDong).OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
					FileCosing = _TMSContext.Attachment.Where(y => y.MaHopDong == getContractById.MaHopDong + "_Costing").OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
				};
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		public async Task<PagedResponseCustom<ListContract>> GetListContractApprove(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var listData = from contract in _TMSContext.HopDongVaPhuLuc
						   join cus in _TMSContext.KhachHang
						   on contract.MaKh equals cus.MaKh
						   join tt in _TMSContext.StatusText
						   on contract.TrangThai equals tt.StatusId
						   where tt.LangId == tempData.LangID && contract.TrangThai == 49
						   orderby contract.UpdatedTime descending
						   select new
						   {
							   contract,
							   cus,
							   tt
						   };

			if (!string.IsNullOrEmpty(filter.Keyword))
			{
				listData = listData.Where(x => x.cus.TenKh.Contains(filter.Keyword) || x.contract.MaHopDong.Contains(filter.Keyword));
			}

			var totalCount = await listData.CountAsync();

			var pagedData = await listData.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListContract()
			{
				countPriceTable = _TMSContext.BangGia.Where(y => y.MaHopDong == x.contract.MaHopDong && y.TrangThai == 3).Count(),
				Account = x.contract.Account,
				ChuoiKhachHang = _TMSContext.ChuoiKhachHang.Where(y => y.MaChuoi == x.cus.Chuoi).Select(y => y.TenChuoi).FirstOrDefault(),
				NgayThanhToan = x.contract.NgayThanhToan,
				MaHopDong = x.contract.MaHopDong,
				TenHienThi = x.contract.TenHienThi,
				LoaiHinhHopTac = x.contract.LoaiHinhHopTac == "TrucTiep" ? "Trực Tiếp" : "Gián Tiếp",
				MaLoaiSPDV = _TMSContext.LoaiSpdv.Where(y => y.MaLoaiSpdv == x.contract.MaLoaiSpdv).Select(y => y.TenLoaiSpdv).FirstOrDefault(),
				MaLoaiHinh = _TMSContext.LoaiHinhKho.Where(y => y.MaLoaiHinh == x.contract.MaLoaiHinh).Select(y => y.TenLoaiHinh).FirstOrDefault(),
				HinhThucThue = x.contract.HinhThucThue != null ? (x.contract.HinhThucThue == "CoDinh" ? "Cố Định" : "Không Cố Định") : "",
				MaKh = x.contract.MaKh,
				TenKH = x.cus.TenKh,
				PhanLoaiHopDong = x.contract.MaLoaiHopDong,
				TrangThai = x.tt.StatusContent,
				ThoiGianBatDau = x.contract.ThoiGianBatDau,
				ThoiGianKetThuc = x.contract.ThoiGianKetThuc,
				FileCosing = _TMSContext.Attachment.Where(y => y.MaHopDong == x.contract.MaHopDong + "_Costing").OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
				FileContract = _TMSContext.Attachment.Where(y => y.MaHopDong == x.contract.MaHopDong).OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
			}).ToListAsync();

			return new PagedResponseCustom<ListContract>()
			{
				dataResponse = pagedData,
				totalCount = totalCount,
				paginationFilter = validFilter
			};
		}

		public async Task<BoolActionResult> ApproveContract(List<ApproveContract> request)
		{
			try
			{
				if (request.Count < 1)
				{
					return new BoolActionResult { isSuccess = false, Message = "Input không hợp lệ" };
				}

				foreach (var item in request)
				{
					var getById = await _TMSContext.HopDongVaPhuLuc.Where(x => x.MaHopDong == item.contractId).FirstOrDefaultAsync();

					if (getById == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Mã Hợp Đồng/Phụ Lục: " + getById.MaHopDong + ", không tồn tại trong hệ thống" };
					}

					if (getById.TrangThai != 49)
					{
						return new BoolActionResult { isSuccess = false, Message = "Không thể duyệt hợp đồng/phụ lục: " + getById.MaHopDong };
					}

					var getListPriceTableByConTract = await _TMSContext.BangGia.Where(x => x.MaHopDong == getById.MaHopDong && x.TrangThai == 3).ToListAsync();

					if (item.Selection == 1)
					{
						getById.TrangThai = 50;
						getById.Updater = tempData.UserName;
						getById.UpdatedTime = DateTime.Now;

						if (getListPriceTableByConTract.Count > 0)
						{
							var approve = await _priceTable.ApprovePriceTable(new Model.Model.PriceListModel.ApprovePriceTable()
							{
								Result = getListPriceTableByConTract.Select(x => new Model.Model.PriceListModel.Result
								{
									Id = x.Id,
									IsAgree = 1
								}).ToList()
							});
						}
					}
					else
					{
						getById.TrangThai = 24;
						getById.Updater = tempData.UserName;
						getById.UpdatedTime = DateTime.Now;

						if (getListPriceTableByConTract.Count > 0)
						{
							var approve = await _priceTable.ApprovePriceTable(new Model.Model.PriceListModel.ApprovePriceTable()
							{
								Result = getListPriceTableByConTract.Select(x => new Model.Model.PriceListModel.Result
								{
									Id = x.Id,
									IsAgree = 0
								}).ToList()
							});
						}
					}

					_TMSContext.Update(getById);
				}

				var result = await _TMSContext.SaveChangesAsync();

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " Approve SubFeePrice with data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Duyệt Hợp đồng/Phụ Lục thành công!" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Duyệt Hợp đồng/Phụ Lục thất bại!" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("SubFeePriceManage", "UserId: " + tempData.UserID + " approve SubFeePrice with ERROR: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
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
							   where tt.LangId == tempData.LangID
							   orderby contract.TrangThai descending
							   select new
							   {
								   contract,
								   cus,
								   tt
							   };

				var listContract = listData.Where(x => x.contract.MaHopDongCha == null);
				var listAddendumst = listData.Where(x => x.contract.MaHopDongCha != null);

				if (!string.IsNullOrEmpty(filter.Keyword))
				{
					listContract = listContract.Where(x => x.cus.TenKh.Contains(filter.Keyword) || x.contract.MaHopDong.Contains(filter.Keyword));
					listAddendumst = listAddendumst.Where(x => x.cus.TenKh.Contains(filter.Keyword) || x.contract.MaHopDong.Contains(filter.Keyword));
				}

				if (!string.IsNullOrEmpty(filter.contractType))
				{
					listContract = listContract.Where(x => x.contract.MaLoaiHopDong == filter.contractType);
					listAddendumst = listAddendumst.Where(x => x.contract.MaLoaiHopDong == filter.contractType);
				}

				if (!string.IsNullOrEmpty(filter.customerType))
				{
					listContract = listContract.Where(x => x.cus.MaLoaiKh == filter.customerType);
					listAddendumst = listAddendumst.Where(x => x.cus.MaLoaiKh == filter.customerType);
				}

				if (!string.IsNullOrEmpty(filter.fromDate.ToString().Trim()) && !string.IsNullOrEmpty(filter.toDate.ToString().Trim()))
				{
					listContract = listContract.Where(x => x.contract.UpdatedTime >= filter.fromDate && x.contract.UpdatedTime <= filter.toDate);
					listAddendumst = listAddendumst.Where(x => x.contract.UpdatedTime >= filter.fromDate && x.contract.UpdatedTime <= filter.toDate);
				}

				if (!string.IsNullOrEmpty(filter.customerId))
				{
					listContract = listContract.Where(x => x.cus.MaKh == filter.customerId);
					listAddendumst = listAddendumst.Where(x => x.cus.MaKh == filter.customerId);
				}

				if (!string.IsNullOrEmpty(filter.statusId))
				{
					listContract = listContract.Where(x => x.contract.TrangThai == int.Parse(filter.statusId));
					listAddendumst = listAddendumst.Where(x => x.contract.TrangThai == int.Parse(filter.statusId));
				}
				var totalCount = await listContract.CountAsync();

				if (totalCount == 0)
				{
					var listChild = await listAddendumst.ToListAsync();

					if (listChild.Count > 0)
					{
						listContract = listData.Where(x => listChild.Select(y => y.contract.MaHopDongCha).Contains(x.contract.MaHopDong));
						totalCount = listContract.Count();
					}
				}

				var pagedData = await listContract.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListContract()
				{
					ChuoiKhachHang = _TMSContext.ChuoiKhachHang.Where(y => y.MaChuoi == x.cus.Chuoi).Select(y => y.TenChuoi).FirstOrDefault(),
					NgayThanhToan = x.contract.NgayThanhToan,
					MaHopDong = x.contract.MaHopDong,
					TenHienThi = x.contract.TenHienThi,
					LoaiHinhHopTac = x.contract.LoaiHinhHopTac == "TrucTiep" ? "Trực Tiếp" : "Gián Tiếp",
					MaLoaiSPDV = _TMSContext.LoaiSpdv.Where(y => y.MaLoaiSpdv == x.contract.MaLoaiSpdv).Select(y => y.TenLoaiSpdv).FirstOrDefault(),
					MaLoaiHinh = _TMSContext.LoaiHinhKho.Where(y => y.MaLoaiHinh == x.contract.MaLoaiHinh).Select(y => y.TenLoaiHinh).FirstOrDefault(),
					HinhThucThue = x.contract.HinhThucThue != null ? (x.contract.HinhThucThue == "CoDinh" ? "Cố Định" : "Không Cố Định") : "",
					MaKh = x.contract.MaKh,
					TenKH = x.cus.TenKh,
					NgayCongNo = x.contract.NgayCongNo,
					PhanLoaiHopDong = x.contract.MaLoaiHopDong,
					TrangThai = x.tt.StatusContent,
					ThoiGianBatDau = x.contract.ThoiGianBatDau,
					ThoiGianKetThuc = x.contract.ThoiGianKetThuc,
					FileCosing = _TMSContext.Attachment.Where(y => y.MaHopDong == x.contract.MaHopDong + "_Costing").OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
					FileContract = _TMSContext.Attachment.Where(y => y.MaHopDong == x.contract.MaHopDong).OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
					listAddendums = listAddendumst.Where(y => y.contract.MaHopDongCha == x.contract.MaHopDong).Select(z => new ListContract()
					{
						MaHopDong = z.contract.MaHopDong,
						TenHienThi = z.contract.TenHienThi,
						Account = z.contract.Account,
						MaKh = z.contract.MaKh,
						LoaiHinhHopTac = z.contract.LoaiHinhHopTac == "TrucTiep" ? "Trực Tiếp" : "Gián Tiếp",
						MaLoaiSPDV = _TMSContext.LoaiSpdv.Where(y => y.MaLoaiSpdv == z.contract.MaLoaiSpdv).Select(y => y.TenLoaiSpdv).FirstOrDefault(),
						MaLoaiHinh = _TMSContext.LoaiHinhKho.Where(y => y.MaLoaiHinh == z.contract.MaLoaiHinh).Select(y => y.TenLoaiHinh).FirstOrDefault(),
						HinhThucThue = z.contract.HinhThucThue != null ? (z.contract.HinhThucThue == "CoDinh" ? "Cố Định" : "Không Cố Định") : "",
						PhanLoaiHopDong = z.contract.MaLoaiHopDong,
						TrangThai = z.tt.StatusContent,
						FileContract = _TMSContext.Attachment.Where(y => y.MaHopDong == z.contract.MaHopDong).OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
						FileCosing = _TMSContext.Attachment.Where(y => y.MaHopDong == z.contract.MaHopDong + "_Costing").OrderByDescending(x => x.Id).Select(y => y.Id).FirstOrDefault().ToString(),
						ThoiGianBatDau = z.contract.ThoiGianBatDau,
						ThoiGianKetThuc = z.contract.ThoiGianKetThuc,
					}).ToList()
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

		public async Task<List<GetContractById>> GetListContractSelect(string MaKH, bool getChild, bool getProductService, bool listApprove)
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

			if (listApprove == true)
			{
				getList = getList.Where(x => x.ct.TrangThai == 50);
			}

			var list = await getList.Select(x => new GetContractById()
			{
				MaHopDong = x.ct.MaHopDong,
				TenHienThi = x.ct.TenHienThi,
				SoHopDongCha = x.ct.MaHopDongCha == null ? "Hợp Đồng" : "Phụ Lục",
			}).ToListAsync();

			return list;
		}

		public async Task<ListOptionSelect> GetListOptionSelect()
		{
			var getSPDV = await _TMSContext.LoaiSpdv.ToListAsync();
			var getWHType = await _TMSContext.LoaiHinhKho.ToListAsync();

			return new ListOptionSelect()
			{
				dsLoaiHinhKho = getWHType,
				dsSPDV = getSPDV,
			};
		}

		private async Task<BoolActionResult> UploadFile(IFormFile file, string maHopDong)
		{
			var PathFolder = $"Contract/{maHopDong}";

			var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
			var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), Guid.NewGuid().ToString());
			var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";

			var supportedTypes = new[] { "pdf", "docx" };

			if (maHopDong.Contains("_Costing"))
			{
				supportedTypes = new[] { "pdf", "docx", "xlsx" };
			}

			var fileExt = Path.GetExtension(originalFileName).Substring(1).ToLower();
			if (!supportedTypes.Contains(fileExt))
			{
				return new BoolActionResult { isSuccess = false, Message = "File không được hỗ trợ, chỉ hỗ trợ .pdf, .docx" };
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
			await _common.LogTimeUsedOfUser(tempData.Token);
			return new BoolActionResult { isSuccess = true }; ;
		}
	}
}