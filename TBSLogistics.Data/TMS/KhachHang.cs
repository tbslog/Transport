using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            BangGia = new HashSet<BangGia>();
            HopDongVaPhuLucs = new HashSet<HopDongVaPhuLuc>();
            VanDons = new HashSet<VanDon>();
        }

        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public int MaDiaDiem { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<HopDongVaPhuLuc> HopDongVaPhuLucs { get; set; }
        public virtual ICollection<VanDon> VanDons { get; set; }
    }
}
