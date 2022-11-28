using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateHandling
    {
        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public List<Handling> DieuPhoi { get; set; }
    }

    public class Handling
    {
        public string MaSoXe { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPTVC { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaDVT { get; set; }
        public string MaTaiXe { get; set; }
        public string DonViVanTai { get; set; }
        public string TenTau { get; set; }
        public string HangTau { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public int? DiemLayTraRong { get; set; }
        public double KhoiLuong { get; set; }
        public double TheTich { get; set; }
        public string GhiChu { get; set; }
        public DateTime? ThoiGianLayTraRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
    }
}
