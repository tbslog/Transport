using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.SubFeePriceManage;

namespace TBSLogistics.ApplicationAPI.Controllers
{
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
        public async Task<IActionResult> ApproveSubFeePrice(long[] ids, string HDPL)
        {
            var ApproveSubFeePrice = await _subFeePrice.ApproveSubFeePrice(ids, HDPL);

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
        public async Task<IActionResult> GetListSubFeePrice(string KH, string HDPL, DateTime ngayapdung, byte trangthai)
        {
            var list = await _subFeePrice.GetListSubFeePrice(KH, HDPL, ngayapdung, trangthai);
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DisableSubFeePrice(long[] ids, string HDPL)
        {
            var DisableSubFeePrice = await _subFeePrice.DisableSubFeePrice(ids, HDPL);

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
        public async Task<IActionResult> DeleteSubFeePrice(long[] ids, string HDPL)
        {
            var DeleteSubFeePrice = await _subFeePrice.DeleteSubFeePrice(ids, HDPL);

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
