using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public  class ListHandling
    {
        public string MaVanDon { get; set; }
        public string PhanLoaiVanDon { get; set; }
        public long MaDieuPhoi { get; set; }
        public string MaSoXe { get; set; }
        public string TenTaiXe { get; set; }
        public string PTVanChuyen { get; set; }
        public string TenTau { get; set; }
        public string HangTau { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string DiemLayRong { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public DateTime? ThoiGianLayTraRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
        public string TrangThai { get; set; }
    }
}
