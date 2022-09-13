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

        public PriceTableController(IPriceTable priceTable,IPaginationService paninationService)
        {
            _priceTable = priceTable;
            _paninationService = paninationService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreatePriceTable(CreatePriceListRequest request)
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

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdatePriceTable(string Id, UpdatePriceListRequest request)
        {
            var update = await _priceTable.EditPriceTable(Id, request);

            if (update.isSuccess == true)
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
        public async Task<IActionResult> GetPriceTableById(string Id)
        {
            var list = await _priceTable.GetPriceTableById(Id);
            return Ok(list);
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
        public async Task<IActionResult> GetListPriceTableByCusId(string CustomerId)
        {
            var list = await _priceTable.GetListPriceTableByCusId(CustomerId);
            return Ok(list);
        }
    }
}
