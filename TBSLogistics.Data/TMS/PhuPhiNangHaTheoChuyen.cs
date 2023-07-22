using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class PhuPhiNangHaTheoChuyen
    {
        public long Id { get; set; }
        public long MaPhuPhiNangHa { get; set; }
        public long MaDieuPhoi { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime Createdtime { get; set; }
        public string Creator { get; set; }

        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
        public virtual PhuPhiNangHa MaPhuPhiNangHaNavigation { get; set; }
    }
}
