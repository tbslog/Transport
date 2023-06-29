using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.CustommerManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CustomerController : ControllerBase
	{
		private ICustomer _customer;
		private IPaginationService _uriService;
		private ICommon _common;

		public CustomerController(ICustomer customer, IPaginationService uriService, ICommon common)
		{
			_customer = customer;
			_uriService = uriService;
			_common = common;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
		{
			var checkPermission = await _common.CheckPermission("A0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var Create = await _customer.CreateCustomer(request);

			if (Create.isSuccess == true)
			{
				return Ok(Create.Message);
			}
			else
			{
				return BadRequest(Create.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> UpdateCustomer(string Id, EditCustomerRequest request)
		{
			var checkPermission = await _common.CheckPermission("A0004");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var Edit = await _customer.EditCustomer(Id, request);

			if (Edit.isSuccess == true)
			{
				return Ok(Edit.Message);
			}
			else
			{
				return BadRequest(Edit.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListCustomer([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("A0002");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _customer.getListCustommer(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustommerRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListCustomerOptionSelect(string type = null)
		{
			var listOptionSelect = await _customer.getListCustomerOptionSelect(type);
			return Ok(listOptionSelect);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetCustomerById(string Id)
		{
			var checkPermission = await _common.CheckPermission("A0004");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var Custommer = await _customer.GetCustomerById(Id);
			return Ok(Custommer);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListChuoiSelect()
		{
			var checkPermission = await _common.CheckPermission("A0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var list = await _customer.GetListChuoiSelect();

			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListCustomerFilter(string type = null)
		{
			var listOptionSelect = await _customer.GetListCustomerFilter(type);
			return Ok(listOptionSelect);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetExcelCustomer()
		{
			var panigation = new PaginationFilter();
			panigation.PageNumber = 1;
			panigation.PageSize = 10000;

			var getData = await _customer.getListCustommer(panigation);

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("KhachHang");
			var currRow = 1;

			worksheet.Range("A1:H1").Style.Border.DiagonalBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:H1").Style.Font.FontSize = 15;
			worksheet.Range("A1:H1").Style.Font.Bold = true;
			worksheet.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet.Range("A1:H1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(currRow, 1).Value = "Chuỗi";
			worksheet.Cell(currRow, 2).Value = "Mã Pháp Nhân";
			worksheet.Cell(currRow, 3).Value = "Tên Pháp Nhân";
			worksheet.Cell(currRow, 4).Value = "Tên Rút Gọn";
			worksheet.Cell(currRow, 5).Value = "Phân Loại";
			worksheet.Cell(currRow, 6).Value = "Mã Số Thuế";
			worksheet.Cell(currRow, 7).Value = "Số Điện Thoại";
			worksheet.Cell(currRow, 8).Value = "Địa Chỉ Email";

			foreach (var row in getData.dataResponse.OrderBy(x => x.Chuoi).Where(x => x.LoaiKH == "KH"))
			{
				currRow++;
				worksheet.Cell(currRow, 1).Value = row.Chuoi;
				worksheet.Cell(currRow, 2).Value = row.MaKh;
				worksheet.Cell(currRow, 3).Value = row.TenKh;
				worksheet.Cell(currRow, 4).Value = row.TenTomTat;
				worksheet.Cell(currRow, 5).Value = "Khách Hàng";
				worksheet.Cell(currRow, 6).Value = row.MaSoThue;
				worksheet.Cell(currRow, 7).Value = row.Sdt;
				worksheet.Cell(currRow, 8).Value = row.Email;
			}
			worksheet.Range("A1:H" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Columns().AdjustToContents();

			var worksheet1 = workbook.Worksheets.Add("NhaCungCap");
			var currRow1 = 1;
			worksheet1.Range("A1:H1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Range("A1:H1").Style.Font.FontSize = 15;
			worksheet1.Range("A1:H1").Style.Font.Bold = true;
			worksheet1.Range("A1:H1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet1.Range("A1:H1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet1.Cell(currRow1, 1).Value = "Chuỗi";
			worksheet1.Cell(currRow1, 2).Value = "Mã Pháp Nhân";
			worksheet1.Cell(currRow1, 3).Value = "Tên Pháp Nhân";
			worksheet1.Cell(currRow1, 4).Value = "Tên Rút Gọn";
			worksheet1.Cell(currRow1, 5).Value = "Phân Loại";
			worksheet1.Cell(currRow1, 6).Value = "Mã Số Thuế";
			worksheet1.Cell(currRow1, 7).Value = "Số Điện Thoại";
			worksheet1.Cell(currRow1, 8).Value = "Địa Chỉ Email";

			foreach (var row in getData.dataResponse.Where(x => x.LoaiKH == "NCC"))
			{
				currRow1++;
				worksheet1.Cell(currRow1, 1).Value = row.Chuoi;
				worksheet1.Cell(currRow1, 2).Value = row.MaKh;
				worksheet1.Cell(currRow1, 3).Value = row.TenKh;
				worksheet1.Cell(currRow1, 4).Value = row.TenTomTat;
				worksheet1.Cell(currRow1, 5).Value = "Nhà Cung Cấp";
				worksheet1.Cell(currRow1, 6).Value = row.MaSoThue;
				worksheet1.Cell(currRow1, 7).Value = row.Sdt;
				worksheet1.Cell(currRow1, 8).Value = row.Email;
			}
			worksheet1.Range("A1:H" + currRow1).Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet1.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelParner");
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
		{
			//var ImportExcel = await _customer.ReadExcelFile(formFile, cancellationToken);

			//if (ImportExcel.isSuccess == true)
			//{
			//    return Ok(ImportExcel.Message);
			//}
			//else
			//{
			//    return BadRequest(ImportExcel.DataReturn + " --- " + ImportExcel.Message);
			//}
			return BadRequest();
		}
	}
}