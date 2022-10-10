using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class VanDon
    {
        public VanDon()
        {
            DieuPhoi = new HashSet<DieuPhoi>();
        }

        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public string HangTau { get; set; }
        public string Tau { get; set; }
        public string CangChuyenTai { get; set; }
        public string CangDich { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTaoDon { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual ICollection<DieuPhoi> DieuPhoi { get; set; }
    }
}
