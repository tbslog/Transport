using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Bill;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class BillsController : ControllerBase
	{
		private readonly IBill _bill;
		private readonly IPriceTable _priceTable;
		private readonly IPaginationService _uriService;
		private readonly ICommon _common;

		public BillsController(IBill bill, IPaginationService uriService, ICommon common, IPriceTable priceTable)
		{
			_priceTable = priceTable;
			_common = common;
			_uriService = uriService;
			_bill = bill;
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListTransportByCustomerId(string customerId, int ky, [FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _bill.GetListTransportByCustomerId(customerId, ky, filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListVanDon>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetBillByCustomerId(string customerId, DateTime datePay)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}
			var billResult = await _bill.GetBillByCustomerId(customerId, datePay);

			return Ok(billResult);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetBillByTransportId(string transportId, long? handlingId = null)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var data = await _bill.GetBillByTransportId(transportId, handlingId);
			return Ok(data);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListBillHandling([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _bill.GetListBillHandling(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ListBillHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ListBillByTransport([FromQuery] PaginationFilter filter)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}

			var route = Request.Path.Value;
			var pagedData = await _bill.GetListBillWeb(filter);
			var pagedReponse = PaginationHelper.CreatePagedReponse<ListBillTransportWeb>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route); ;
			return Ok(pagedReponse);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListKy(string customerId)
		{
			var checkPermission = await _common.CheckPermission("G0001");
			if (checkPermission.isSuccess == false)
			{
				return BadRequest(checkPermission.Message);
			}
			var data = await _bill.GetListKyThanhToan(customerId);

			return Ok(data);
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> ExportExcelBill([FromQuery] PaginationFilter filter)
		{
			filter.PageNumber = 1;
			filter.PageSize = 500000;
			var data = await _bill.GetListBillHandling(filter);

			var workbook = new XLWorkbook();
			var worksheet = workbook.Worksheets.Add("HoaDon");

			var currRow = 1;
			worksheet.Range("A1:AA1").Style.Border.TopBorder = XLBorderStyleValues.Thin;
			worksheet.Range("A1:AA1").Style.Font.FontSize = 15;
			worksheet.Range("A1:AA1").Style.Font.Bold = true;
			worksheet.Range("A1:AA1").Style.Fill.BackgroundColor = XLColor.LightGray;
			worksheet.Range("A1:AA1").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

			worksheet.Cell(1, 1).Value = "Booking No";
			worksheet.Cell(1, 2).Value = "CONT NO";
			worksheet.Cell(1, 3).Value = "Reuse CONT";
			worksheet.Cell(1, 4).Value = "Thời Gian Tạo";
			worksheet.Cell(1, 5).Value = "Ngày CUT OFF";
			worksheet.Cell(1, 6).Value = "Thời Gian Hoàn Thành";
			worksheet.Cell(1, 7).Value = "Loại Vận Đơn";
			worksheet.Cell(1, 8).Value = "Loại Hàng Hóa";
			worksheet.Cell(1, 9).Value = "Loại Phương Tiện";
			worksheet.Cell(1, 10).Value = "Phương Thức Vận Chuyển";
			worksheet.Cell(1, 11).Value = "Khách Hàng";
			worksheet.Cell(1, 12).Value = "Account";
			worksheet.Cell(1, 13).Value = "Đơn Vị Vận Tải";
			worksheet.Cell(1, 14).Value = "Điểm Đóng Hàng";
			worksheet.Cell(1, 15).Value = "Điểm Hạ Hàng";
			worksheet.Cell(1, 16).Value = "Điểm Lấy Rỗng";
			worksheet.Cell(1, 17).Value = "Điểm Trả Rỗng";
			worksheet.Cell(1, 18).Value = "Đơn Giá Khách Hàng";
			worksheet.Cell(1, 19).Value = "Đơn Vị Tiền Tệ";
			worksheet.Cell(1, 20).Value = "Đơn Giá Quy Đổi";
			worksheet.Cell(1, 21).Value = "Đơn Giá Nhà Cung Cấp";
			worksheet.Cell(1, 22).Value = "Đơn Vị Tiền Tệ";
			worksheet.Cell(1, 23).Value = "Đơn Giá Quy Đổi";
			worksheet.Cell(1, 24).Value = "Doanh Thu";
			worksheet.Cell(1, 25).Value = "Phụ Phí Hợp Đồng";
			worksheet.Cell(1, 26).Value = "Phụ Phí Phát Sinh";
			worksheet.Cell(1, 27).Value = "Lợi Nhuận";

			int row = 2;
			foreach (var item in data.dataResponse)
			{
				worksheet.Cell(row, 1).Value = item.MaVanDonKH;
				worksheet.Cell(row, 2).Value = item.ContNo;
				worksheet.Cell(row, 3).Value = item.Reuse;
				worksheet.Cell(row, 4).Value = item.createdTime;
				worksheet.Cell(row, 5).Value = item.CutOffDate;
				worksheet.Cell(row, 6).Value = item.ThoiGianHoanThanh;
				worksheet.Cell(row, 7).Value = item.LoaiVanDon == "nhap" ? "Nhập" : "Xuất";
				worksheet.Cell(row, 8).Value = item.LoaiHangHoa;
				worksheet.Cell(row, 9).Value = item.LoaiPhuongTien;
				worksheet.Cell(row, 10).Value = item.MaPTVC;
				worksheet.Cell(row, 11).Value = item.TenKH;
				worksheet.Cell(row, 12).Value = item.AccountName;
				worksheet.Cell(row, 13).Value = item.TenNCC;
				worksheet.Cell(row, 14).Value = item.DiemDau;
				worksheet.Cell(row, 15).Value = item.DiemCuoi;
				worksheet.Cell(row, 16).Value = item.DiemLayRong;
				worksheet.Cell(row, 17).Value = item.DiemTraRong;
				worksheet.Cell(row, 18).Value = item.DonGiaKH;
				worksheet.Cell(row, 19).Value = item.LoaiTienTeKH;
				worksheet.Cell(row, 20).Value = item.DonGiaKH * (decimal)await _priceTable.GetPriceTradeNow(item.LoaiTienTeKH);
				worksheet.Cell(row, 21).Value = item.DonGiaNCC;
				worksheet.Cell(row, 22).Value = item.LoaiTienTeNCC;
				worksheet.Cell(row, 23).Value = item.DonGiaNCC * (decimal)await _priceTable.GetPriceTradeNow(item.LoaiTienTeNCC); ;
				worksheet.Cell(row, 24).Value = item.DoanhThu;
				worksheet.Cell(row, 25).Value = item.ChiPhiHopDong;
				worksheet.Cell(row, 26).Value = item.ChiPhiPhatSinh;
				worksheet.Cell(row, 27).Value = item.LoiNhuan;
				row++;
			}

			worksheet.Range("A1:AA" + currRow).Style.Border.TopBorder = XLBorderStyleValues.Dashed;
			worksheet.Range("D2:F" + row).Style.DateFormat.Format = "DD-MM-YYYY HH:mm";
			worksheet.Columns().AdjustToContents();

			using var stream = new MemoryStream();
			workbook.SaveAs(stream);
			var content = stream.ToArray();

			string excelName = $"HoaDon " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

			return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
		}
	}
}