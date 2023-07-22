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
using TBSLogistics.Model.Model.SubfeeLoloModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.LoloSubfeeManager;

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class SubfeeLoloController : ControllerBase
	{
		private readonly ILoloSubfee _loloSubfee;
		private readonly IAddress _address;
		private readonly IPaginationService _uriService;
		private readonly ICommon _common;
		private readonly TMSContext _context;

		public SubfeeLoloController(ILoloSubfee loloSubfee, IPaginationService uriService, ICommon common, TMSContext context, IAddress address)
		{
			_common = common;
			_context = context;
			_loloSubfee = loloSubfee;
			_uriService = uriService;
			_address = address;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateSubfeeLolo(List<CreateOrUpdateSubfeeLolo> request)
		{
			var checkPermission = await _common.CheckPermission("F0009");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}


			var create = await _loloSubfee.CreateSubfeeLolo(request, false);

			if (create.isSuccess)
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
		public async Task<IActionResult> UpdateSubfeeLolo(int id, CreateOrUpdateSubfeeLolo request)
		{
			var checkPermission = await _common.CheckPermission("F0009");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}


			var update = await _loloSubfee.UpdateSubfeeLolo(id, request);

			if (update.isSuccess)
			{
				return Ok(update.Message);
			}
			else
			{
				return BadRequest(update.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetSubfeeLoloById(int id)
		{
			var getById = await _loloSubfee.GetSubfeeLoloById(id);

			return Ok(getById);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListSubfeeLolo([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("F0008");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _loloSubfee.GetListSubfeeLolo(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListSubfeeLolo>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ApproveSubfeeLolo(ApprovePriceTable request)
		{
			var checkPermission = await _common.CheckPermission("F0010");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var approve = await _loloSubfee.ApproveSubfeeLolo(request);

			if (approve.isSuccess == true)
			{
				return Ok(approve.Message);
			}
			else
			{
				return BadRequest(approve.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ReadFileExcelLoloSubfee(IFormFile formFile, CancellationToken cancellationToken)
		{
			var checkPermission = await _common.CheckPermission("F0009");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var create = await _loloSubfee.CreateSubfeeLoloByExcel(formFile, cancellationToken);

			if (create.isSuccess == true)
			{
				return Ok(create.Message);
			}
			else
			{
				return BadRequest(create.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportExcelLoloSubfee([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("F0008");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var data = await _loloSubfee.GetListSubfeeLoloExportExcel(filter);

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("PhuPhiLoLo");

			var currRow = 1;
			worksheet.Range("A1:H1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:H1").Style.Font.FontSize = 15;
			worksheet.Range("A1:H1").Style.Font.Bold = true;
			worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet.Range("A1:H1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(currRow, 1).Value = "Tên Địa Điểm";
			worksheet.Cell(currRow, 2).Value = "Hãng Tàu";
			worksheet.Cell(currRow, 3).Value = "Loại Phụ Phí";
			worksheet.Cell(currRow, 4).Value = "Loại Container";
			worksheet.Cell(currRow, 5).Value = "Tên Khách Hàng";
			worksheet.Cell(currRow, 6).Value = "Đơn Giá";
			worksheet.Cell(currRow, 7).Value = "Trạng Thái";
			worksheet.Cell(currRow, 8).Value = "Thời Gian Tạo";

			foreach (var row in data)
			{
				currRow++;
				worksheet.Cell(currRow, 1).Value = row.TenDiaDiem;
				worksheet.Cell(currRow, 2).Value = row.HangTau;
				worksheet.Cell(currRow, 3).Value = row.LoaiPhuPhi;
				worksheet.Cell(currRow, 4).Value = row.LoaiCont;
				worksheet.Cell(currRow, 5).Value = row.TenKh;
				worksheet.Cell(currRow, 6).Value = row.DonGia;
				worksheet.Cell(currRow, 7).Value = row.TenTrangThai;
				worksheet.Cell(currRow, 8).Value = row.Createdtime.ToString("dd-MM-yyyy");
			}

			worksheet.Range("A1:H" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Dashed;
			worksheet.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			string excelName = $"PhuPhiLolo " + DateTime.Now.ToString("dd-MM-yyyy");

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportTemplateExcel()
		{
			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("TemplateSubfeeLolo");
			var worksheet1 = workbook.Worksheets.Add("MasterData");

			worksheet.Range("A1:F1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:F1").Style.Fill.BackgroundColor = XLColor.Red;
			worksheet.Range("A1:F1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(1, 1).Value = "MaKH";
			worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.White;
			worksheet.Cell(1, 2).Value = "MaDiaDiem";
			worksheet.Cell(1, 3).Value = "HangTau";
			worksheet.Cell(1, 3).Style.Fill.BackgroundColor = XLColor.White;
			worksheet.Cell(1, 4).Value = "LoaiCont";
			worksheet.Cell(1, 5).Value = "LoaiPhuPhi";
			worksheet.Cell(1, 6).Value = "DonGia";

			//name
			worksheet.Cell(2, 1).Value = "Mã Khách Hàng";
			worksheet.Cell(2, 2).Value = "Mã Địa Điểm";
			worksheet.Cell(2, 3).Value = "Hãng Tàu";
			worksheet.Cell(2, 4).Value = "Loại Container";
			worksheet.Cell(2, 5).Value = "Loại Phụ Phí";
			worksheet.Cell(2, 6).Value = "Đơn Giá";


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
			worksheet1.Cell(1, 5).Value = "Mã Khách Hàng";
			worksheet1.Cell(1, 6).Value = "Tên Khách Hàng";
			var getCustomer = await _context.KhachHang.Where(x => x.MaLoaiKh == "KH").ToListAsync();
			int currRowCustomer = 1;
			foreach (var item in getCustomer)
			{
				currRowCustomer++;
				worksheet1.Cell(currRowCustomer, 5).Value = item.MaKh;
				worksheet1.Cell(currRowCustomer, 6).Value = item.TenKh;

			}
			worksheet1.Range("E1:F" + currRowCustomer).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("H1:J1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("H1:J1").Style.Font.FontSize = 15;
			worksheet1.Range("H1:J1").Style.Font.Bold = true;
			worksheet1.Range("H1:J1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("H1:J1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 8).Value = "Loại PT";
			worksheet1.Cell(1, 9).Value = "Tên PT";
			worksheet1.Cell(1, 10).Value = "Mã PT";
			var getVehicleType = await _context.LoaiPhuongTien.ToListAsync();

			int currRowVehicleType = 1;
			foreach (var item in getVehicleType)
			{
				currRowVehicleType++;
				worksheet1.Cell(currRowVehicleType, 8).Value = item.PhanLoai;
				worksheet1.Cell(currRowVehicleType, 9).Value = item.TenLoaiPhuongTien;
				worksheet1.Cell(currRowVehicleType, 10).Value = item.MaLoaiPhuongTien;
			}
			worksheet1.Range("H1:J" + currRowVehicleType).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("L1:M1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("L1:M1").Style.Font.FontSize = 15;
			worksheet1.Range("L1:M1").Style.Font.Bold = true;
			worksheet1.Range("L1:M1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("L1:M1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 12).Value = "Mã Hãng Tàu";
			worksheet1.Cell(1, 13).Value = "Tên Hãng Tàu";
			var getShipList = await _context.ShippingInfomation.ToListAsync();
			int currRowShip = 1;
			foreach (var item in getShipList)
			{
				currRowShip++;
				worksheet1.Cell(currRowShip, 12).Value = item.ShippingCode;
				worksheet1.Cell(currRowShip, 13).Value = item.ShippingLineName;
			}
			worksheet1.Range("L1:M" + currRowShip).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Range("O1:P1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("O1:P1").Style.Font.FontSize = 15;
			worksheet1.Range("O1:P1").Style.Font.Bold = true;
			worksheet1.Range("O1:P1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("O1:P1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
			worksheet1.Cell(1, 15).Value = "Mã Loại Phụ Phí";
			worksheet1.Cell(1, 16).Value = "Tên Loại Phụ Phí";
			worksheet1.Cell(2, 15).Value = "1";
			worksheet1.Cell(2, 16).Value = "Nâng";
			worksheet1.Cell(3, 15).Value = "2";
			worksheet1.Cell(3, 16).Value = "Hạ";
			worksheet1.Range("O1:P3" ).Style.Border.TopBorder = XLBorderStyleValues.Thin;

			worksheet1.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();
			string excelName = $"TemplatePricetable " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
		}
	}
}