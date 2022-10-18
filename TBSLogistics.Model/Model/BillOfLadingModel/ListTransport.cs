using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class ListTransport
    {
        public string MaVanDon { get; set; }
        public string LoaiVanDon { get; set; }
        public string DiemLayRong { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public int TongThungHang { get; set; }
        public double TongTheTich { get; set; }
        public double TongKhoiLuong { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
        public string TrangThai { get; set; }
        public int MaTrangThai { get; set; }
        public DateTime ThoiGianTaoDon { get; set; }
    }
}