using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.DriverModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.BillOfLadingManage;
using TBSLogistics.Service.Repository.DriverManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfLadingController : ControllerBase
    {
        private readonly IBillOfLading _billOfLading;
        private IPaginationService _paninationService;

        public BillOfLadingController(IBillOfLading billOfLading,IPaginationService paginationService)
        {
            _billOfLading = billOfLading;
            _paninationService = paginationService;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataHandling(string RoadId)
        {
            var data = await _billOfLading.LoadDataHandling(RoadId);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTransportById(string transportId)
        {
            var data = await _billOfLading.GetTransportById(transportId);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransport([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListTransport(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListTransport>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);

        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateTransport(CreateTransport request)
        {
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
    }
}
