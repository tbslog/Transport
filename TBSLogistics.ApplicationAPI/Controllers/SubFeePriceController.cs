using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.SubFeePriceManage;

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubFeePriceController : ControllerBase
    {
        private readonly ISubFeePrice _subFeePrice;
        private readonly IPaginationService _uriService;

        public SubFeePriceController(ISubFeePrice subFeePrice, IPaginationService uriService)
        {
            _subFeePrice = subFeePrice;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request)
        {
            var CreateSubFeePrice = await _subFeePrice.CreateSubFeePrice(request);

            if (CreateSubFeePrice.isSuccess == true)
            {
                return Ok(CreateSubFeePrice.Message);
            }
            else
            {
                return BadRequest(CreateSubFeePrice.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateSubFeePrice(long Id, UpdateSubFeePriceRequest request)
        {
            var UpdateSubFeePrice = await _subFeePrice.UpdateSubFeePrice(Id, request);

            if (UpdateSubFeePrice.isSuccess == true)
            {
                return Ok(UpdateSubFeePrice.Message);
            }
            else
            {
                return BadRequest(UpdateSubFeePrice.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApproveSubFeePrice(List<ApproveSubFee> request)
        {
            var ApproveSubFeePrice = await _subFeePrice.ApproveSubFeePrice(request);

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
        public async Task<IActionResult> GetSubFeePriceById(long id)
        {
            var sfp = await _subFeePrice.GetSubFeePriceById(id);
            return Ok(sfp);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeePrice([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _subFeePrice.GetListSubFeePrice(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListSubFeePriceRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeeSelect()
        {
            var list = await _subFeePrice.GetListSubFeeSelect();
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DisableSubFeePrice(List<long> ids)
        {
            var DisableSubFeePrice = await _subFeePrice.DisableSubFeePrice(ids);

            if (DisableSubFeePrice.isSuccess == true)
            {
                return Ok(DisableSubFeePrice.Message);
            }
            else
            {
                return BadRequest(DisableSubFeePrice.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteSubFeePrice(List<long> ids)
        {
            var DeleteSubFeePrice = await _subFeePrice.DeleteSubFeePrice(ids);

            if (DeleteSubFeePrice.isSuccess == true)
            {
                return Ok(DeleteSubFeePrice.Message);
            }
            else
            {
                return BadRequest(DeleteSubFeePrice.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeeIncurredApprove([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _subFeePrice.GetListSubFeeIncurredApprove(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListSubFeeIncurred>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSubFeeIncurredById(int id)
        {
            var data = await _subFeePrice.GetSubFeeIncurredById(id);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApproveSubFeeIncurred(List<ApproveSubFee> request)
        {
            var ApproveSubFeePrice = await _subFeePrice.ApproveSubFeeIncurred(request);

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
            var list = await _subFeePrice.GetListSubFeeIncurredByHandling(id);
            return Ok(list);    
        }
    }
}
