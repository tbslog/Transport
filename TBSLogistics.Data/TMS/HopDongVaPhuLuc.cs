using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class HopDongVaPhuLuc
    {
        public HopDongVaPhuLuc()
        {
            BangGia = new HashSet<BangGia>();
        }

        public string MaHopDong { get; set; }
        public string SoHopDongCha { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public string MaPtvc { get; set; }
        public string MaLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public decimal? PhuPhi { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<BangGia> BangGia { get; set; }
    }
}
