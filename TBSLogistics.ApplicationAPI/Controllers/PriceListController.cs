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
    public class PriceListController : ControllerBase
    {
        private readonly IPriceList _PriceList;

        public PriceListController(IPriceList priceList)
        {
            _PriceList = priceList;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreatePriceList(CreatePriceListRequest request)
        {
            var create = await _PriceList.CreatePriceList(request);

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
        public async Task<IActionResult> EditPriceList(string PriceListId, UpdatePriceListRequest request)
        {
            var update = await _PriceList.EditPriceList(PriceListId, request);

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
        public async Task<IActionResult> GetPriceListById(string PriceListId)
        {
            var list = await _PriceList.GetPriceListById(PriceListId);
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceList()
        {
            var PriceList = await _PriceList.GetListPriceList();
            return Ok(PriceList);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceListByCusId(string CustomerId)
        {
            var list = await _PriceList.GetListPriceListByCusId(CustomerId);
            return Ok(list);
        }
    }
}
