using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class PriceTableController : ControllerBase
	{
		private readonly IPriceTable _priceTable;
		private readonly IPaginationService _paninationService;
		private readonly IAddress _address;
		private readonly ICommon _common;
		private readonly TMSContext _context;

		public PriceTableController(IPriceTable priceTable, IPaginationService paninationService, ICommon common, TMSContext context, IAddress address)
		{
			_address = address;
			_priceTable = priceTable;
			_paninationService = paninationService;
			_common = common;
			_context = context;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
		{
			var checkPermissiCreate = await _common.CheckPermission("C0002");
			var checkPermission = await _common.CheckPermission("C0003");
			if (checkPermissiCreate.isSuccess == true && checkPermission.isSuccess == true)
			{
				var createPricetable = await _priceTable.CreatePriceTable(request, true);

				if (createPricetable.isSuccess == true)
				{
					return Ok(createPricetable.Message);
				}
				else
				{
					return BadRequest(createPricetable.Message);
				}
			}

			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var create = await _priceTable.CreatePriceTable(request, false);

			if (create.isSuccess == true)
			{
				return Ok(create.Message);
			}
			else
			{
				return BadRequest(create.Message);
			}


		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> GetListPriceTable([FromQuery] PaginationFilter filter)
		{
			var checkPermissionKH = await _common.CheckPermission("C0001");
			var checkPermissionNCC = await _common.CheckPermission("C0005");

			if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
			{
				return BadRequest("Bạn không có quyền hạn");
			}

			if (filter.customerType == "NCC")
			{
				if (!checkPermissionNCC.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			if (filter.customerType == "KH")
			{
				if (!checkPermissionKH.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			var route = Request.Path.Value;
			var pagedData = await _priceTable.GetListPriceTable(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustomerOfPriceTable>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
			return Ok(pagedReponse);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> GetListPriceTableByContractId([FromQuery] PaginationFilter filter, ListFilter listFilter, string Id, string onlyct = null)
		{
			var checkPermissionKH = await _common.CheckPermission("C0001");
			var checkPermissionNCC = await _common.CheckPermission("C0005");

			if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
			{
				return BadRequest("Bạn không có quyền hạn");
			}

			var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == Id).FirstOrDefaultAsync();

			if (checkContract.MaKh.Contains("SUP"))
			{
				if (!checkPermissionNCC.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			if (checkContract.MaKh.Contains("CUS"))
			{
				if (!checkPermissionKH.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			var route = Request.Path.Value;
			var pagedData = await _priceTable.GetListPriceTableByContractId(Id, onlyct, listFilter, filter);
			var pagedReponse = PaginationHelper.CreatePagedReponse<GetPriceListRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListPriceTableByCustomerId(string Id)
		{
			var checkPermission = await _common.CheckPermission("C0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var list = await _priceTable.GetListPriceTableByCustommerId(Id);

			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListPriceTableApprove([FromQuery] PaginationFilter filter, string contractId = null)
		{
			var checkPermission = await _common.CheckPermission("C0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _priceTable.GetListPriceTableApprove(contractId, filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListApprove>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
			return Ok(pagedReponse);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ApprovePriceTable(ApprovePriceTable request)
		{
			var checkPermission = await _common.CheckPermission("C0002");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var approve = await _priceTable.ApprovePriceTable(request);

			if (approve.isSuccess == true)
			{
				return Ok(approve.Message);
			}
			else
			{
				return BadRequest(approve.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetPriceTableById(int id)
		{
			var checkPermission = await _common.CheckPermission("C0004");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var data = await _priceTable.GetPriceTableById(id);

			return Ok(data);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> UpdatePriceTable(int id, GetPriceListById request)
		{
			var checkPermission = await _common.CheckPermission("C0004");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var update = await _priceTable.UpdatePriceTable(id, request);

			if (update.isSuccess)
			{
				return Ok(update.Message);
			}
			else
			{
				return BadRequest(update.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ReadFileExcelPricetable(IFormFile formFile, CancellationToken cancellationToken)
		{
			var checkPermissiCreate = await _common.CheckPermission("C0002");
			var checkPermission = await _common.CheckPermission("C0003");
			if (checkPermissiCreate.isSuccess == true && checkPermission.isSuccess == true)
			{
				var ImportExcel = await _priceTable.CreatePriceByExcel(formFile, true, cancellationToken);

				if (ImportExcel.isSuccess)
				{
					return Ok(ImportExcel.Message);
				}
				else
				{
					return Ok(ImportExcel.Message);
				}
			}

			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var create = await _priceTable.CreatePriceByExcel(formFile, false, cancellationToken);

			if (create.isSuccess == true)
			{
				return Ok(create.Message);
			}
			else
			{
				return BadRequest(create.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> RevertPriceTableHandling(string contractId, string cusId)
		{
			var revert = await _priceTable.RevertPriceTableOfHandling(contractId, cusId);

			if (revert.isSuccess)
			{
				return Ok(revert.Message);
			}
			else
			{
				return BadRequest(revert.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportExcelPriceTable(string cusType)
		{
			var checkPermissionKH = await _common.CheckPermission("C0006");
			var checkPermissionNCC = await _common.CheckPermission("C0007");

			if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
			{
				return BadRequest("Bạn không có quyền hạn");
			}

			if (cusType == "NCC")
			{
				if (!checkPermissionNCC.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			if (cusType == "KH")
			{
				if (!checkPermissionKH.isSuccess)
				{
					return BadRequest("Bạn không có quyền hạn");
				}
			}

			var data = await _priceTable.GetListPriceTableExportExcel(cusType);

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("BangGia");

			var currRow = 1;
			worksheet.Range("A1:N1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:N1").Style.Font.FontSize = 15;
			worksheet.Range("A1:N1").Style.Font.Bold = true;
			worksheet.Range("A1:N1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet.Range("A1:N1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(currRow, 1).Value = "Hợp Đồng";
			worksheet.Cell(currRow, 2).Value = "Phân Loại";
			worksheet.Cell(currRow, 3).Value = cusType == "KH" ? "Khách Hàng" : "Đơn Vị Vận Tải";
			worksheet.Cell(currRow, 4).Value = "Account";
			worksheet.Cell(currRow, 5).Value = "Điểm Đóng Hàng";
			worksheet.Cell(currRow, 6).Value = "Điểm Hạ Hàng";
			worksheet.Cell(currRow, 7).Value = "Điểm Lấy/Trả Rỗng";
			worksheet.Cell(currRow, 8).Value = "Đơn Giá VND";
			worksheet.Cell(currRow, 9).Value = "Loại Tiền Tệ";
			worksheet.Cell(currRow, 10).Value = "PTVC";
			worksheet.Cell(currRow, 11).Value = "Loại Phương Tiện";
			worksheet.Cell(currRow, 12).Value = "Loại Hàng Hóa";
			worksheet.Cell(currRow, 13).Value = "Đơn Vị Tính";
			worksheet.Cell(currRow, 14).Value = "Ngày Áp Dụng";

			foreach (var row in data)
			{
				currRow++;
				worksheet.Cell(currRow, 1).Value = row.MaHopDong;
				worksheet.Cell(currRow, 2).Value = row.SoHopDongCha;
				worksheet.Cell(currRow, 3).Value = row.TenKH;
				worksheet.Cell(currRow, 4).Value = row.AccountName;
				worksheet.Cell(currRow, 5).Value = row.DiemDau;
				worksheet.Cell(currRow, 6).Value = row.DiemCuoi;
				worksheet.Cell(currRow, 7).Value = row.DiemLayTraRong;
				worksheet.Cell(currRow, 8).Value = row.DonGia;
				worksheet.Cell(currRow, 9).Value = row.LoaiTienTe;
				worksheet.Cell(currRow, 10).Value = row.MaPTVC;
				worksheet.Cell(currRow, 11).Value = row.MaLoaiPhuongTien;
				worksheet.Cell(currRow, 12).Value = row.MaLoaiHangHoa;
				worksheet.Cell(currRow, 13).Value = row.MaDVT;
				worksheet.Cell(currRow, 14).Value = row.NgayApDung.ToString("dd-MM-yyyy HH:mm:ss");
			}

			worksheet.Range("A1:N" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Dashed;
			worksheet.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			string excelName = $"BangGia " + DateTime.Now.ToString("dd-MM-yyyy");

			//return File(stream, "application/octet-stream", excelName);
			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportExcelTemplatePriceTable()
		{
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("TemplatePriceTable");
			var worksheet1 = workbook.Worksheets.Add("MasterData");

			worksheet.Range("A1:K1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:K1").Style.Fill.BackgroundColor = XLColor.Red;
			worksheet.Range("A1:K1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(1, 1).Value = "MaKH";
			worksheet.Cell(1, 2).Value = "Account";
			worksheet.Cell(1, 2).Style.Fill.BackgroundColor = XLColor.White;
			worksheet.Cell(1, 3).Value = "MaHopDong";
			worksheet.Cell(1, 4).Value = "DiemDau";
			worksheet.Cell(1, 5).Value = "DiemCuoi";
			worksheet.Cell(1, 6).Value = "DiemLayTraRong";
			worksheet.Cell(1, 6).Style.Fill.BackgroundColor = XLColor.White;
			worksheet.Cell(1, 7).Value = "DonGiaVnd";
			worksheet.Cell(1, 8).Value = "LoaiTienTe";
			worksheet.Cell(1, 9).Value = "MaPtvc";
			worksheet.Cell(1, 10).Value = "MaLoaiPhuongTien";
			worksheet.Cell(1, 11).Value = "MaLoaiHangHoa";

			//name
			worksheet.Cell(2, 1).Value = "Mã Khách Hàng";
			worksheet.Cell(2, 2).Value = "Mã Account";
			worksheet.Cell(2, 3).Value = "Mã Hợp Đồng";
			worksheet.Cell(2, 4).Value = "Điểm Đóng Hàng";
			worksheet.Cell(2, 5).Value = "Điểm Hạ Hàng";
			worksheet.Cell(2, 6).Value = "Điểm Lấy/Trả Rỗng";
			worksheet.Cell(2, 7).Value = "Đơn Giá";
			worksheet.Cell(2, 8).Value = "Loại Tiền Tệ";
			worksheet.Cell(2, 9).Value = "Mã Phương Thức Vận Chuyển";
			worksheet.Cell(2, 10).Value = "Mã Loại Phương Tiện";
			worksheet.Cell(2, 11).Value = "Mã Loại Hàng Hóa";

			worksheet.Range("A1:K2").Style.Border.TopBorder = XLBorderStyleValues.Dotted;
			worksheet.Columns().AdjustToContents();

			var panigation = new PaginationFilter();
			panigation.PageNumber = 1;
			panigation.PageSize = 10000;

			var getData = await _address.GetListAddress(panigation);
			var currRow = 1;

			worksheet1.Range("A1:C1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("A1:C1").Style.Font.FontSize = 15;
			worksheet1.Range("A1:C1").Style.Font.Bold = true;
			worksheet1.Range("A1:C1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("A1:C1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(currRow, 3).Value = "Mã Địa Điểm";
			worksheet1.Cell(currRow, 2).Value = "Tên Địa Điểm";
			worksheet1.Cell(currRow, 1).Value = "Thuộc Khu Vực";


			foreach (var row in getData.dataResponse)
			{
				currRow++;
				worksheet1.Cell(currRow, 2).Value = row.TenDiaDiem;
				worksheet1.Cell(currRow, 1).Value = row.KhuVuc;
				worksheet1.Cell(currRow, 3).Value = row.MaDiaDiem;
			}
			worksheet1.Range("A1:C" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("E1:F1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("E1:F1").Style.Font.FontSize = 15;
			worksheet1.Range("E1:F1").Style.Font.Bold = true;
			worksheet1.Range("E1:F1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("E1:F1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 5).Value = "Tên Tiền Tệ";
			worksheet1.Cell(1, 6).Value = "Mã Tiền Tệ";

			var getPriceType = await _context.LoaiTienTe.ToListAsync();

			int currRowPrice = 1;

			foreach (var item in getPriceType)
			{
				currRowPrice++;
				worksheet1.Cell(currRowPrice, 5).Value = item.TenLoaiTienTe;
				worksheet1.Cell(currRowPrice, 6).Value = item.MaLoaiTienTe;
			}
			worksheet1.Range("E1:F" + currRowPrice).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("H1:I1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("H1:I1").Style.Font.FontSize = 15;
			worksheet1.Range("H1:I1").Style.Font.Bold = true;
			worksheet1.Range("H1:I1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("H1:I1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 8).Value = "Tên PTVC";
			worksheet1.Cell(1, 9).Value = "Mã PTVC";
			var getTransportType = await _context.PhuongThucVanChuyen.ToListAsync();

			int currRowTransportType = 1;
			foreach (var item in getTransportType)
			{
				currRowTransportType++;
				worksheet1.Cell(currRowTransportType, 8).Value = item.TenPtvc;
				worksheet1.Cell(currRowTransportType, 9).Value = item.MaPtvc;
			}
			worksheet1.Range("H1:I" + currRowTransportType).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("K1:M1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("K1:M1").Style.Font.FontSize = 15;
			worksheet1.Range("K1:M1").Style.Font.Bold = true;
			worksheet1.Range("K1:M1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("K1:M1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 11).Value = "Loại PT";
			worksheet1.Cell(1, 12).Value = "Tên PT";
			worksheet1.Cell(1, 13).Value = "Mã PT";
			var getVehicleType = await _context.LoaiPhuongTien.ToListAsync();

			int currRowVehicleType = 1;
			foreach (var item in getVehicleType)
			{
				currRowVehicleType++;
				worksheet1.Cell(currRowVehicleType, 11).Value = item.PhanLoai;
				worksheet1.Cell(currRowVehicleType, 12).Value = item.TenLoaiPhuongTien;
				worksheet1.Cell(currRowVehicleType, 13).Value = item.MaLoaiPhuongTien;
			}
			worksheet1.Range("K1:M" + currRowVehicleType).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("O1:P1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("O1:P1").Style.Font.FontSize = 15;
			worksheet1.Range("O1:P1").Style.Font.Bold = true;
			worksheet1.Range("O1:P1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("O1:P1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 15).Value = "Tên Hàng Hóa";
			worksheet1.Cell(1, 16).Value = "Mã Hàng Hóa";
			var getGoodsType = await _context.LoaiHangHoa.ToListAsync();

			int currRowGoodsType = 1;
			foreach (var item in getGoodsType)
			{
				currRowGoodsType++;
				worksheet1.Cell(currRowGoodsType, 15).Value = item.TenLoaiHangHoa;
				worksheet1.Cell(currRowGoodsType, 16).Value = item.MaLoaiHangHoa;
			}
			worksheet1.Range("O1:P" + currRowGoodsType).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();
			string excelName = $"TemplatePricetable " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
		}
	}
}