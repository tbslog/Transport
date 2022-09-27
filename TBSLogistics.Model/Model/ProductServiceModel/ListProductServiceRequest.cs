using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ProductServiceModel
{
    public class ListProductServiceRequest
    {
        public string MaSPDV { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal DonGia { get; set; }
        public string MaDVT { get; set; }
        public int SoLuong { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPTVC { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
