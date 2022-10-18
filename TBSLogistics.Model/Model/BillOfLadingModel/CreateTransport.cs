using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateTransport
    {
        public string LoaiVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public int TongThungHang { get; set; }
        public double TongKhoiLuong { get; set; }
        public double TongTheTich { get; set; }

        public string HangTau { get; set; }
        public string TenTau { get; set; }
        public string CangDich { get; set; }
        public string CangChuyenTai { get; set; }

        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
    }
}
