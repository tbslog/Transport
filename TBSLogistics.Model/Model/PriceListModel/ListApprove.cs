using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
    public class ListApprove
    {
        public long Id { get; set; }
        public string KhuVuc { get; set; }
        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaHopDong { get; set; }
        public string TenHopDong { get; set; }
        public string PTVC { get; set; }
        public string MaCungDuong { get; set; }
        public decimal DonGia { get; set; }
        public string TenCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string DVT { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaLoaiDoiTac { get; set; }
        public string NgayApDung { get; set; }
        public string NgayHetHieuLuc { get; set; }
        public string ThoiGianTao { get; set; }
    }
}
