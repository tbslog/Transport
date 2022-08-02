using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SupplierModel
{
    public class UpdateSupplierRequest
    {
        public string TenNhaCungCap { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string LoaiDichVu { get; set; }
        public string MaSoThue { get; set; }
        public int MaDiaDiem { get; set; }
        public string LoaiNhaCungCap { get; set; }
        public string MaHopDong { get; set; }
    }
}
