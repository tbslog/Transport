using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.FileModel;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.BillOfLadingManage;
using TBSLogistics.Service.Services.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LayTrongTaiXe(string vehicleType, string DonVi, double giaTri)
        {
            var trongtai = await _billOfLading.LayTrongTaiXe(vehicleType, DonVi, giaTri);
            return Ok(trongtai);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransport([FromQuery] PaginationFilter filter, ListFilter listFilter)
        {
            var checkPermission = await _common.CheckPermission("E0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListTransport(listFilter, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListTransport>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListHandlingLess([FromQuery] PaginationFilter filter, ListFilter listFilter)
        {
            var checkPermission = await _common.CheckPermission("F0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListHandlingLess(listFilter, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListHandlingByTransportId([FromQuery] PaginationFilter filter, string transportId)
        {
            var checkPermission = await _common.CheckPermission("E0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListHandlingByTransportId(transportId, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReadFileExcelTransport(IFormFile formFile, CancellationToken cancellationToken)
        {
            var ImportExcel = await _billOfLading.CreateTransportByExcel(formFile, cancellationToken);

            if (ImportExcel.isSuccess == true)
            {
                return Ok(ImportExcel.Message);
            }
            else
            {
                return BadRequest(ImportExcel.Message);
            }
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
        public async Task<IActionResult> ChangeStatusHandling(int id,string maChuyen)
        {
            var checkPermission = await _common.CheckPermission("F0007");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.ChangeStatusHandling(id, maChuyen);

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
        public async Task<IActionResult> CancelHandlingByCustomer(int id, string note)
        {
            var action = await _billOfLading.CancelHandlingByCustomer(id, note);
            if (action.isSuccess)
            {
                return Ok(action.Message);
            }
            return BadRequest(action.Message);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> AcceptOrRejectTransport(string transportId, int action)
        {
            var checkPermission = await _common.CheckPermission("E0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var result = await _billOfLading.AcceptOrRejectTransport(transportId, action);

            if (result.isSuccess)
            {
                return Ok(result.Message);
            }
            return BadRequest(result.Message);
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteImage(int docId)
        {
            var checkPermission = await _common.CheckPermission("F0005");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var del = await _billOfLading.DeleteImageById(docId);

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
        public async Task<IActionResult> GetDocById(int docId)
        {
            var doc = await _billOfLading.GetDocById(docId);
            return Ok(doc);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateDoc([FromForm] DocumentType request)
        {
            var checkPermission = await _common.CheckPermission("F0006");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var create = await _billOfLading.CreateDoc(request);

            if (create.isSuccess == true)
            {
                return Ok(create.Message);
            }

            return BadRequest(create.Message);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateDoc(int docId, [FromForm] DocumentType request)
        {
            var checkPermission = await _common.CheckPermission("F0006");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var update = await _billOfLading.UpdateDoc(docId,request);
            if (update.isSuccess == true)
            {
                return Ok(update.Message);
            }

            return BadRequest(update.Message);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateTransportLess(CreateTransportLess request)
        {
            var checkPermission = await _common.CheckPermission("E0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("F0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("E0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
        public async Task<IActionResult> SendMailToSupplier(GetIdHandling handlingIds)
        {
            var checkPermission = await _common.CheckPermission("F0014");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var sendmail = await _billOfLading.SendMailToSuppliers(handlingIds);
            return Ok(sendmail.Message);
        }

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> ExportExcelHandLing([FromQuery] PaginationFilter filter, ListFilter listFilter, CancellationToken cancellationToken)
        //{
        //    var checkPermission = await _common.CheckPermission("F0012");
        //    if (checkPermission.isSuccess == false)
        //    {
        //        return BadRequest(checkPermission.Message);
        //    }

        //    if (filter.fromDate == null && filter.toDate == null)
        //    {
        //        return BadRequest("Vui lòng chọn mốc thời gian để xuất Excel");
        //    }

        //    await Task.Yield();
        //    var stream = new MemoryStream();
        //    filter.PageNumber = 1;
        //    filter.PageSize = 500000;
        //    var data = await _billOfLading.GetListHandlingLess(listFilter, filter);
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (var package = new ExcelPackage(stream))
        //    {
        //        var workSheet = package.Workbook.Worksheets.Add("DieuPhoi");
        //        workSheet.Cells[1, 1].Value = "Booking No";
        //        workSheet.Cells[1, 2].Value = "Loại Vận Đơn";
        //        workSheet.Cells[1, 3].Value = "Phương Thức Vận Chuyển";
        //        workSheet.Cells[1, 4].Value = "Khách Hàng";
        //        workSheet.Cells[1, 5].Value = "Mã CONT";
        //        workSheet.Cells[1, 6].Value = "Loại Phương Tiện";
        //        workSheet.Cells[1, 7].Value = "Hãng Tàu";
        //        workSheet.Cells[1, 8].Value = "Điểm Lấy Rỗng";
        //        workSheet.Cells[1, 9].Value = "Điểm Trả Rỗng";
        //        workSheet.Cells[1, 10].Value = "Điểm Đóng Hàng";
        //        workSheet.Cells[1, 11].Value = "Điểm Hạ Hàng";
        //        workSheet.Cells[1, 12].Value = "Đơn Vị Vận Tải";
        //        workSheet.Cells[1, 13].Value = "Khối Lượng";
        //        workSheet.Cells[1, 14].Value = "Thể Tích";
        //        workSheet.Cells[1, 15].Value = "Số Kiện";
        //        workSheet.Cells[1, 16].Value = "Trạng Thái";
        //        workSheet.Cells[1, 17].Value = "Thời Gian Tạo";
        //        int row = 2;
        //        foreach (var item in data.dataResponse)
        //        {
        //            workSheet.Cells[row, 1].Value = item.MaVanDonKH;
        //            workSheet.Cells[row, 2].Value = item.PhanLoaiVanDon == "nhap" ? "Nhập" : "Xuất";
        //            workSheet.Cells[row, 3].Value = item.MaPTVC;
        //            workSheet.Cells[row, 4].Value = item.MaKH;
        //            workSheet.Cells[row, 5].Value = item.ContNo;
        //            workSheet.Cells[row, 6].Value = item.PTVanChuyen;
        //            workSheet.Cells[row, 7].Value = item.HangTau;
        //            workSheet.Cells[row, 8].Value = item.DiemLayRong;
        //            workSheet.Cells[row, 9].Value = item.DiemTraRong;
        //            workSheet.Cells[row, 10].Value = item.DiemDau;
        //            workSheet.Cells[row, 11].Value = item.DiemCuoi;
        //            workSheet.Cells[row, 12].Value = item.DonViVanTai;
        //            workSheet.Cells[row, 13].Value = item.KhoiLuong;
        //            workSheet.Cells[row, 14].Value = item.TheTich;
        //            workSheet.Cells[row, 15].Value = item.SoKien;
        //            workSheet.Cells[row, 16].Value = item.TrangThai;
        //            workSheet.Cells[row, 17].Value = item.ThoiGianTaoDon.ToString();
        //            row++;
        //        }

        //        workSheet.Cells["A1:P1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //        workSheet.Cells["A1:P1"].Style.Font.Bold = true;
        //        workSheet.Cells["A1:P1"].Style.Font.Size = 14;

        //        workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
        //        workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //        workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //        workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //        workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

        //        package.Save();
        //    }
        //    stream.Position = 0;
        //    string excelName = $"DieuPhoi " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

        //    //return File(stream, "application/octet-stream", excelName);
        //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        //}

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ExportExcelHandlingLess([FromQuery] PaginationFilter filter, ListFilter listFilter, CancellationToken cancellationToken)
        {
            var checkPermission = await _common.CheckPermission("F0012");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            if (filter.fromDate == null && filter.toDate == null)
            {
                return BadRequest("Vui lòng chọn mốc thời gian để xuất Excel");
            }

            await Task.Yield();
            var stream = new MemoryStream();
            filter.PageNumber = 1;
            filter.PageSize = 100000;
            var data = await _billOfLading.GetListHandlingLess(listFilter, filter);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("DieuPhoi");
                workSheet.Cells[1, 1].Value = "Mã Chuyến";
                workSheet.Cells[1, 2].Value = "Booking No";
                workSheet.Cells[1, 3].Value = "Loại Vận Đơn";
                workSheet.Cells[1, 4].Value = "Phương Thức Vận Chuyển";
                workSheet.Cells[1, 5].Value = "Khách Hàng";
                workSheet.Cells[1, 6].Value = "Account";
                workSheet.Cells[1, 7].Value = "Đơn Vị Vận Tải";
                workSheet.Cells[1, 8].Value = "Mã CONT";
                workSheet.Cells[1, 9].Value = "Loại Phương Tiện";
                workSheet.Cells[1, 10].Value = "Hãng Tàu";
                workSheet.Cells[1, 11].Value = "Điểm Lấy Rỗng";
                workSheet.Cells[1, 12].Value = "Điểm Trả Rỗng";
                workSheet.Cells[1, 13].Value = "Điểm Đóng Hàng";
                workSheet.Cells[1, 14].Value = "Điểm Hạ Hàng";
                workSheet.Cells[1, 15].Value = "Khối Lượng";
                workSheet.Cells[1, 16].Value = "Thể Tích";
                workSheet.Cells[1, 17].Value = "Số Kiện";
                workSheet.Cells[1, 18].Value = "Trạng Thái";
                workSheet.Cells[1, 19].Value = "Thời Gian Tạo";
                int row = 2;
                foreach (var item in data.dataResponse)
                {
                    workSheet.Cells[row, 1].Value = item.MaChuyen;
                    workSheet.Cells[row, 2].Value = item.MaVanDonKH;
                    workSheet.Cells[row, 3].Value = item.PhanLoaiVanDon == "nhap" ? "Nhập" : "Xuất";
                    workSheet.Cells[row, 4].Value = item.MaPTVC;
                    workSheet.Cells[row, 5].Value = item.MaKH;
                    workSheet.Cells[row, 6].Value = item.AccountName;
                    workSheet.Cells[row, 7].Value = item.DonViVanTai;
                    workSheet.Cells[row, 8].Value = item.ContNo;
                    workSheet.Cells[row, 9].Value = item.PTVanChuyen;
                    workSheet.Cells[row, 10].Value = item.HangTau;
                    workSheet.Cells[row, 11].Value = item.DiemLayRong;
                    workSheet.Cells[row, 12].Value = item.DiemTraRong;
                    workSheet.Cells[row, 13].Value = item.DiemDau;
                    workSheet.Cells[row, 14].Value = item.DiemCuoi;
                    workSheet.Cells[row, 15].Value = item.KhoiLuong;
                    workSheet.Cells[row, 16].Value = item.TheTich;
                    workSheet.Cells[row, 17].Value = item.SoKien;
                    workSheet.Cells[row, 18].Value = item.TrangThai;
                    workSheet.Cells[row, 19].Value = item.ThoiGianTaoDon.ToString();
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