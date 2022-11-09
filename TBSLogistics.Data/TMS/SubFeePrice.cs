using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class SubFeePrice
    {
        public SubFeePrice()
        {
            SfeeByTcommand = new HashSet<SfeeByTcommand>();
        }

        public long PriceId { get; set; }
        public string ContractId { get; set; }
        public long SfId { get; set; }
        public string GoodsType { get; set; }
        public int? FirstPlace { get; set; }
        public int? SecondPlace { get; set; }
        public double UnitPrice { get; set; }
        public byte SfStateByContract { get; set; }
        public string Description { get; set; }
        public string Approver { get; set; }
        public int Status { get; set; }
        public string Creator { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? DeactiveDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual HopDongVaPhuLuc Contract { get; set; }
        public virtual SubFee Sf { get; set; }
        public virtual ICollection<SfeeByTcommand> SfeeByTcommand { get; set; }
    }
}
