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
        public string Bank { get; set; }
        public double? PriceFix { get; set; }
        public string Note { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }
        public DateTime? DateTransfer { get; set; }
    }
}
