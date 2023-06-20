using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Model.CurrencyExchangeModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Bill;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.CurrencyExchange;
using TBSLogistics.Service.Services.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class CurrencyExchangeController : ControllerBase
	{
		private readonly ICurrencyExchange _iCurrencyExchange;
		private readonly IPaginationService _uriService;
		private readonly ICommon _common;

		public CurrencyExchangeController(ICurrencyExchange ICurrencyExchange, IPaginationService uriService, ICommon common)
		{
			_iCurrencyExchange = ICurrencyExchange;
			_common = common;
			_uriService = uriService;
		}

		[HttpGet]
		[Route("[action]")]
		public async Task<IActionResult> GetListExchangeRate([FromQuery] PaginationFilter filter)
		{
			var route = Request.Path.Value;
			var pagedData = await _iCurrencyExchange.GetListExchangeRate(filter);

			var pagedReponse = PaginationHelper.CreatePagedReponse<ExchangeRate>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
			return Ok(pagedReponse);
		}

		[HttpPost]
		[Route("[action]")]
		public async Task<IActionResult> CreateExchangeRate(List<CreateExchangeRateModel> request)
		{
			var create = await _iCurrencyExchange.CreateExchangeRate(request);

			if (create.isSuccess)
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
		public async Task<IActionResult> UpdateExchangeRate(int id, float priceFix, string note)
		{
			var update = await _iCurrencyExchange.UpdateExchangeRate(id, priceFix, note);

			if (update.isSuccess)
			{
				return Ok(update.Message);
			}
			else
			{
				return BadRequest(update.Message);
			}
		}
	}
}
