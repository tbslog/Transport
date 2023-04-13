using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class LogGps
    {
        public long Id { get; set; }
        public long MaDieuPhoi { get; set; }
        public int? DiemLayRong { get; set; }
        public int? DiemTraRong { get; set; }
        public int? DiemDau { get; set; }
        public int? DiemCuoi { get; set; }
        public string MaGps { get; set; }
        public int TrangThaiDp { get; set; }

        public virtual DiaDiem DiemCuoiNavigation { get; set; }
        public virtual DiaDiem DiemDauNavigation { get; set; }
        public virtual DiaDiem DiemLayRongNavigation { get; set; }
        public virtual DiaDiem DiemTraRongNavigation { get; set; }
        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
    }
}
