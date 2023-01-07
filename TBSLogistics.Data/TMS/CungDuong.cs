using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class CungDuong
    {
        public CungDuong()
        {
            BangGia = new HashSet<BangGia>();
        }

        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public double Km { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }

        public virtual ICollection<BangGia> BangGia { get; set; }
    }
}
