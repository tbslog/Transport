using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CurrencyExchangeModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.CurrencyExchange
{
	public interface ICurrencyExchange
	{
		Task<BoolActionResult> CreateExchangeRate(List<CreateExchangeRateModel> request);
		Task<BoolActionResult> UpdateExchangeRate(int id, float priceFix, string note);
		Task<double> GetPriceTradeNow(string priceCode, DateTime? dateTime = null, string bank = null);
		Task<PagedResponseCustom<ExchangeRate>> GetListExchangeRate(PaginationFilter filter);
		Task<BoolActionResult> GetPriceTrade();
	}
}
