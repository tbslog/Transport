using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.CurrencyExchangeModel
{
	public class CreateExchangeRateModel
	{
		public string CurrencyCode { get; set; }
		public double? PriceBuy { get; set; }
		public double? PriceSell { get; set; }
		public double? PriceTransfer { get; set; }
		public DateTime CreatedTime { get; set; }
		public string Bank { get; set; }
		public double? PriceFix { get; set; }
		public string Note { get; set; }
	}
}
