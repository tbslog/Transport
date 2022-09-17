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


        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateBillOfLading(CreateBillOfLadingRequest request)
        {
            var create = await _billOfLading.CreateBillOfLading(request);

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
        public async Task<IActionResult> EditBillOfLading(string billOfLadingId, EditBillOfLadingRequest request)
        {
            var edit = await _billOfLading.EditBillOfLading(billOfLadingId, request);

            if (edit.isSuccess == true)
            {
                return Ok(edit.Message);
            }
            else
            {
                return BadRequest(edit.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillOfLadingById(string billOfLadingId)
        {
            var billOfLading = await _billOfLading.GetBillOfLadingById(billOfLadingId);

            return Ok(billOfLading);
        }
    }
}
