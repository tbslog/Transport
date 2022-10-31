using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class SubFee
    {
        public SubFee()
        {
            SubFeePrice = new HashSet<SubFeePrice>();
        }

        public long SubFeeId { get; set; }
        public string SfName { get; set; }
        public long SfType { get; set; }
        public byte SfState { get; set; }
        public string Creator { get; set; }
        public string SfDescription { get; set; }

        public virtual SubFeeType SfTypeNavigation { get; set; }
        public virtual ICollection<SubFeePrice> SubFeePrice { get; set; }
    }
}
