using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class SubFeeByContract
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public long MaDieuPhoi { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Creator { get; set; }

        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
        public virtual SubFeePrice Price { get; set; }
    }
}
