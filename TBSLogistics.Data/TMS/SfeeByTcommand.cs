using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class SfeeByTcommand
    {
        public long Id { get; set; }
        public long IdTcommand { get; set; }
        public long SfId { get; set; }
        public long? SfPriceId { get; set; }
        public double Price { get; set; }
        public double FinalPrice { get; set; }
        public int ApproveStatus { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public virtual DieuPhoi IdTcommandNavigation { get; set; }
        public virtual SubFee Sf { get; set; }
        public virtual SubFeePrice SfPrice { get; set; }
    }
}
