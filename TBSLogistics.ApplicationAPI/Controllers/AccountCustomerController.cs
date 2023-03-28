using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.AccountModel;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AccountManager;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountCustomerController : ControllerBase
    {
        private readonly IAccount _account;
        private readonly IPaginationService _uriService;
        private readonly ICommon _common;

        public AccountCustomerController(IAccount account, IPaginationService uriService, ICommon common)
        {
            _common = common;
            _account = account;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateAccountCus(CreateOrUpdateAccount request)
        {
            var CreateAddress = await _account.CreateAccount(request);

            if (CreateAddress.isSuccess == true)
            {
                return Ok(CreateAddress.Message);
            }
            else
            {
                return BadRequest(CreateAddress.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateAccountCus(string accountId, CreateOrUpdateAccount request)
        {
            var CreateAddress = await _account.UpdateAccount(accountId, request);

            if (CreateAddress.isSuccess == true)
            {
                return Ok(CreateAddress.Message);
            }
            else
            {
                return BadRequest(CreateAddress.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAccountById(string accountId)
        {
            var getData = await _account.GetAccountById(accountId);
            return Ok(getData);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListAccountSelectByCus(string accountId)
        {
            var listSeelect = await _account.GetListAccountSelectByCus(accountId);
            return Ok(listSeelect);
        }

    }
}
