using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class Romooc
    {
        public Romooc()
        {
            DieuPhoi = new HashSet<DieuPhoi>();
        }

        public string MaRomooc { get; set; }
        public string KetCauSan { get; set; }
        public string SoGuRomooc { get; set; }
        public string ThongSoKyThuat { get; set; }
        public string MaLoaiRomooc { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual LoaiRomooc MaLoaiRomoocNavigation { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoi { get; set; }
    }
}
