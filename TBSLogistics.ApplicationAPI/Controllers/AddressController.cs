using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AddressController : ControllerBase
	{
		private readonly IAddress _address;
		private readonly IPaginationService _uriService;
		private readonly ICommon _common;

		public AddressController(IAddress address, IPaginationService uriService, ICommon common)
		{
			_common = common;
			_address = address;
			_uriService = uriService;
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateAddress(CreateAddressRequest request)
		{
			var checkPermission = await _common.CheckPermission("L0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var CreateAddress = await _address.CreateAddress(request);

			if (CreateAddress.isSuccess == true)
			{
				return Ok(CreateAddress.Message);
			}
			else
			{
				return BadRequest(CreateAddress.Message);
			}
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> EditAddress(int Id, UpdateAddressRequest request)
		{
			var checkPermission = await _common.CheckPermission("L0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var EditAddress = await _address.EditAddress(Id, request);

			if (EditAddress.isSuccess == true)
			{
				return Ok(EditAddress.Message);
			}
			else
			{
				return BadRequest(EditAddress.Message);
			}
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetAddressById(int Id)
		{
			var checkPermission = await _common.CheckPermission("L0003");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var address = await _address.GetAddressById(Id);
			return Ok(address);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListAddress([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("L0002");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _address.GetListAddress(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<GetAddressModel>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListAddressType()
		{
			var list = await _address.GetListTypeAddress();
			return Ok(list);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
		{
			//var ImportExcel = await _address.ReadExcelFile(formFile, cancellationToken);

			//if (ImportExcel.isSuccess == true)
			//{
			//    return Ok(ImportExcel.Message);
			//}
			//else
			//{
			//    return BadRequest(ImportExcel.DataReturn + " --- " + ImportExcel.Message);
			//}

			return Ok();
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListProvinces()
		{
			var list = await _address.GetProvinces();

			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListDistricts(int ProvinceId)
		{
			var list = await _address.GetDistricts(ProvinceId);
			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListWards(int DistrictId)
		{
			var list = await _address.GetWards(DistrictId);
			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListAddressSelect(string pointType, string type)
		{
			var list = await _address.GetListAddressSelect(pointType, type);

			return Ok(list);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportExcelAddress()
		{
			var panigation = new PaginationFilter();
			panigation.PageNumber = 1;
			panigation.PageSize = 10000;

			var getData = await _address.GetListAddress(panigation);

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("DiaDiem");

			var currRow = 1;

			worksheet.Range("A1:C1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:C1").Style.Font.FontSize = 15;
			worksheet.Range("A1:C1").Style.Font.Bold = true;
			worksheet.Range("A1:C1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet.Range("A1:C1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(currRow, 1).Value = "Mã Địa Điểm";
			worksheet.Cell(currRow, 2).Value = "Tên Địa Điểm";
			worksheet.Cell(currRow, 3).Value = "Thuộc Khu Vực";

			foreach (var row in getData.dataResponse)
			{
				currRow++;
				worksheet.Cell(currRow, 1).Value = row.MaDiaDiem;
				worksheet.Cell(currRow, 2).Value = row.TenDiaDiem;
				worksheet.Cell(currRow, 3).Value = row.KhuVuc;
			}

			worksheet.Range("A1:C" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DiaDiemFileExcel");
		}

		//[HttpPost]
		//[Route("[action]")]
		//[AllowAnonymous]
		//public async Task<IActionResult> CreateProvince(int matinh, string tentinh, string phanloai)
		//{
		//    var add = await _address.CreateProvince(matinh, tentinh, phanloai);
		//    return Ok();
		//}

		//[HttpPost]
		//[Route("[action]")]
		//[AllowAnonymous]
		//public async Task<IActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode)
		//{
		//    var add = await _address.CreateDistricts(mahuyen, tenhuyen, phanloai, parentcode);
		//    return Ok();
		//}

		//[HttpPost]
		//[Route("[action]")]
		//[AllowAnonymous]
		//public async Task<JsonResult> CreateWard(string jsonResult)
		//{
		//    List<WardModel> wardModels = JsonConvert.DeserializeObject<List<WardModel>>(jsonResult);

		//    var add = await _address.CreateWard(wardModels);
		//    return new JsonResult("OK");
		//}
	}
}