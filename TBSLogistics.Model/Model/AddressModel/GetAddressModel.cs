using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AddressModel
{
   public class GetAddressModel
    {
        public int MaDiaDiem { get; set; }
        public string TenDiaDiem { get; set; }
        public int? maquocgia { get; set; }
        public int matinh { get; set; }
        public int mahuyen { get; set; }
        public int maphuong { get; set; }
        public string sonha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string MaGps { get; set; }
        public string MaLoaiDiaDiem { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
