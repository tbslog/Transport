using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class ListSubFeePriceRequest
    {
        public long PriceId { get; set; }
        public string CustomerName { get;set; }
        public string ContractId { get; set; }
        public string ContractName { get; set; }
        public string GoodsType { get; set; }
        public string AreaName { get; set; }
        public string TripName { get; set; }
        public string sfName { get; set; }
        public string Status { get; set; }
        public double UnitPrice { get; set; }
        public byte SfStateByContract { get; set; }
        public string Approver { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
    }
}
