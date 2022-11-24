using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
    public class ListBillHandling
    {
        public long MaChuyen { get; set; }
        public string MaVanDon { get; set; }
        public string LoaiHangHoa { get; set; }
        public string LoaiPhuongTien { get; set; }
        public string MaKh { get; set; }
        public string MaNcc { get; set; }
        public string DonViVanTai { get; set; }
        public string KhachHang { get; set; }
        public decimal? DonGiaKH { get; set; }
        public decimal? DonGiaNCC { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal LoiNhuan { get; set; }
        public decimal ChiPhiHopDong { get; set; }
        public decimal ChiPhiPhatSinh { get; set; }

    }
}
