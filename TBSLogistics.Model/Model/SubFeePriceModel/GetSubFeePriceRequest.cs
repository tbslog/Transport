
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubFeePriceModel
{
    public class GetSubFeePriceRequest
    {
        public string accountId { get; set; }
        public int? firstPlace { get; set; }
        public int? secondPlace { get; set; }
        public int? getEmptyPlace { get; set; }
        public string VehicleType { get; set; }
        public long PriceId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerId { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public string GoodsType { get; set; }
        public double Price { get; set; }
        public byte SfStateByContract { get; set; }
        public string Description { get; set; }
        public string Approver { get; set; }
        public string Creator { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
    }
}
