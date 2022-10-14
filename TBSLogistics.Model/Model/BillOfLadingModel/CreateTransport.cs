using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateTransport
    {
        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public string HangTau { get; set; }
        public string Tau { get; set; }
        public string CangChuyenTai { get; set; }
        public string CangDich { get; set; }
        public int TrangThai { get; set; }
        public DateTime NgayTaoDon { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<Handling> DieuPhoi { get; set; }
    }

    public class Handling
    {
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
    }
}
