using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class EditBillOfLadingRequest
    {
        public DateTime NgayNhapHang { get; set; }
        public string MaKh { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string MaRomooc { get; set; }
        public string MaPtvc { get; set; }
        public string Booking { get; set; }
        public string ClpNo { get; set; }
        public string ContNo { get; set; }
        public string SealHt { get; set; }
        public string SealHq { get; set; }
        public string MaLoaiThungHang { get; set; }
        public string MaDonViVanTai { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public double TrongLuong { get; set; }
        public double TheTich { get; set; }
        public int MaDvt { get; set; }
        public int DiemLayRong { get; set; }
        public int DiemLayHang { get; set; }
        public int DiemNhapHang { get; set; }
        public int DiemGioHang { get; set; }
        public int DiemTraRong { get; set; }
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
    }
}
