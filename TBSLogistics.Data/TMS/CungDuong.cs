using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class CungDuong
    {
        public CungDuong()
        {
            BangGia = new HashSet<BangGia>();
            PhuPhis = new HashSet<PhuPhi>();
        }

        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string MaHopDong { get; set; }
        public double Km { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public int DiemLayRong { get; set; }
        public string GhiChu { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<PhuPhi> PhuPhis { get; set; }
    }
}
