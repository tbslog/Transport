using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SFeeByTcommand
{
    public class GetListSubFeeByHandling
    {
        public long Id { get; set; }
        public long IdTcommand { get; set; }
        public long SfId { get; set; }
        public long? SfPriceId { get; set; }
        public double Price { get; set; }
        public double FinalPrice { get; set; }
        public string ApproveStatus { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
