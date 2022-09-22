using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
    public class CreatePriceListRequest
    {
        public string MaHopDong { get; set; }
        public string MaKH { get; set; }
        public string MaPtvc { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal DonGia { get; set; }
        public string MaDvt { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaLoaiHopDong { get; set; }
        public DateTime NgayApDung { get; set; }
        public int TrangThai { get; set; }
    }
}
