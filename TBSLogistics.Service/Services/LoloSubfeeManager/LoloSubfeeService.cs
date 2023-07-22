using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.SubfeeLoloModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;
using DateTime = System.DateTime;

namespace TBSLogistics.Service.Services.LoloSubfeeManager
{
	public class LoloSubfeeService : ILoloSubfee
	{
		private readonly TMSContext _context;
		private readonly ICommon _common;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private TempData tempData;

		public LoloSubfeeService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor)
		{
			_context = context;
			_httpContextAccessor = httpContextAccessor;
			_common = common;
			tempData = _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", ""));
		}

		public async Task<BoolActionResult> ApproveSubfeeLolo(ApprovePriceTable request)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				if (request.Result.Count < 1)
				{
					return new BoolActionResult { isSuccess = false, Message = "Không có dữ liệu nào để duyệt" };
				}

				var checkData = await _context.PhuPhiNangHa.Where(x => request.Result.Select(y => y.Id).Contains(x.Id)).ToListAsync();

				if (checkData.Count != request.Result.Count)
				{
					return new BoolActionResult { isSuccess = false, Message = "Dữ liệu không tồn tại trong hệ thống" };
				}

				foreach (var item in checkData)
				{
					foreach (var itemRq in request.Result)
					{
						if (item.Id == itemRq.Id)
						{
							if (item.TrangThai != 51)
							{
								return new BoolActionResult { isSuccess = false, Message = "Chỉ phụ phí có trạng thái chờ duyệt mới có thể duyệt" };
							}

							if (itemRq.IsAgree == 1)
							{
								item.TrangThai = 54;
								_context.Update(item);
							}

							if (itemRq.IsAgree == 0)
							{
								var checkOldItem = await _context.PhuPhiNangHa.Where(x =>
								x.MaKh == item.MaKh &&
								x.MaDiaDiem == item.MaDiaDiem &&
								x.LoaiPhuPhi == item.LoaiPhuPhi &&
								x.LoaiCont == item.LoaiCont &&
								x.HangTau == item.HangTau &&
								x.TrangThai == 52
								).ToListAsync();

								if (checkOldItem != null)
								{
									checkOldItem.ForEach(x => { x.TrangThai = 53; x.UpdateTime = DateTime.Now; x.NgayHetHieuLuc = DateTime.Now; });
								}

								item.TrangThai = 52;
								item.UpdateTime = DateTime.Now;
								item.NgayApDung = DateTime.Now;
								item.Approver = tempData.UserName;
								_context.Update(item);
							}
						}
					}
				}

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await transaction.CommitAsync();
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("SubfeeLolo", "UserId: " + tempData.UserName + " Approve SubfeeLolo with Data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Duyệt phụ phí LoLo thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Duyệt phụ phí LoLo thất bại" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("SubfeeLolo", "UserId:" + tempData.UserID + " Approve SubfeeLolo with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		public async Task<BoolActionResult> CreateSubfeeLolo(List<CreateOrUpdateSubfeeLolo> request, bool createByExcel = false)
		{
			try
			{
				string ErrorValidate = "";
				int rows = 0;

				if (createByExcel)
				{
					rows = 3;
				}

				foreach (var item in request)
				{
					rows++;
					var checkDuplicate = request.Where(x =>
					x.MaDiaDiem == item.MaDiaDiem
					&& x.MaKh == item.MaKh
					&& x.LoaiPhuPhi == item.LoaiPhuPhi
					&& x.LoaiCont == item.LoaiCont
					&& x.HangTau == item.HangTau).ToList();

					if (checkDuplicate.Count > 1)
					{
						ErrorValidate += "Dòng " + rows + ": Bị trùng lặp, vui lòng kiểm tra lại! ";
					}

					if (!string.IsNullOrEmpty(item.MaKh))
					{
						var checkCustomer = await _context.KhachHang.Where(x => x.MaKh == item.MaKh).FirstOrDefaultAsync();
						if (checkCustomer == null)
						{
							ErrorValidate += "Dòng " + rows + ": Khách hàng " + item.MaKh + " không tồn tại";
						}
					}

					if (!string.IsNullOrEmpty(item.HangTau))
					{
						var checkShipCode = await _context.ShippingInfomation.Where(x => x.ShippingCode == item.HangTau).FirstOrDefaultAsync();
						if (checkShipCode == null)
						{
							ErrorValidate += "Dòng " + rows + ": Hãng Tàu " + item.HangTau + " không tồn tại";
						}
					}

					var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == item.MaDiaDiem && x.DiaDiemCha != null).FirstOrDefaultAsync();
					if (checkPlace == null)
					{
						ErrorValidate += "Dòng " + rows + ": Địa điểm nâng/hạ không tồn tại! ";
					}

					var checkCont = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == item.LoaiCont && x.PhanLoai == "Cont").FirstOrDefaultAsync();
					if (checkCont == null)
					{
						ErrorValidate += "Dòng " + rows + ": Loại Cont không tồn tại! ";
					}

					if (item.LoaiPhuPhi != 1 && item.LoaiPhuPhi != 2)
					{
						ErrorValidate += "Dòng " + rows + ": Loại phụ phí không tồn tại! ";
					}

					var checkData = await _context.PhuPhiNangHa.Where(x => x.MaDiaDiem == item.MaDiaDiem
					&& x.MaKh == item.MaKh
					&& x.HangTau == item.HangTau
					&& x.LoaiCont == item.LoaiCont
					&& x.LoaiPhuPhi == item.LoaiPhuPhi
					&& x.TrangThai == 51).FirstOrDefaultAsync();

					if (checkData != null)
					{
						ErrorValidate += "Dòng " + rows + ": đã tồn tại trên hệ thống và ở trạng thái chưa duyệt";
					}

					await _context.PhuPhiNangHa.AddAsync(new PhuPhiNangHa()
					{
						MaDiaDiem = item.MaDiaDiem,
						MaKh = item.MaKh,
						HangTau = item.HangTau,
						LoaiCont = item.LoaiCont,
						LoaiPhuPhi = item.LoaiPhuPhi,
						DonGia = item.DonGia,
						TrangThai = 51,
						CreatedTime = DateTime.Now,
						Creator = tempData.UserName,
					});
				}

				if (ErrorValidate != "")
				{
					return new BoolActionResult { isSuccess = false, Message = ErrorValidate };
				}

				var reuslt = await _context.SaveChangesAsync();
				await _common.LogTimeUsedOfUser(tempData.Token);
				await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Create SubfeeLolo with Data: " + JsonSerializer.Serialize(request));
				return new BoolActionResult { isSuccess = true, Message = "Tạo mới Phụ Phí Lolo thành công" };
			}
			catch (Exception ex)
			{
				await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " create new SubfeeLolo with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}

		public async Task<BoolActionResult> CreateSubfeeLoloByExcel(IFormFile formFile, CancellationToken cancellationToken)
		{
			var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				int ErrorRow = 0;
				if (formFile == null || formFile.Length <= 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
				}

				if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
				{
					return new BoolActionResult { isSuccess = false, Message = "Not Support file extension" };
				}

				var list = new List<CreateOrUpdateSubfeeLolo>();

				using var stream = new MemoryStream();
				await formFile.CopyToAsync(stream, cancellationToken);
				using var wbook = new XLWorkbook(stream);
				var ws1 = wbook.Worksheet(1);
				var rowWs1 = ws1.RowsUsed().Count();

				if (rowWs1 == 0)
				{
					return new BoolActionResult { isSuccess = false, Message = "This file is empty" };
				}

				if (
					ws1.Cell(1, 1).Value.ToString().Trim() != "MaKH" ||
					ws1.Cell(1, 2).Value.ToString().Trim() != "MaDiaDiem" ||
					ws1.Cell(1, 3).Value.ToString().Trim() != "HangTau" ||
					ws1.Cell(1, 4).Value.ToString().Trim() != "LoaiCont" ||
					ws1.Cell(1, 5).Value.ToString().Trim() != "LoaiPhuPhi" ||
					ws1.Cell(1, 6).Value.ToString().Trim() != "DonGia")
				{
					return new BoolActionResult { isSuccess = false, Message = "File excel không đúng định dạng chuẩn" };
				}

				for (int row = 3; row <= rowWs1; row++)
				{
					ErrorRow = row;

					string MaKH = ws1.Cell(row, 1).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 1).Value.ToString().Trim().ToUpper();
					string MaDiaDiem = ws1.Cell(row, 2).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 2).Value.ToString().Trim();
					string HangTau = ws1.Cell(row, 3).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 3).Value.ToString().Trim().ToUpper();
					string LoaiCont = ws1.Cell(row, 4).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 4).Value.ToString().Trim().ToUpper();
					string LoaiPhuPhi = ws1.Cell(row, 5).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 5).Value.ToString().Trim().ToUpper();
					string DonGia = ws1.Cell(row, 6).GetValue<string>().Trim() == "" ? null : ws1.Cell(row, 6).Value.ToString().Trim();

					if (MaDiaDiem == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Mã Hợp Đồng" };
					}

					if (LoaiCont == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Loại Container" };
					}

					if (LoaiPhuPhi == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Không được để trống Loại Phụ Phí" };
					}

					var checkPrice = decimal.TryParse(DonGia, out _);
					if (!checkPrice)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Đơn giá không hợp lệ" };
					}

					var checkPlace = int.TryParse(MaDiaDiem, out _);
					if (!checkPlace)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Mã địa điểm không hợp lệ" };
					}

					var checkSfType = int.TryParse(LoaiPhuPhi, out _);
					if (!checkPlace)
					{
						return new BoolActionResult { isSuccess = false, Message = "Dòng " + row + ":Loại phụ phí không hợp lệ" };
					}

					list.Add(new CreateOrUpdateSubfeeLolo()
					{
						MaDiaDiem = int.Parse(MaDiaDiem),
						MaKh = MaKH,
						HangTau = HangTau,
						LoaiCont = LoaiCont,
						LoaiPhuPhi = int.Parse(LoaiPhuPhi),
						DonGia = decimal.Parse(DonGia),
					});
				}

				var create = await CreateSubfeeLolo(list, true);

				if (create.isSuccess)
				{
					await transaction.CommitAsync();
					return new BoolActionResult { isSuccess = true, Message = create.Message };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = create.Message };
				}
			}
			catch (Exception ex)
			{
				return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
			}
		}

		public async Task<PagedResponseCustom<ListSubfeeLolo>> GetListSubfeeLolo(PaginationFilter filter)
		{
			var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);

			var getData = from sfLolo in _context.PhuPhiNangHa
						  join kh in _context.KhachHang
						  on sfLolo.MaKh equals kh.MaKh into khsf
						  from cussf in khsf.DefaultIfEmpty()
						  join diadiem in _context.DiaDiem
						  on sfLolo.MaDiaDiem equals diadiem.MaDiaDiem
						  join vehicle in _context.LoaiPhuongTien
						  on sfLolo.LoaiCont equals vehicle.MaLoaiPhuongTien
						  join hangtau in _context.ShippingInfomation
						  on sfLolo.HangTau equals hangtau.ShippingCode into ship
						  from shipsf in ship.DefaultIfEmpty()
						  join tt in _context.StatusText
						  on sfLolo.TrangThai equals tt.StatusId
						  where tt.LangId == "VI"
						  select new { sfLolo, cussf, diadiem, vehicle, shipsf, tt };

			if (!string.IsNullOrEmpty(filter.vehicleType))
			{
				getData = getData.Where(x => x.sfLolo.LoaiCont == filter.vehicleType);
			}

			if (!string.IsNullOrEmpty(filter.placeId))
			{
				getData = getData.Where(x => x.sfLolo.MaDiaDiem == int.Parse(filter.placeId));
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				getData = getData.Where(x => x.sfLolo.MaKh == filter.customerId);
			}

			if (!string.IsNullOrEmpty(filter.statusId))
			{
				getData = getData.Where(x => x.sfLolo.TrangThai == int.Parse(filter.statusId));
			}

			if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
			{
				getData = getData.Where(x => x.sfLolo.CreatedTime.Date >= filter.fromDate.Value.Date && x.sfLolo.CreatedTime.Date <= filter.toDate.Value.Date);
			}

			var totalCount = await getData.CountAsync();

			var pagedData = await getData.OrderByDescending(x => x.sfLolo.CreatedTime).Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new ListSubfeeLolo()
			{
				Id = x.sfLolo.Id,
				MaDiaDiem = x.diadiem.MaDiaDiem,
				TenDiaDiem = x.diadiem.TenDiaDiem,
				MaKh = x.sfLolo.MaKh == null ? "" : x.cussf.MaKh,
				TenKh = x.sfLolo.MaKh == null ? "" : x.cussf.TenKh,
				HangTau = x.sfLolo.HangTau == null ? "" : x.shipsf.ShippingLineName,
				LoaiCont = x.vehicle.TenLoaiPhuongTien,
				LoaiPhuPhi = x.sfLolo.LoaiPhuPhi == 1 ? "Nâng" : "Hạ",
				DonGia = x.sfLolo.DonGia,
				TrangThai = x.tt.StatusId,
				TenTrangThai = x.tt.StatusContent,
				ApproveDate = x.sfLolo.NgayApDung,
				DisableDate = x.sfLolo.NgayHetHieuLuc,
				Approver = x.sfLolo.Approver,
				Createdtime = x.sfLolo.CreatedTime,
			}).ToListAsync();

			return new PagedResponseCustom<ListSubfeeLolo>()
			{
				dataResponse = pagedData,
				totalCount = totalCount,
				paginationFilter = validFilter
			};
		}

		public async Task<List<ListSubfeeLolo>> GetListSubfeeLoloExportExcel(PaginationFilter filter)
		{
			var getData = from sfLolo in _context.PhuPhiNangHa
						  join kh in _context.KhachHang
						  on sfLolo.MaKh equals kh.MaKh into khsf
						  from cussf in khsf.DefaultIfEmpty()
						  join diadiem in _context.DiaDiem
						  on sfLolo.MaDiaDiem equals diadiem.MaDiaDiem
						  join vehicle in _context.LoaiPhuongTien
						  on sfLolo.LoaiCont equals vehicle.MaLoaiPhuongTien
						  join hangtau in _context.ShippingInfomation
						  on sfLolo.HangTau equals hangtau.ShippingCode into ship
						  from shipsf in ship.DefaultIfEmpty()
						  join tt in _context.StatusText
						  on sfLolo.TrangThai equals tt.StatusId
						  where tt.LangId == "VI"
						  select new { sfLolo, cussf, diadiem, vehicle, shipsf, tt };

			var filterByCus = await _context.UserHasCustomer.Where(x => x.UserId == tempData.UserID).ToListAsync();
			getData = getData.Where(x => filterByCus.Select(y => y.CustomerId).Contains(x.sfLolo.MaKh));

			if (!string.IsNullOrEmpty(filter.vehicleType))
			{
				getData = getData.Where(x => x.sfLolo.LoaiCont == filter.vehicleType);
			}

			if (!string.IsNullOrEmpty(filter.placeId))
			{
				getData = getData.Where(x => x.sfLolo.MaDiaDiem == int.Parse(filter.placeId));
			}

			if (!string.IsNullOrEmpty(filter.customerId))
			{
				getData = getData.Where(x => filter.customerId == filter.customerId);
			}

			if (!string.IsNullOrEmpty(filter.statusId))
			{
				getData = getData.Where(x => x.sfLolo.TrangThai == int.Parse(filter.statusId));
			}

			if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
			{
				getData = getData.Where(x => x.sfLolo.CreatedTime.Date >= filter.fromDate.Value.Date && x.sfLolo.CreatedTime.Date <= filter.toDate.Value.Date);
			}

			var dataReturn = await getData.Select(x => new ListSubfeeLolo()
			{
				Id = x.sfLolo.Id,
				MaDiaDiem = x.diadiem.MaDiaDiem,
				TenDiaDiem = x.diadiem.TenDiaDiem,
				MaKh = x.cussf.MaKh,
				TenKh = x.cussf.TenKh,
				HangTau = x.shipsf.ShippingLineName,
				LoaiCont = x.vehicle.TenLoaiPhuongTien,
				LoaiPhuPhi = x.sfLolo.LoaiPhuPhi == 1 ? "Nâng" : "Hạ",
				DonGia = x.sfLolo.DonGia,
				TrangThai = x.tt.StatusId,
				TenTrangThai = x.tt.StatusContent,
				ApproveDate = x.sfLolo.NgayApDung,
				DisableDate = x.sfLolo.NgayHetHieuLuc,
				Approver = x.sfLolo.Approver,
				Createdtime = x.sfLolo.CreatedTime,
			}).ToListAsync();

			return dataReturn;
		}

		public async Task<GetSubfeeLoloById> GetSubfeeLoloById(int id)
		{
			var getByid = await _context.PhuPhiNangHa.Where(x => x.Id == id).FirstOrDefaultAsync();

			if (getByid != null)
			{
				return new GetSubfeeLoloById()
				{
					Id = getByid.Id,
					MaDiaDiem = getByid.MaDiaDiem,
					MaKh = getByid.MaKh,
					HangTau = getByid.HangTau,
					LoaiCont = getByid.LoaiCont,
					LoaiPhuPhi = getByid.LoaiPhuPhi,
					DonGia = getByid.DonGia,
					TrangThai = getByid.TrangThai,
				};
			}
			else
			{
				return null;
			}
		}

		public async Task<BoolActionResult> UpdateSubfeeLolo(int id, CreateOrUpdateSubfeeLolo request)
		{
			try
			{
				var findById = await _context.PhuPhiNangHa.Where(x => x.Id == id).FirstOrDefaultAsync();

				if (findById == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Phụ phí Lolo không tồn tại" };
				}

				if (findById.TrangThai != 51)
				{
					return new BoolActionResult { isSuccess = false, Message = "Phụ phí này không thể sửa đổi" };
				}

				if (!string.IsNullOrEmpty(request.MaKh))
				{
					var checkCustomer = await _context.KhachHang.Where(x => x.MaKh == request.MaKh).FirstOrDefaultAsync();
					if (checkCustomer == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Khách hàng không tồn tại" };
					}
				}

				if (!string.IsNullOrEmpty(request.HangTau))
				{
					var checkShipCode = await _context.ShippingInfomation.Where(x => x.ShippingCode == request.HangTau).FirstOrDefaultAsync();
					if (checkShipCode == null)
					{
						return new BoolActionResult { isSuccess = false, Message = "Hãng tàu không tồn tại" };
					}
				}

				var checkPlace = await _context.DiaDiem.Where(x => x.MaDiaDiem == request.MaDiaDiem && x.DiaDiemCha != null).FirstOrDefaultAsync();
				if (checkPlace == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Điểm Nâng/Hạ không tồn tại" };
				}

				var checkCont = await _context.LoaiPhuongTien.Where(x => x.MaLoaiPhuongTien == request.LoaiCont && x.PhanLoai == "Cont").FirstOrDefaultAsync();
				if (checkCont == null)
				{
					return new BoolActionResult { isSuccess = false, Message = "Loại Container không tồn tại" };
				}

				if (request.LoaiPhuPhi != 1 && request.LoaiPhuPhi != 2)
				{
					return new BoolActionResult { isSuccess = false, Message = "Loại Phụ Phí không tồn tại" };
				}

				findById.MaDiaDiem = request.MaDiaDiem;
				findById.MaKh = request.MaKh;
				findById.HangTau = request.HangTau;
				findById.LoaiCont = request.LoaiCont;
				findById.LoaiPhuPhi = request.LoaiPhuPhi;
				findById.DonGia = request.DonGia;

				_context.PhuPhiNangHa.Update(findById);

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					await _common.LogTimeUsedOfUser(tempData.Token);
					await _common.Log("PriceTableManage", "UserId: " + tempData.UserName + " Update SubfeeLolo with Data: " + JsonSerializer.Serialize(request));
					return new BoolActionResult { isSuccess = true, Message = "Cập nhật Phụ Phí Lolo thành công" };
				}
				else
				{
					return new BoolActionResult { isSuccess = false, Message = "Cập nhật Phụ Phí Lolo thất bại" };
				}
			}
			catch (Exception ex)
			{
				await _common.Log("PriceTableManage", "UserId:" + tempData.UserID + " Update SubfeeLolo with Error: " + ex.ToString());
				return new BoolActionResult { isSuccess = false, Message = ex.ToString(), DataReturn = "Exception" };
			}
		}

		public async Task<BoolActionResult> AutoAddSubfeeLolo(long handlingId)
		{
			try
			{
				var dataTransport = from vd in _context.VanDon
									join dp in _context.DieuPhoi
									on vd.MaVanDon equals dp.MaVanDon
									where dp.Id == handlingId
									select new { vd, dp };

				var getData = await dataTransport.FirstOrDefaultAsync();

				if (getData.dp.DonViVanTai == null)
				{
					return new BoolActionResult { isSuccess = false };
				}

				var getSubfeeLolo = await _context.PhuPhiNangHa.Where(x =>
				(x.MaKh == getData.vd.MaKh || x.MaKh == null)
				&& x.MaDiaDiem == (getData.vd.LoaiVanDon == "xuat" ? getData.dp.DiemLayRong : getData.dp.DiemTraRong)
				&& (x.HangTau == getData.vd.HangTau || x.HangTau == null)
				&& x.LoaiCont == getData.dp.MaLoaiPhuongTien
				&& x.LoaiPhuPhi == (getData.vd.LoaiVanDon == "xuat" ? 1 : 2)
				&& x.TrangThai == 52
				).ToListAsync();

				if (getSubfeeLolo.Count > 1)
				{
					var filterByCusAndShip = getSubfeeLolo.Where(x => x.MaKh == getData.vd.MaKh && x.HangTau == getData.vd.HangTau).ToList();

					if (filterByCusAndShip.Count == 0)
					{
						var filterByCus = getSubfeeLolo.Where(x => x.MaKh == getData.vd.MaKh && x.HangTau == null).ToList();

						if (filterByCus.Count == 0)
						{
							var filterByShip = getSubfeeLolo.Where(x => x.HangTau == getData.vd.HangTau && x.MaKh == null).ToList();

							if (filterByShip.Count == 0)
							{
								var filterFinal = getSubfeeLolo.Where(x => x.HangTau == null && x.MaKh == null).ToList();

								if (filterFinal.Count == 0)
								{
									return new BoolActionResult { isSuccess = false, Message = "Không tìm thấy phụ phí Lolo" };
								}
								else
								{
									getSubfeeLolo = filterFinal;
								}
							}
							else
							{
								getSubfeeLolo = filterByShip;
							}
						}
						else
						{
							getSubfeeLolo = filterByCus;
						}
					}
					else
					{
						getSubfeeLolo = filterByCusAndShip;
					}
				}

				var subFee = getSubfeeLolo.Select(x => new SfeeByTcommand()
				{
					IdTcommand = getData.dp.Id,
					SfId = 17,
					Price = (double)x.DonGia,
					ApproveStatus = 14,
					Note = null,
					CreatedDate = DateTime.Now,
					Creator = tempData.UserName,
					PlaceId = x.MaDiaDiem,
					PayForId = 1
				}).FirstOrDefault();

				if (subFee != null)
				{
					if (getData.dp.ReuseCont == true)
					{
						var checkSf = await _context.SfeeByTcommand.Where(x => x.IdTcommand == handlingId
						&& x.PlaceId == (getData.vd.LoaiVanDon == "xuat" ? getData.dp.DiemLayRong : getData.dp.DiemTraRong)
						&& x.SfId == 17).ToListAsync();

						if (checkSf.Count == 0)
						{
							await _context.SfeeByTcommand.AddAsync(subFee);
						}
					}
					else
					{
						var checkSf = await _context.SfeeByTcommand.Where(x => x.IdTcommand == handlingId
						&& x.PlaceId == (getData.vd.LoaiVanDon == "xuat" ? getData.dp.DiemLayRong : getData.dp.DiemTraRong)
						&& x.SfId == 17).ToListAsync();

						if (checkSf.Count == 0)
						{
							var listSubfee = new List<SfeeByTcommand>()
							{
								new SfeeByTcommand(){
									IdTcommand = subFee.IdTcommand,
									SfId = 17,
									Price = subFee.Price,
									ApproveStatus = subFee.ApproveStatus,
									Note = null,
									CreatedDate = DateTime.Now,
									Creator = tempData.UserName,
									PlaceId = subFee.PlaceId,
									PayForId = 1
								},
								new SfeeByTcommand(){
									IdTcommand = subFee.IdTcommand,
									SfId = 17,
									Price = subFee.Price,
									ApproveStatus = subFee.ApproveStatus,
									Note = null,
									CreatedDate = DateTime.Now,
									Creator = tempData.UserName,
									PlaceId = subFee.PlaceId,
									PayForId = 2
								},
							};
							await _context.SfeeByTcommand.AddRangeAsync(listSubfee);
						}
					}
				}

				var result = await _context.SaveChangesAsync();

				if (result > 0)
				{
					return new BoolActionResult { isSuccess = true };
				}
				else
				{
					return new BoolActionResult { isSuccess = false };
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}
	}
}