using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ExchangeRate
    {
        public long Id { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public double? PriceBuy { get; set; }
        public double? PriceSell { get; set; }
        public double? PriceTransfer { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
