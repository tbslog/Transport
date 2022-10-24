﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class UpdateSubFeePriceRequest
    {
        public long PriceId { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public byte? GoodsType { get; set; }
        public int? FirstPlace { get; set; }
        public int? SecondPlace { get; set; }
        public double UnitPrice { get; set; }
        public string Description { get; set; }
    }
}
