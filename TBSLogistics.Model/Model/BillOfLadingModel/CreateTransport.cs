using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateTransport
    {
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public string AccountId { get; set; }
        public string LoaiVanDon { get; set; }
        public string MaKH { get; set; }
        public string MaVanDonKH { get; set; }
        public string HangTau { get; set; }
        public string TenTau { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongSoKien { get; set; }
        public int TongThungHang { get; set; }
        public string GhiChu { get; set; }
        public string MaPTVC { get; set; }

        public List<arrHandling> arrHandlings { get; set; }

        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
        public DateTime? ThoiGianTraRong { get; set; }
        public DateTime? ThoiGianLayRong { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
    }

    public class arrHandling
    {
        public long? MaDieuPhoi { get; set; }   
        public string GhiChu { get; set; }
        public string ContNo { get; set; }
        public string DonViVanTai { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public string DonViTinh { get; set; }
		public bool ReuseCont { get; set; }
		public int? DiemTraRong { get; set; }
        public int? DiemLayRong { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public double? SoKien { get; set; }
    }
}
