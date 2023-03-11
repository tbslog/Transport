using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
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

        public PriceTableController(IPriceTable priceTable, IPaginationService paninationService, ICommon common)
        {
            _priceTable = priceTable;
            _paninationService = paninationService;
            _common = common;
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTable([FromQuery] PaginationFilter filter)
        {
            var checkPermissionKH = await _common.CheckPermission("C0001");
            var checkPermissionNCC = await _common.CheckPermission("C0005");

            if (checkPermissionKH.isSuccess == true)
            {
                filter.customerType = "KH";
            }

            if (checkPermissionNCC.isSuccess == true)
            {
                filter.customerType = "NCC";
            }

            if (checkPermissionKH.isSuccess && checkPermissionNCC.isSuccess)
            {
                filter.customerType = "";
            }
            if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
            {
                return BadRequest("Bạn không có quyền hạn");
            }

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTable(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<GetListPiceTableRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableByContractId(string Id, int PageNumber, int PageSize, string onlyct = null)
        {
            var checkPermission = await _common.CheckPermission("C0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableByContractId(Id, onlyct, PageNumber, PageSize);
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
        public async Task<IActionResult> UpdatePriceTable(int id, GetPriceListRequest request)
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
    }
}