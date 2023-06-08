using ClosedXML.Excel;
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
		private readonly ICommon _common;
		private readonly TMSContext _context;

		public PriceTableController(IPriceTable priceTable, IPaginationService paninationService, ICommon common, TMSContext context)
		{
			_priceTable = priceTable;
			_paninationService = paninationService;
			_common = common;
			_context = context;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
		{
			var checkPermission = await _common.CheckPermission("C0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var create = await _priceTable.CreatePriceTable(request);

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
			var checkPermission = await _common.CheckPermission("C0002");
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
			var ImportExcel = await _priceTable.CreatePriceByExcel(formFile, cancellationToken);

			if (ImportExcel.isSuccess == true)
			{
				return Ok(ImportExcel.Message);
			}
			else
			{
				return BadRequest(ImportExcel.Message);
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
				worksheet.Cell(currRow, 14).Value = row.NgayApDung.ToString("dd-MM-yyyy HH:ss");
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
	}
}