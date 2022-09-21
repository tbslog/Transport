using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
   public class GetPriceListRequest
    {
        public string MaHopDong { get; set; }
        public string MaKh { get; set; }
        public string MaCungDuong { get; set; }
        public DateTime NgayApDung { get; set; }
        public decimal GiaVND { get; set; }
        public decimal GiaUSD { get; set; }
        public int SoLuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaDVT { get; set; }
        public string MaPTVC { get; set; }
        public int TrangThai { get; set; }
    }
}
