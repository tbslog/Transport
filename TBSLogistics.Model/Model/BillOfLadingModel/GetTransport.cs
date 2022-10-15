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
        public string LoaiVanDon { get; set; }
        public string DiemLayRong { get; set; }
        public string DiemLayHang { get; set; }
        public string DiemTraHang { get; set; }
        public string CungDuong { get; set; }
        public int TongThungHang { get; set; }
        public double TongTrongLuong { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
        public DateTime ThoiGianTaoDon { get; set; }
    }
}
