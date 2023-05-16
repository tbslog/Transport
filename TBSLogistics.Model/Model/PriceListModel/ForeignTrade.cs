using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
	public class ForeignTrade
	{
		public string CurrencyCode { get; set; }
		public string CurrencyName { get; set; }
		public string Buy { get; set; }
		public string Transfer { get; set; }
		public string Sell { get; set; }
	}
}
