using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
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
        public async Task<IActionResult> GetListPriceTableApprove([FromQuery] PaginationFilter filter)
        {
            var checkPermission = await _common.CheckPermission("C0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableApprove(filter);

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

            await Task.Yield();
            var stream = new MemoryStream();
            var data = await _priceTable.GetListPriceTableExportExcel(cusType);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(stream))
            {
                var workSheet = package.Workbook.Worksheets.Add("BangGia");
                workSheet.Cells[1, 1].Value = "Hợp Đồng";
                workSheet.Cells[1, 2].Value = "Phân Loại";
                workSheet.Cells[1, 3].Value = cusType == "KH" ? "Khách Hàng" : "Đơn Vị Vận Tải";
                workSheet.Cells[1, 4].Value = "Account";
                workSheet.Cells[1, 5].Value = "Điểm Đóng Hàng";
                workSheet.Cells[1, 6].Value = "Điểm Hạ Hàng";
                workSheet.Cells[1, 7].Value = "Điểm Lấy/Trả Rỗng";
                workSheet.Cells[1, 8].Value = "Đơn Giá";
                workSheet.Cells[1, 9].Value = "PTVC";
                workSheet.Cells[1, 10].Value = "Loại Phương Tiện";
                workSheet.Cells[1, 11].Value = "Loại Hàng Hóa";
                workSheet.Cells[1, 12].Value = "Đơn Vị Tính";
                workSheet.Cells[1, 13].Value = "Ngày Áp Dụng";


                int row = 2;
                foreach (var item in data)
                {
                    workSheet.Cells[row, 1].Value = item.MaHopDong;
                    workSheet.Cells[row, 2].Value = item.SoHopDongCha;
                    workSheet.Cells[row, 3].Value = item.TenKH;
                    workSheet.Cells[row, 4].Value = item.AccountName;
                    workSheet.Cells[row, 5].Value = item.DiemDau;
                    workSheet.Cells[row, 6].Value = item.DiemCuoi;
                    workSheet.Cells[row, 7].Value = item.DiemLayTraRong;
                    workSheet.Cells[row, 8].Value = item.DonGia;
                    workSheet.Cells[row, 9].Value = item.MaPTVC;
                    workSheet.Cells[row, 10].Value = item.MaLoaiPhuongTien;
                    workSheet.Cells[row, 11].Value = item.MaLoaiHangHoa;
                    workSheet.Cells[row, 12].Value = item.MaDVT;
                    workSheet.Cells[row, 13].Value = item.NgayApDung;
                    row++;
                }


                workSheet.Cells["M2:M" + row].Style.Numberformat.Format = "DD-MM-YYYY HH:mm";
                workSheet.Cells["A1:M1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                workSheet.Cells["A1:M1"].Style.Font.Bold = true;
                workSheet.Cells["A1:M1"].Style.Font.Size = 14;

                workSheet.Cells[workSheet.Dimension.Address].AutoFitColumns();
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[workSheet.Dimension.Address].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                package.Save();
            }
            stream.Position = 0;
            string excelName = $"BangGia " + DateTime.Now.ToString("dd-MM-yyyy") + ".xlsx";

            //return File(stream, "application/octet-stream", excelName);  
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}