using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AddressModel
{
   public class GetAddressModel
    {
        public string TenKhuVuc { get; set; }
        public int? MaKhuVuc { get; set; }
        public int MaDiaDiem { get; set; }
        public string TenDiaDiem { get; set; }
        public string LoaiDiaDiem { get; set; }
        public string MaGps { get; set; }
        public string DiaChiDayDu { get; set; }
        public int MaTinh { get; set; }
        public int MaHuyen { get; set; }
        public int MaPhuong { get; set; }
        public string SoNha { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
