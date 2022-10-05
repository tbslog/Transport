using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class SubFee
    {
        public long SubFeeId { get; set; }
        public string SfName { get; set; }
        public byte SfType { get; set; }
        public byte SfState { get; set; }
        public string Creator { get; set; }
        public string SfDescription { get; set; }
    }
}
