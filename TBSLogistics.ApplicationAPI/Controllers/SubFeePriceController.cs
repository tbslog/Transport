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
using TBSLogistics.Service.Services.Common;
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
        private readonly ICommon _common;

        public SubFeePriceController(ISubFeePrice subFeePrice, IPaginationService uriService, ICommon common)
        {
            _subFeePrice = subFeePrice;
            _uriService = uriService;
            _common = common;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request)
        {
            var checkPermission = await _common.CheckPermission("D0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("D0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("D0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("D0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var sfp = await _subFeePrice.GetSubFeePriceById(id);
            return Ok(sfp);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeePrice([FromQuery] PaginationFilter filter)
        {
            var checkPermission = await _common.CheckPermission("D0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("D0005");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("D0006");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
    }
}
