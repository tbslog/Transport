using System;
using System.Diagnostics.Contracts;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class UpdateHandling
    {
        public string DonViVanTai { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public string DonViTinh { get; set; }
        public int? DiemTraRong { get; set; }
        public int? DiemLayRong { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public bool ReuseCont { get; set; }
        public string SealHq { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public double? SoKien { get; set; }
        public string GhiChu { get; set; }

        public DateTime? ThoiGianLayRongThucTe { get; set; }
        public DateTime? ThoiGianTraRongThucTe { get; set; }
        public DateTime? ThoiGianCoMatThucTe { get; set; }
        public DateTime? ThoiGianHaCangThucTe { get; set; }

        public DateTime? ThoiGianLayHangThucTe { get; set; }
        public DateTime? ThoiGianTraHangThucTe { get; set; }
    }
}
