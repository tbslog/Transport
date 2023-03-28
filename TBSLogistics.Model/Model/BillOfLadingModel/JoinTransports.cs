using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class JoinTransports
    {
        public List<string> TransportIds { get; set; }
        public string MaChuyen { get; set; }

    }

    public class LoadJoinTransports
    {
        public CreateHandlingLess handlingLess { get; set; }
        public string LoaiVanDon { get; set; }
        public string HangTau { get; set; }
        public string TenTau { get; set; }
        public string MaPTVC { get; set; }
        public List<LoadTransports> loadTransports { get; set; }
        public string MessageErrors { get; set; }
    }

    public class LoadTransports
    {
        public long? handlingId { get; set; }
        public int? ThuTuGiaoHang { get; set; }
        public string AccountId { get; set; }
        public string LoaiHangHoa { get; set; }
        public string SealHq { get; set; }
        public string SealNp { get; set; }
        public string ContNo { get; set; }
        public string LoaiVanDon { get; set; }
        public string MaPTVC { get; set; }
        public string AccountName { get; set; }
        public string MaKH { get; set; }
        public string MaVanDon { get; set; }
        public string MaVanDonKH { get; set; }
        public string DiemDau { get; set; }
        public string DiemCuoi { get; set; }
        public int? DiemLayRong { get; set; }
        public int? DiemTraRong { get; set; }
        public DateTime? TGHanLenh { get; set; }
        public DateTime? TGLayRong { get; set; }
        public DateTime? TGTraRong { get; set; }
        public DateTime? TGHaCang { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongSoKien { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
    }
}
