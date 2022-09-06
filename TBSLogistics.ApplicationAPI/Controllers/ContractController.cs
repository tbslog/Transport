using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.ContractManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContract _contract;
        private readonly IPaginationService _pagination;

        public ContractController(IContract contract, IPaginationService pagination)
        {
            _contract = contract;
            _pagination = pagination;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateContract(CreateContract request)
        {
            var createContract = await _contract.CreateContract(request);

            if (createContract.isSuccess == true)
            {
                return Ok(createContract.Message);
            }
            else
            {
                return BadRequest(createContract.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateContract(string id, EditContract request)
        {
            var editContract = await _contract.EditContract(id, request);

            if (editContract.isSuccess == true)
            {
                return Ok(editContract.Message);
            }
            else
            {
                return BadRequest(editContract.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetContractById(string id)
        {
            var contract = await _contract.GetContractById(id);
            return Ok(contract);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContract([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _contract.GetListContract(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListContract>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
            return Ok(pagedReponse);
        }
    }
}
