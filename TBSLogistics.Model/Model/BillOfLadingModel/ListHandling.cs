using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class ListHandling
    {
        public int ContNum { get; set; }
        public string MaChuyen { get; set; }
        public string DiemDau { get; set; }
        public string DiemCuoi { get; set; }
        public string MaVanDonKH { get; set; }
        public string MaKH { get; set; }
        public string DonViVanTai { get; set; }
        public string CungDuong { get; set; }
        public string SealHQ { get; set; }
        public string SealNP { get; set; }
        public string MaVanDon { get; set; }
        public string PhanLoaiVanDon { get; set; }
        public long? MaDieuPhoi { get; set; }
        public string MaSoXe { get; set; }
        public string TenTaiXe { get; set; }
        public string SoDienThoai { get; set; }
        public string MaPTVC { get; set; }
        public string PTVanChuyen { get; set; }
        public string TenTau { get; set; }
        public string HangTau { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string DiemLayRong { get; set; }
        public string DiemTraRong { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public double? SoKien { get; set; }
        public DateTime? ThoiGianTraRong { get; set; }
        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
        public DateTime? ThoiGianTaoDon { get; set; }
        public string TrangThai { get; set; }
        public int? statusId { get; set; }
    }
}
