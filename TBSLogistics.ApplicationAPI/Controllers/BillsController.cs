using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Bill;
using TBSLogistics.Service.Services.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {

        private readonly IBill _bill;
        private readonly IPaginationService _uriService;
        private readonly ICommon _common;

        public BillsController(IBill bill, IPaginationService uriService, ICommon common)
        {
            _common = common;
            _uriService = uriService;
            _bill = bill;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransportByCustomerId(string customerId, int ky, [FromQuery] PaginationFilter filter)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _bill.GetListTransportByCustomerId(customerId, ky, filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListVanDon>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillByCustomerId(string customerId, int ky)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var billResult = await _bill.GetBillByCustomerId(customerId, ky);

            return Ok(billResult);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillByTransportId(string customerId, string transportId)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var data = await _bill.GetBillByTransportId(customerId, transportId);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListBillHandling([FromQuery] PaginationFilter filter)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _bill.GetListBillHandling(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListBillHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListKy(string customerId)
        {
            var checkPermission = await _common.CheckPermission("G0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var data = await _bill.GetListKyThanhToan(customerId);

            return Ok(data);
        }
    }
}
