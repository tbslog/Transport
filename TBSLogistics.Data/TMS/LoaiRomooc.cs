using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class LoaiRomooc
    {
        public LoaiRomooc()
        {
            Romoocs = new HashSet<Romooc>();
        }

        public string MaLoaiRomooc { get; set; }
        public string TenLoaiRomooc { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual ICollection<Romooc> Romoocs { get; set; }
    }
}
