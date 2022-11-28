using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class GetHandling
    {
        public RoadDetail CungDuong { get; set; }
        public double TongKhoiLuong { get; set; }
        public double TongTheTich { get; set; }
        public string PhanLoaiVanDon { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPTVC { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaDVT { get; set; }
        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string DonViVanTai { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public string MaKh { get; set; }
        public string TenTau { get; set; }
        public string HangTau { get; set; }
        public long IdbangGia { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public double KhoiLuong { get; set; }
        public double TheTich { get; set; }
        public double SoKhoi { get; set; }
        public string GhiChu { get; set; }
        public string GhiChuVanDon { get; set; }
        public int? DiemLayRong { get; set; }
        public DateTime? ThoiGianLayTraRong { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }

        public DateTime? ThoiGianLayTraRongThucTe { get; set; }
        public DateTime? ThoiGianHaCangThucTe { get; set; }
        public DateTime? ThoiGianHanLenhThucTe { get; set; }
        public DateTime? ThoiGianCoMatThucTe { get; set; }
        public DateTime? ThoiGianLayHangThucTe { get; set; }
        public DateTime? ThoiGianTraHangThucTe { get; set; }
    }

    public class RoadDetail
    {
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public string DiemLayRong { get; set; }
    }
}
