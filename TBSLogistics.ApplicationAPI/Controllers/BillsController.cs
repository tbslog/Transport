using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Bill;
using TBSLogistics.Service.Services.BillOfLadingManage;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.CustommerManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {

        private readonly IBill _bill;
        private readonly IPaginationService _uriService;
        private readonly ICommon _common;

        public BillsController(IBill bill, IPaginationService uriService, ICommon common)
        {
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
        public async Task<IActionResult> GetBillByCustomerId(string customerId, int ky)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var billResult = await _bill.GetBillByCustomerId(customerId, ky);

            return Ok(billResult);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillByTransportId(string customerId, string transportId)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var data = await _bill.GetBillByTransportId(customerId, transportId);
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
            await Task.Yield();
            var stream = new MemoryStream();
            filter.PageNumber = 1;
            filter.PageSize = 500000;
            var data = await _bill.GetListBillHandling(filter);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DieuPhoi");
                workSheet.Cells[1, 1].Value = "Mã Vận Đơn";
                workSheet.Cells[1, 2].Value = "Booking No";
                workSheet.Cells[1, 3].Value = "Ngày CUT OFF ";
                workSheet.Cells[1, 4].Value = "Loại Vận Đơn";
                workSheet.Cells[1, 5].Value = "Loại Hàng Hóa";
                workSheet.Cells[1, 6].Value = "Loại Phương Tiện";
                workSheet.Cells[1, 7].Value = "Phương Thức Vận Chuyển";
                workSheet.Cells[1, 8].Value = "Khách Hàng";
                workSheet.Cells[1, 9].Value = "Account";
                workSheet.Cells[1, 10].Value = "Đơn Vị Vận Tải";
                workSheet.Cells[1, 11].Value = "Điểm Đóng Hàng";
                workSheet.Cells[1, 12].Value = "Điểm Hạ Hàng";
                workSheet.Cells[1, 13].Value = "Điểm Lấy Rỗng";
                workSheet.Cells[1, 14].Value = "Điểm Trả Rỗng";
                workSheet.Cells[1, 15].Value = "Đơn Giá Khách Hàng";
                workSheet.Cells[1, 16].Value = "Đơn Giá Nhà Cung Cấp";
                workSheet.Cells[1, 17].Value = "Doanh Thu";
                workSheet.Cells[1, 18].Value = "Lợi Nhuận";
                workSheet.Cells[1, 19].Value = "Phụ Phí Hợp Đồng";
                workSheet.Cells[1, 20].Value = "Phụ Phí Phát Sinh";

                int row = 2;
                foreach (var item in data.dataResponse)
                {
                    workSheet.Cells[row, 1].Value = item.MaVanDon;
                    workSheet.Cells[row, 2].Value = item.MaVanDonKH;
                    workSheet.Cells[row, 3].Value = item.CutOffDate;
                    workSheet.Cells[row, 4].Value = item.LoaiVanDon == "nhap" ? "Nhập" : "Xuất";
                    workSheet.Cells[row, 5].Value = item.LoaiHangHoa;
                    workSheet.Cells[row, 6].Value = item.LoaiPhuongTien;
                    workSheet.Cells[row, 7].Value = item.MaPTVC;
                    workSheet.Cells[row, 8].Value = item.TenKH;
                    workSheet.Cells[row, 9].Value = item.AccountName;
                    workSheet.Cells[row, 10].Value = item.TenNCC;
                    workSheet.Cells[row, 11].Value = item.DiemDau;
                    workSheet.Cells[row, 12].Value = item.DiemCuoi;
                    workSheet.Cells[row, 13].Value = item.DiemLayRong;
                    workSheet.Cells[row, 14].Value = item.DiemTraRong;
                    workSheet.Cells[row, 15].Value = item.DonGiaKH;
                    workSheet.Cells[row, 16].Value = item.DonGiaNCC;
                    workSheet.Cells[row, 17].Value = item.DoanhThu;
                    workSheet.Cells[row, 18].Value = item.LoiNhuan;
                    workSheet.Cells[row, 19].Value = item.ChiPhiHopDong;
                    workSheet.Cells[row, 20].Value = item.ChiPhiPhatSinh;
                    row++;
                }


                workSheet.Cells["C2:C" + row ].Style.Numberformat.Format = "DD-MM-YYYY HH:mm";
                workSheet.Cells["A1:S1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1:S1"].Style.Font.Bold = true;
                workSheet.Cells["A1:S1"].Style.Font.Size = 14;

                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                package.Save();
            }
            stream.Position = 0;
            string excelName = $"HoaDon " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}
