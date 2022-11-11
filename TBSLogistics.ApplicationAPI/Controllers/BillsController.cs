using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.AddressManage;
using TBSLogistics.Service.Services.Bill;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillsController : ControllerBase
    {

        private readonly IBill _bill;
        private readonly IPaginationService _uriService;

        public BillsController(IBill bill, IPaginationService uriService)
        {
            _uriService = uriService;
            _bill = bill;
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustomerHasBill([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _bill.GetListCustomerHasBill(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustomerHasBill>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetBillByCustomerId(string customerId, DateTime fromDate, DateTime toDate)
        {
            var billResult = await _bill.GetBillByCustomerId(customerId, fromDate, toDate);

            return Ok(billResult);
        }
    }
}
