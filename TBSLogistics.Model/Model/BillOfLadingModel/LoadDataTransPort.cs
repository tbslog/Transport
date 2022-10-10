using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class LoadDataTransPort
    {
        public List<NhaPhanPhoiSelect> ListNhaPhanPhoi { get; set; }
        public List<KhachHangSelect> ListKhachHang { get; set; }
        public List<BangGiaVanDon> BangGiaVanDon { get; set; }
    }

    public class BangGiaVanDon
    {
        public string MaNPP { get; set; }
        public string PTVC { get; set; }
        public string DVT { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public decimal Price { get; set; }
        public string MaCungDuong { get; set; }
    }

    public class NhaPhanPhoiSelect
    {
        public string TenNPP { get; set; }
        public string MaNPP { get; set; }
    }
    public class KhachHangSelect
    {
        public string TenKH { get; set; }
        public string MaKH { get; set; }
    }

}
