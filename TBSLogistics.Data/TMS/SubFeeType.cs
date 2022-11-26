using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SubFeeType
    {
        public SubFeeType()
        {
            SubFee = new HashSet<SubFee>();
        }

        public long SfTypeId { get; set; }
        public string SfTypeName { get; set; }

        public virtual ICollection<SubFee> SubFee { get; set; }
    }
}
