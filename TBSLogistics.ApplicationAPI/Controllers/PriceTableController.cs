using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Service.Repository.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceTableController : ControllerBase
    {
        private readonly IPriceTable _priceTable;

        public PriceTableController(IPriceTable priceTable)
        {
            _priceTable = priceTable;
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
        public async Task<IActionResult> EditPriceTable(string PriceListId, UpdatePriceListRequest request)
        {
            var update = await _priceTable.EditPriceTable(PriceListId, request);

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
        public async Task<IActionResult> GetPriceTableById(string PriceListId)
        {
            var list = await _priceTable.GetPriceTableById(PriceListId);
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTable()
        {
            var PriceList = await _priceTable.GetListPriceTable();
            return Ok(PriceList);
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
