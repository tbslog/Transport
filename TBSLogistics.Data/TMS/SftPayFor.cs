using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SftPayFor
    {
        public SftPayFor()
        {
            SfeeByTcommand = new HashSet<SfeeByTcommand>();
        }

        public int PayForId { get; set; }
        public string PfName { get; set; }
        public string PfDescription { get; set; }

        public virtual ICollection<SfeeByTcommand> SfeeByTcommand { get; set; }
    }
}
