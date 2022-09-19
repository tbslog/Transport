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
            BangGiaCungDuong = new HashSet<BangGiaCungDuong>();
            PhuPhi = new HashSet<PhuPhi>();
            SanPhamDichVu = new HashSet<SanPhamDichVu>();
        }

        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string MaHopDong { get; set; }
        public double Km { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public int DiemLayRong { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<BangGiaCungDuong> BangGiaCungDuong { get; set; }
        public virtual ICollection<PhuPhi> PhuPhi { get; set; }
        public virtual ICollection<SanPhamDichVu> SanPhamDichVu { get; set; }
    }
}
