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
        public string PhanLoaiVanDon { get; set; }
        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string NhaCungCap { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public string MaKh { get; set; }
        public string TenTau { get; set; }
        public string HangTau { get; set; }
        public long IdbangGia { get; set; }
        public decimal GiaThamChieu { get; set; }
        public decimal GiaThucTe { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public string GhiChu { get; set; }
        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime? ThoiGianTraRong { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
    }

    public class RoadDetail
    {
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string DiemLayRong { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
    }
}
