using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class LoadDataHandling
    {
        public List<NhaPhanPhoiSelect> ListNhaPhanPhoi { get; set; }
        public List<KhachHangSelect> ListKhachHang { get; set; }
        public List<DriverTransport> ListTaiXe { get; set; }
        public List<VehicleTransport> ListXeVanChuyen { get; set; }
        public List<RomoocTransport> ListRomooc { get; set; }
    }

    public class NhaPhanPhoiSelect
    {
        public string TenNPP { get; set; }
        public string MaNPP { get; set; }
    }
    public class KhachHangSelect
    {
        public string TenKH { get; set; }
        public string MaKH { get; set; }
    }
    public class DriverTransport
    {
        public string MaTaiXe { get; set; }
        public string TenTaiXe { get; set; }
    }
    public class VehicleTransport
    {
        public string MaSoXe { get; set; }
        public string MaLoaiPhuongTien { get; set; }
    }

    public class RomoocTransport
    {
        public string MaRomooc { get; set; }
        public string TenLoaiRomooc { get; set; }
    }

}
