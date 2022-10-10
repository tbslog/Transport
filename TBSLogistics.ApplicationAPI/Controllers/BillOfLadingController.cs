using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Service.Repository.BillOfLadingManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfLadingController : ControllerBase
    {
        private readonly IBillOfLading _billOfLading;

        public BillOfLadingController(IBillOfLading billOfLading)
        {
            _billOfLading = billOfLading;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataTransport(string RoadId)
        {
            var data = await _billOfLading.getListRoadBillOfLading(RoadId);
            return Ok(data);
        }
    }
}
