using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class GetTransport
    {
        public string MaVanDon { get; set; }
        public string AccountId { get; set; }
        public string MaKh { get; set; }
        public string MaVanDonKH { get; set; }
        public string LoaiThungHang { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public string LoaiVanDon { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public string CungDuong { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongSoKien { get; set; }
        public int TongThungHang { get; set; }
        public string HangTau { get; set; }
        public string TenTau { get; set; }
        public string GhiChu { get; set; }
        public string MaPTVC { get; set; }

        public List<arrHandling> arrHandlings { get; set; }

        public DateTime ThoiGianTaoDon { get; set; }

        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianTraRong { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
    }
}
