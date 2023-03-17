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
        public string LoaiVanDon { get; set; }
        public string MaPTVC { get; set; }
        public string MaKH { get; set; }
        public string MaVanDon { get; set; }
        public string MaVanDonKH { get; set; }
        public string DiemDau { get; set; }
        public string DiemCuoi { get; set; }
        public double? TongKhoiLuong { get; set; }
        public double? TongTheTich { get; set; }
        public double? TongSoKien { get; set; }
        public DateTime? ThoiGianLayHang { get; set; }
        public DateTime? ThoiGianTraHang { get; set; }
    }
}
