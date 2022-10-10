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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteBillOfLading(DeleteBillOfLading request)
        {
            var edit = await _billOfLading.DeleteBillOfLading( request);

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
