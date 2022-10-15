using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public  class UpdateTransport
    {
        public string MaCungDuong { get; set; }
        public int TongThungHang { get; set; }
        public int TongKhoiLuong { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
    }
}
