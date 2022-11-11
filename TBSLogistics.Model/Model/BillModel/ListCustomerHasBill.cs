using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
    public class ListCustomerHasBill
    {
        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public int TongVanDon { get; set; }
        public int TongSoChuyen { get; set; }
    }
}
