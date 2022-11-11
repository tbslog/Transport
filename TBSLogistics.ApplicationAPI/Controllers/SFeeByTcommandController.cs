using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Service.Services.ProductServiceManage;
using TBSLogistics.Service.Services.SFeeByTcommandManage;


namespace TBSLogistics.ApplicationAPI.Controllers
{
  
    [Route("api/[controller]")]
    [ApiController]
    public class SFeeByTcommandController : ControllerBase
    {
        private readonly ISFeeByTcommand _SFeeByTcommand;
        public SFeeByTcommandController(ISFeeByTcommand sFeeByTcommand)
        {
            _SFeeByTcommand = sFeeByTcommand;
        }
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request)
        {
            var Create = await _SFeeByTcommand.CreateSFeeByTCommand(request);
            if (Create.isSuccess == true)
            {
                return Ok(Create.Message);
            }
            else
            {
                return BadRequest(Create.Message);
            }
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> ApproveSFeeByTCommand(List<ApproveSFeeByTCommand> request)
        {
            var approve =await _SFeeByTcommand.ApproveSFeeByTCommand(request);
            if (approve.isSuccess == true)
            {
                return Ok(approve.Message);
            }
            else
            {
                return BadRequest(approve.Message);
            }
        }
        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> DeleteSFeeByTCommand(DeleteSFeeByTCommand Id)
        {
            var delete = await _SFeeByTcommand.DeleteSFeeByTCommand(Id);
            if (delete.isSuccess == true)
            {
                return Ok(delete.Message);
            }
            else
            {
                return BadRequest(delete.Message);
            }
        }
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeeByHandling(long IdTcommand)
        {
            var list = await _SFeeByTcommand.GetListSubFeeByHandling(IdTcommand);
            return Ok(list);

        }
    }
}
