using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.CustomerModel
{
   public class ListCustommerRequest
    {
        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public int MaDiaDiem { get; set; }
        public string DiaDiem { get; set; }
        public DateTime Createdtime { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
