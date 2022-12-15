using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateHandling
    {
        public string MaVanDon { get; set; }
        public string MaCungDuong { get; set; }
        public List<Handling> DieuPhoi { get; set; }
    }

    public class Handling
    {
        public string MaPTVC { get; set; }
        public string MaKH { get; set; }
        public string DonViVanTai { get; set; }
        public string PTVanChuyen { get; set; }
        public string LoaiHangHoa { get; set; }
        public string DonViTinh { get; set; }
        public int? DiemLayTraRong { get; set; }
        public double KhoiLuong { get; set; }
        public double TheTich { get; set; }
        public double SoKien { get; set; }
    }
}
