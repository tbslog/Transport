using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SubFee
    {
        public SubFee()
        {
            SfeeByTcommand = new HashSet<SfeeByTcommand>();
            SubFeePrice = new HashSet<SubFeePrice>();
        }

        public long SubFeeId { get; set; }
        public string SfName { get; set; }
        public long SfType { get; set; }
        /// <summary>
        /// 0: deactivated, 1: create new, 2: approved, 3: deleted
        /// </summary>
        public byte SfState { get; set; }
        public string SfDescription { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual SubFeeType SfTypeNavigation { get; set; }
        public virtual ICollection<SfeeByTcommand> SfeeByTcommand { get; set; }
        public virtual ICollection<SubFeePrice> SubFeePrice { get; set; }
    }
}
