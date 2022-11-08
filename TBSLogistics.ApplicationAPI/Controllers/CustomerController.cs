using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private ICustomer _customer;
        private IPaginationService _uriService;

        public CustomerController(ICustomer customer, IPaginationService uriService)
        {
            _customer = customer;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
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

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateCustomer(string Id, EditCustomerRequest request)
        {
            var Edit = await _customer.EditCustomer(Id, request);

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
        public async Task<IActionResult> GetListCustomer([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _customer.getListCustommer(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustommerRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustomerOptionSelect()
        {
            var listOptionSelect = await _customer.getListCustomerOptionSelect();
            return Ok(listOptionSelect);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCustomerById(string Id)
        {
            var Custommer = await _customer.GetCustomerById(Id);
            return Ok(Custommer);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            var ImportExcel = await _customer.ReadExcelFile(formFile, cancellationToken);

            if (ImportExcel.isSuccess == true)
            {
                return Ok(ImportExcel.Message);
            }
            else
            {
                return BadRequest(ImportExcel.DataReturn + " --- " + ImportExcel.Message);
            }
        }
    }
}
