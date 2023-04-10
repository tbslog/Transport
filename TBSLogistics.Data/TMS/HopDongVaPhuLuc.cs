using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class HopDongVaPhuLuc
    {
        public HopDongVaPhuLuc()
        {
            BangGia = new HashSet<BangGia>();
            InverseMaHopDongChaNavigation = new HashSet<HopDongVaPhuLuc>();
            SubFeePrice = new HashSet<SubFeePrice>();
            TepHopDong = new HashSet<TepHopDong>();
        }

        public string MaHopDong { get; set; }
        public string MaHopDongCha { get; set; }
        public string Account { get; set; }
        public string LoaiHinhHopTac { get; set; }
        public string MaLoaiSpdv { get; set; }
        public string MaLoaiHinh { get; set; }
        public string HinhThucThue { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public string MaLoaiHopDong { get; set; }
        public int? NgayThanhToan { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual HopDongVaPhuLuc MaHopDongChaNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<HopDongVaPhuLuc> InverseMaHopDongChaNavigation { get; set; }
        public virtual ICollection<SubFeePrice> SubFeePrice { get; set; }
        public virtual ICollection<TepHopDong> TepHopDong { get; set; }
    }
}
