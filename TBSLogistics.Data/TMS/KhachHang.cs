using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            HopDongVaPhuLuc = new HashSet<HopDongVaPhuLuc>();
            TaiXe = new HashSet<TaiXe>();
            VanDon = new HashSet<VanDon>();
        }

        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string MaNhomKh { get; set; }
        public string MaLoaiKh { get; set; }
        public int MaDiaDiem { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual ICollection<HopDongVaPhuLuc> HopDongVaPhuLuc { get; set; }
        public virtual ICollection<TaiXe> TaiXe { get; set; }
        public virtual ICollection<VanDon> VanDon { get; set; }
    }
}
