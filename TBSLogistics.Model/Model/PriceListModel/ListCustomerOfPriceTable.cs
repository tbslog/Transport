using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
    public class ListCustomerOfPriceTable
    {
        public string TenChuoi { get; set; }
        public string MaKH { get; set; }
        public string TenKH { get; set; }
        public string MaSoThue { get; set; }
        public string SoDienThoai { get; set; }
        public List<ListContractOfCustomer> listContractOfCustomers { get; set; }
    }

    public class ListContractOfCustomer
    {
        public string MaHopDong { get; set; }
        public string MaKH { get; set; }
        public string TenHopDong { get; set; }
        public string LoaiHinhHopTac { get; set; }
        public string SanPhamDichVu { get; set; }
    }
}
