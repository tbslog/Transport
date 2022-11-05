using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class BoPhan
    {
        public int Id { get; set; }
        public string MaBoPhan { get; set; }
        public string TenBoPhan { get; set; }
        public string MoTaCongViec { get; set; }
    }
}
