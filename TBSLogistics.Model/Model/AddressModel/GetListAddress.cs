using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AddressModel
{
   public class GetListAddress
    {
        public int MaDiaDiem { get; set; }
        public string TenDiaDiem { get; set; }
        public string LoaiDiaDiem { get; set; }
    }

    public class DataTempAddress
    {
        public int? MaDiaDiem { get; set; }
        public int? DiaDiemCha { get; set; }
    }
}
