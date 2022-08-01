using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            DoiTacs = new HashSet<DoiTac>();
            HopDongVaPhuLucs = new HashSet<HopDongVaPhuLuc>();
            VanDons = new HashSet<VanDon>();
        }

        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public int MaDiaDiem { get; set; }
        public string MaBangGia { get; set; }
        public DateTime Createdtime { get; set; }
        public DateTime UpdateTime { get; set; }

        public virtual BangGium MaBangGiaNavigation { get; set; }
        public virtual ICollection<DoiTac> DoiTacs { get; set; }
        public virtual ICollection<HopDongVaPhuLuc> HopDongVaPhuLucs { get; set; }
        public virtual ICollection<VanDon> VanDons { get; set; }
    }
}
