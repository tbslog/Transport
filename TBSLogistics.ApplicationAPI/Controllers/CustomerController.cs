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
using TBSLogistics.Service.Services.CustommerManage;
using TBSLogistics.Service.Services.Common;

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
        private ICommon _common;

        public CustomerController(ICustomer customer, IPaginationService uriService, ICommon common)
        {
            _customer = customer;
            _uriService = uriService;
            _common = common;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateCustomer(CreateCustomerRequest request)
        {
            var checkPermission = await _common.CheckPermission("A0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }


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
            var checkPermission = await _common.CheckPermission("A0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }


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
            var checkPermission = await _common.CheckPermission("A0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _customer.getListCustommer(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustommerRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustomerOptionSelect(string type = null)
        {
            var listOptionSelect = await _customer.getListCustomerOptionSelect(type);
            return Ok(listOptionSelect);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetCustomerById(string Id)
        {
            var checkPermission = await _common.CheckPermission("A0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var Custommer = await _customer.GetCustomerById(Id);
            return Ok(Custommer);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            //var ImportExcel = await _customer.ReadExcelFile(formFile, cancellationToken);

            //if (ImportExcel.isSuccess == true)
            //{
            //    return Ok(ImportExcel.Message);
            //}
            //else
            //{
            //    return BadRequest(ImportExcel.DataReturn + " --- " + ImportExcel.Message);
            //}
            return BadRequest();

        }
    }
}
