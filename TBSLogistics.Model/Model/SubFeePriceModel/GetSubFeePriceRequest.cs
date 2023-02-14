﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class GetSubFeePriceRequest
    {
        public long PriceId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerId { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public string GoodsType { get; set; }
        public int? AreaID { get; set; }
        public string TripID { get; set; }
        public double Price { get; set; }
        public byte SfStateByContract { get; set; }
        public string Description { get; set; }
        public string Approver { get; set; }
        public string Creator { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
    }
}