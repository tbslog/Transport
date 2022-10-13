using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ProductServiceModel
{

    public class EditProductServiceRequest
    {
        public int id { get; set; }
        public string MaHopDong { get; set; }
        public string MaPTVC { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal DonGia { get; set; }
        public string MaDVT { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaLoaiHopDong { get; set; }
        public DateTime NgayHetHieuLuc { get; set; }
    }
}
