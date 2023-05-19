using System;
using System.Collections.Generic;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateHandlingLess
    {
        public List<arrTransport> arrTransports { get; set; }
        public string PTVanChuyen { get; set; }
        public string DonViVanTai { get; set; }
        public string XeVanChuyen { get; set; }
        public string TaiXe { get; set; }
        public string GhiChu { get; set; }
        public string Romooc { get; set; }
    }

    public class UpdateHandlingLess
    {
        public List<arrTransport> arrTransports { get; set; }
        public string PTVanChuyen { get; set; }
        public string DonViVanTai { get; set; }
        public string XeVanChuyen { get; set; }
        public string TaiXe { get; set; }
        public string GhiChu { get; set; }
        public string Romooc { get; set; }
    }

    public class arrTransport
    {
        public long? MaDieuPhoi { get; set; }
        public int? ThuTuGiaoHang { get; set; }
        public string SealHQ { get; set; }
        public string SealNP { get; set; }
        public string ContNo { get; set; }
        public int? DiemTraRong { get; set; }
        public int? DiemLayRong { get; set; }
        public string MaVanDon { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaDVT { get; set; }
        public DateTime? TGLayHang { get; set; }
        public DateTime? TGTraHang { get; set; }
        public DateTime? TGTraRong { get; set; }
        public DateTime? TGLayRong { get; set; }
        public DateTime? TGHanLenh { get; set; }
        public DateTime? TGHaCang { get; set; }
    }
}
