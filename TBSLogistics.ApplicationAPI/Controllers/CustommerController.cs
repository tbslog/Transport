using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.Common;
using TBSLogistics.Service.Repository.CustommerManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustommerController : ControllerBase
    {
        private ICustomer _customer;
        private IUriService _uriService;

        public CustommerController(ICustomer customer, IUriService uriService)
        {
            _customer = customer;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCustommer(CreateCustomerRequest request)
        {
            var Create = await _customer.CreateCustomer(request);

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
        public async Task<IActionResult> EdtiCustommer(string CustommerId, EditCustomerRequest request)
        {
            var Edit = await _customer.EditCustomer(CustommerId, request);

            if (Edit.isSuccess == true)
            {
                return Ok(Edit.Message);
            }
            else
            {
                return BadRequest(Edit.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustommer([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _customer.getListCustommer(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustommerRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCustommerById(string CustommerId)
        {
            var Custommer = await _customer.GetCustomerById(CustommerId);
            return Ok(Custommer);
        }
    }
}
