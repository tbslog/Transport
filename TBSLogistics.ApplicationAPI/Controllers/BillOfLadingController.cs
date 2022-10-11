using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.BillOfLadingManage;
using TBSLogistics.Service.Services.ProductServiceManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfLadingController : ControllerBase
    {
        private readonly IBillOfLading _billOfLading;
        private readonly IPaginationService _pagination;
        

        public BillOfLadingController(IBillOfLading billOfLading, IPaginationService pagination)
        {
            _billOfLading = billOfLading;
            _pagination = pagination;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataTransport(string RoadId)
        {
            var data = await _billOfLading.getListRoadBillOfLading(RoadId);
            return Ok(data);
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillOfLading([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListBillOfLading(filter);
            var pagedReponse = PaginationHelper.CreatePagedReponse<GetBillOfLadingRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
            return Ok(pagedReponse);
        }
    }
}
