using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class UpdateTransport
    {
        public string MaVanDonKH { get; set; }
        public string MaKh { get; set; }
        public string LoaiVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public int TongThungHang { get; set; }
        public double TongKhoiLuong { get; set; }
        public double TongTheTich { get; set; }
        public DateTime ThoiGianLayTraRong { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
    }
}
