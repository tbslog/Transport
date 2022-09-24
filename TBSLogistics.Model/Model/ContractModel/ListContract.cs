using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ContractModel
{
    public class ListContract
    {
        public string MaHopDong { get; set; }
        public string MaBangGia { get; set; }
        public string TenHienThi { get; set; }
        public string SoHopDongCha { get; set; }
        public string MaKh { get; set; }
        public string TenKH { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public int TrangThai { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
    }
}
