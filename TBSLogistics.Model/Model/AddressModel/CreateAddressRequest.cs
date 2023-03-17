using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AddressModel
{
    public class CreateAddressRequest
    {
        public int? DiaDiemCha { get; set; }
        public string TenDiaDiem { get; set; }
        public int MaQuocGia { get; set; }
        public int? MaTinh { get; set; }
        public int? MaHuyen { get; set; }
        public int? MaPhuong { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string MaGps { get; set; }
        public string PhanLoaiLoaiDiaDiem { get; set; }
    }
}
