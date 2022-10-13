using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class DieuPhoi
    {
        public int Id { get; set; }
        public string MaVanDon { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string DonViVanTai { get; set; }
        public string MaKh { get; set; }
        public int IdbangGia { get; set; }
        public decimal GiaThamChieu { get; set; }
        public decimal GiaThucTe { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public double? TrongLuong { get; set; }
        public double? TheTich { get; set; }
        public string GhiChu { get; set; }
        public DateTime ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime ThoiGianKeoCong { get; set; }
        public DateTime ThoiGianHanLech { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime ThoiGianTraRong { get; set; }
        public DateTime? ThoiGianNhapHang { get; set; }
        public DateTime? ThoiGianXaHang { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }

        public virtual KhachHang DonViVanTaiNavigation { get; set; }
        public virtual BangGia IdbangGiaNavigation { get; set; }
        public virtual Romooc MaRomoocNavigation { get; set; }
        public virtual XeVanChuyen MaSoXeNavigation { get; set; }
        public virtual TaiXe MaTaiXeNavigation { get; set; }
        public virtual VanDon MaVanDonNavigation { get; set; }
    }
}
