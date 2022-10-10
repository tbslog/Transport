using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class VanDon
    {
        public string MaVanDon { get; set; }
        public string MaKh { get; set; }
        public int IdbangGia { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string MaRomooc { get; set; }
        public string MaDonHang { get; set; }
        public string ClpNo { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public string MaLoaiThungHang { get; set; }
        public string MaDonViVanTai { get; set; }
        public float TrongLuong { get; set; }
        public float TheTich { get; set; }
        public int DiemLayHang { get; set; }
        public int DiemNhapHang { get; set; }
        public int DiemGioHang { get; set; }
        public int DiemTraRong { get; set; }
        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public int ThoiGianHanLech { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime ThoiGianTraRong { get; set; }
        public string HangTau { get; set; }
        public string Tau { get; set; }
        public string CangChuyenTai { get; set; }
        public string CangDich { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTaoDon { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual BangGia IdbangGiaNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual Romooc MaRomoocNavigation { get; set; }
        public virtual XeVanChuyen MaSoXeNavigation { get; set; }
        public virtual TaiXe MaTaiXeNavigation { get; set; }
    }
}
