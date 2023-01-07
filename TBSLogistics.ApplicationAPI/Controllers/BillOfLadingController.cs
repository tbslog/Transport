using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.BillOfLadingManage;
using TBSLogistics.Service.Services.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfLadingController : ControllerBase
    {
        private readonly IBillOfLading _billOfLading;
        private readonly IPaginationService _paninationService;
        private readonly ICommon _common;

        public BillOfLadingController(IBillOfLading billOfLading, IPaginationService paginationService, ICommon common)
        {
            _billOfLading = billOfLading;
            _paninationService = paginationService;
            _common = common;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataHandling()
        {
            var data = await _billOfLading.LoadDataHandling();
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTransportById(string transportId)
        {
            var checkPermission = await _common.CheckPermission("E0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var data = await _billOfLading.GetTransportById(transportId);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransport([FromQuery] PaginationFilter filter, string[] customers)
        {
            var checkPermission = await _common.CheckPermission("E0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListTransport(customers, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListTransport>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListHandling([FromQuery] PaginationFilter filter, string[] customers, string transportId = null)
        {
            var checkPermission = await _common.CheckPermission("F0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListHandling(transportId, customers, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListHandlingLess([FromQuery] PaginationFilter filter, string[] customers)
        {
            var checkPermission = await _common.CheckPermission("F0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListHandlingLess(customers, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateTransport(CreateTransport request)
        {
            var checkPermission = await _common.CheckPermission("E0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var create = await _billOfLading.CreateTransport(request);

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
        public async Task<IActionResult> UpdateTransport(string transportId, UpdateTransport request)
        {
            var checkPermission = await _common.CheckPermission("E0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.UpdateTransport(transportId, request);

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
        public async Task<IActionResult> GetHandlingById(int id)
        {
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var data = await _billOfLading.GetHandlingById(id);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetRuning(int id)
        {
            var checkPermission = await _common.CheckPermission("F0007");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.SetRunning(id);

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
        public async Task<IActionResult> CancelHandling(int id)
        {
            var checkPermission = await _common.CheckPermission("F0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.CancelHandling(id);

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
        public async Task<IActionResult> SetRuningLess(string id)
        {
            var checkPermission = await _common.CheckPermission("F0007");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.SetRunningLess(id);

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
        public async Task<IActionResult> CancelHandlingLess(string id)
        {
            var checkPermission = await _common.CheckPermission("F0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.CancelHandlingLess(id);

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
        public async Task<IActionResult> UpdateHandling(int id, UpdateHandling request)
        {
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.UpdateHandling(id, request);

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
        public async Task<IActionResult> GetListImage(int handlingId)
        {
            var checkPermission = await _common.CheckPermission("F0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var list = await _billOfLading.GetListImageByHandlingId(handlingId);
            return Ok(list);
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> CloneHandling(int id)
        //{
        //    var checkPermission = await _common.CheckPermission("F0001");
        //    if (checkPermission.isSuccess == false)
        //    {
        //        return BadRequest(checkPermission.Message);
        //    }

        //    var copy = await _billOfLading.CloneHandling(id);

        //    if (copy.isSuccess)
        //    {
        //        return Ok(copy.Message);
        //    }
        //    else
        //    {
        //        return BadRequest(copy.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> RemoveHandling(int id)
        //{
        //    var checkPermission = await _common.CheckPermission("F0001");
        //    if (checkPermission.isSuccess == false)
        //    {
        //        return BadRequest(checkPermission.Message);
        //    }

        //    var remove = await _billOfLading.RemoveHandling(id);

        //    if (remove.isSuccess)
        //    {
        //        return Ok(remove.Message);
        //    }
        //    else
        //    {
        //        return BadRequest(remove.Message);
        //    }
        //}

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteImage(int fileId)
        {
            var checkPermission = await _common.CheckPermission("F0005");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var del = await _billOfLading.DeleteImageById(fileId);

            if (del.isSuccess)
            {
                return Ok(del.Message);
            }
            else
            {
                return BadRequest(del.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var checkPermission = await _common.CheckPermission("F0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var image = await _billOfLading.GetImageById(id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), image.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", image.FileName);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataRoadTransportByCusId(string id)
        {
            var list = await _billOfLading.LoadDataRoadTransportByCusId(id);
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFile([FromForm] UploadImagesHandling request)
        {
            var checkPermission = await _common.CheckPermission("F0006");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var uploadFile = await _billOfLading.UploadFile(request);

            if (uploadFile.isSuccess)
            {
                return Ok(uploadFile.Message);
            }
            else
            {
                return BadRequest(uploadFile.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateTransportLess(CreateTransportLess request)
        {
            var create = await _billOfLading.CreateTransportLess(request);
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
        public async Task<IActionResult> LoadJoinTransports(JoinTransports request)
        {
            var data = await _billOfLading.LoadJoinTransport(request);

            if (!string.IsNullOrEmpty(data.MessageErrors))
            {
                return BadRequest(data.MessageErrors);
            }

            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateHandlingLess(CreateHandlingLess request)
        {
            var create = await _billOfLading.CreateHandlingLess(request);

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
        public async Task<IActionResult> UpdateHandlingLess(string handlingId, UpdateHandlingLess request)
        {
            var update = await _billOfLading.UpdateHandlingLess(handlingId, request);
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
        public async Task<IActionResult> GetTransportLessById(string transportId)
        {
            var transport = await _billOfLading.GetTransportLessById(transportId);

            if (transport == null)
            {
                return BadRequest("Vận đơn không tồn tại");
            }
            return Ok(transport);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateTransportLess(string transportId, UpdateTransportLess request)
        {
            var update = await _billOfLading.UpdateTransportLess(transportId, request);

            if (update.isSuccess == true)
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
        public async Task<IActionResult> ExportExcelHandLing([FromQuery] PaginationFilter filter, string[] customers, CancellationToken cancellationToken)
        {
            if (filter.fromDate == null && filter.toDate == null)
            {
                return BadRequest("Vui lòng chọn mốc thời gian để xuất Excel");
            }

            await Task.Yield();
            var stream = new MemoryStream();
            filter.PageNumber = 1;
            filter.PageSize = 500000;
            var data = await _billOfLading.GetListHandling("", customers, filter);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DieuPhoi");
                workSheet.Cells[1, 1].Value = "Mã Vân Đơn KH";
                workSheet.Cells[1, 2].Value = "Loại Vận Đơn";
                workSheet.Cells[1, 3].Value = "Khách Hàng";
                workSheet.Cells[1, 4].Value = "Đơn Vị Vận Tải";
                workSheet.Cells[1, 5].Value = "Phương Thức Vận Chuyển";
                workSheet.Cells[1, 6].Value = "Điểm Lấy Hàng";
                workSheet.Cells[1, 7].Value = "Điểm Trả Hàng";
                workSheet.Cells[1, 8].Value = "Điểm Lấy Rỗng";
                workSheet.Cells[1, 9].Value = "Mã Số Xe";
                workSheet.Cells[1, 10].Value = "Mã CONT";
                workSheet.Cells[1, 11].Value = "Hãng Tàu";
                workSheet.Cells[1, 12].Value = "Loại Phương Tiện";
                workSheet.Cells[1, 13].Value = "Khối Lượng";
                workSheet.Cells[1, 14].Value = "Thể Tích";
                workSheet.Cells[1, 15].Value = "Trạng Thái";
                workSheet.Cells[1, 16].Value = "Thời Gian Tạo";
                int row = 2;
                foreach (var item in data.dataResponse)
                {
                    workSheet.Cells[row, 1].Value = item.MaVanDonKH;
                    workSheet.Cells[row, 2].Value = item.PhanLoaiVanDon == "nhap" ? "Nhập" : "Xuất";
                    workSheet.Cells[row, 3].Value = item.MaKH;
                    workSheet.Cells[row, 4].Value = item.DonViVanTai;
                    workSheet.Cells[row, 5].Value = item.MaPTVC;
                    workSheet.Cells[row, 6].Value = item.DiemLayHang;
                    workSheet.Cells[row, 7].Value = item.DiemTraHang;
                    workSheet.Cells[row, 8].Value = item.DiemLayRong;
                    workSheet.Cells[row, 9].Value = item.MaSoXe;
                    workSheet.Cells[row, 10].Value = item.ContNo;
                    workSheet.Cells[row, 11].Value = item.HangTau;
                    workSheet.Cells[row, 12].Value = item.PTVanChuyen;
                    workSheet.Cells[row, 13].Value = item.KhoiLuong;
                    workSheet.Cells[row, 14].Value = item.TheTich;
                    workSheet.Cells[row, 15].Value = item.TrangThai;
                    workSheet.Cells[row, 16].Value = item.ThoiGianTaoDon.ToString();
                    row++;
                }

                workSheet.Cells["A1:P1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1:P1"].Style.Font.Bold = true;
                workSheet.Cells["A1:P1"].Style.Font.Size = 14;

                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                package.Save();
            }
            stream.Position = 0;
            string excelName = $"DieuPhoi " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

            //return File(stream, "application/octet-stream", excelName);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ExportExcelHandlingLess([FromQuery] PaginationFilter filter, string[] customers, CancellationToken cancellationToken)
        {
            if (filter.fromDate == null && filter.toDate == null)
            {
                return BadRequest("Vui lòng chọn mốc thời gian để xuất Excel");
            }

            await Task.Yield();
            var stream = new MemoryStream();
            filter.PageNumber = 1;
            filter.PageSize = 500000;
            var data = await _billOfLading.GetListHandlingLess(customers, filter);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DieuPhoi");
                workSheet.Cells[1, 1].Value = "Mã Chuyến";
                workSheet.Cells[1, 2].Value = "Mã Vận Đơn";
                workSheet.Cells[1, 3].Value = "Loại Vận Đơn";
                workSheet.Cells[1, 4].Value = "Khách Hàng";
                workSheet.Cells[1, 5].Value = "Đơn Vị Vận Tải";
                workSheet.Cells[1, 6].Value = "Phương Thức Vận Chuyển";
                workSheet.Cells[1, 7].Value = "Điểm Lấy Hàng";
                workSheet.Cells[1, 8].Value = "Điểm Trả Hàng";
                workSheet.Cells[1, 9].Value = "Điểm Lấy Rỗng";
                workSheet.Cells[1, 10].Value = "Mã Số Xe";
                workSheet.Cells[1, 11].Value = "Mã CONT";
                workSheet.Cells[1, 12].Value = "Hãng Tàu";
                workSheet.Cells[1, 13].Value = "Loại Phương Tiện";
                workSheet.Cells[1, 14].Value = "Khối Lượng";
                workSheet.Cells[1, 15].Value = "Thể Tích";
                workSheet.Cells[1, 16].Value = "Trạng Thái";
                workSheet.Cells[1, 17].Value = "Thời Gian Tạo";
                int row = 2;
                foreach (var item in data.dataResponse)
                {
                    workSheet.Cells[row, 1].Value = item.MaChuyen;
                    workSheet.Cells[row, 2].Value = item.MaVanDonKH;
                    workSheet.Cells[row, 3].Value = item.PhanLoaiVanDon == "nhap" ? "Nhập" : "Xuất";
                    workSheet.Cells[row, 4].Value = item.MaKH;
                    workSheet.Cells[row, 5].Value = item.DonViVanTai;
                    workSheet.Cells[row, 6].Value = item.MaPTVC;
                    workSheet.Cells[row, 7].Value = item.DiemLayHang;
                    workSheet.Cells[row, 8].Value = item.DiemTraHang;
                    workSheet.Cells[row, 9].Value = item.DiemLayRong;
                    workSheet.Cells[row, 10].Value = item.MaSoXe;
                    workSheet.Cells[row, 11].Value = item.ContNo;
                    workSheet.Cells[row, 12].Value = item.HangTau;
                    workSheet.Cells[row, 13].Value = item.PTVanChuyen;
                    workSheet.Cells[row, 14].Value = item.KhoiLuong;
                    workSheet.Cells[row, 15].Value = item.TheTich;
                    workSheet.Cells[row, 16].Value = item.TrangThai;
                    workSheet.Cells[row, 17].Value = item.ThoiGianTaoDon.ToString();
                    row++;
                }

                workSheet.Cells["A1:Q1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1:Q1"].Style.Font.Bold = true;
                workSheet.Cells["A1:Q1"].Style.Font.Size = 14;

                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                package.Save();
            }
            stream.Position = 0;
            string excelName = $"DieuPhoi " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

            //return File(stream, "application/octet-stream", excelName);
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}