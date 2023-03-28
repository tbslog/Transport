using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class ListTransport
    {
        public string MaVanDonKH { get; set; }
        public string AccountName { get; set; }
        public string HangTau { get; set; }
        public string MaPTVC { get; set; }
        public string MaVanDon { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string LoaiVanDon { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public int TongThungHang { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongSoKien { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
        public DateTime ThoiGianTaoDon { get; set; }
        public DateTime? ThoiGianTraRong { get; set; }
        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public string TrangThai { get; set; }
        public int MaTrangThai { get; set; }
    }
}