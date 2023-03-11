using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.MobileModel
{
    public class GetDataTransportMobile
    {
        public string BookingNo { get; set; }
        public string LoaiVanDon { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public string CungDuong { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongSoKien { get; set; }
        public string HangTau { get; set; }
        public string TenTau { get; set; }
        public string GhiChu { get; set; }
        public string MaPTVC { get; set; }

        public List<GetDataHandlingMobile> getDataHandlingMobiles { get; set; }

        public DateTime? ThoiGianLayTraRong { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
    }

    public class GetDataHandlingMobile
    {
        public string LoaiPhuongTien { get; set; }
        public long HandlingId { get; set; }
        public string MaChuyen { get; set; }
        public string ContNo { get; set; }
        public string DiemLayTraRong { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public double? SoKien { get; set; }
        public string TrangThai { get; set; }
        public int MaTrangThai { get; set; }
    }
}
