using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.ProductServiceManage;
using TBSLogistics.Service.Services.SFeeByTcommandManage;


namespace TBSLogistics.ApplicationAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SFeeByTcommandController : ControllerBase
    {
        private readonly ISFeeByTcommand _SFeeByTcommand;
        private readonly IPaginationService _uriService;


        public SFeeByTcommandController(ISFeeByTcommand sFeeByTcommand, IPaginationService uriService)
        {
            _SFeeByTcommand = sFeeByTcommand;
            _uriService = uriService;
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
        public async Task<IActionResult> GetListSubFeeIncurredApprove([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _SFeeByTcommand.GetListSubFeeIncurredApprove(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListSubFeeIncurred>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSubFeeIncurredById(int id)
        {
            var data = await _SFeeByTcommand.GetSubFeeIncurredById(id);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApproveSubFeeIncurred(List<ApproveSFeeByTCommand> request)
        {
            var ApproveSubFeePrice = await _SFeeByTcommand.ApproveSubFeeIncurred(request);

            if (ApproveSubFeePrice.isSuccess == true)
            {
                return Ok(ApproveSubFeePrice.Message);
            }
            else
            {
                return BadRequest(ApproveSubFeePrice.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeeIncurredByHandling(int id)
        {
            var list = await _SFeeByTcommand.GetListSubFeeIncurredByHandling(id);
            return Ok(list);
        }
    }
}
