using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceTableController : ControllerBase
    {
        private readonly IPriceTable _priceTable;
        private IPaginationService _paninationService;

        public PriceTableController(IPriceTable priceTable, IPaginationService paninationService)
        {
            _priceTable = priceTable;
            _paninationService = paninationService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
        {
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
            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTable(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<GetListPiceTableRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableByContractId(string Id, int PageNumber, int PageSize, string onlyct = null)
        {
            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableByContractId(Id, onlyct, PageNumber, PageSize);
            var pagedReponse = PaginationHelper.CreatePagedReponse<GetPriceListRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableByCustomerId(string Id)
        {
            var list = await _priceTable.GetListPriceTableByCustommerId(Id);

            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableApprove([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableApprove(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListApprove>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApprovePriceTable(ApprovePriceTable request)
        {
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
            var data = await _priceTable.GetPriceTableById(id);

            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdatePriceTable(int id, GetPriceListRequest request)
        {
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
