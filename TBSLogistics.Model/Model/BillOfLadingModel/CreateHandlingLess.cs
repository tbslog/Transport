using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class CreateHandlingLess
    {
        public string VehicleId { get; set; }
        public List<arrDataHandlingLess> arrDataHandlingLess { get; set; }
        public string MaSoXe { get; set; }
        public string MaDVT { get; set; }
        public string MaTaiXe { get; set; }
        public string DonViVanTai { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public int? DiemLayTraRong { get; set; }

        public DateTime? ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime? ThoiGianHaCang { get; set; }
        public DateTime? ThoiGianLayTraRong { get; set; }
    }

    public class arrDataHandlingLess
    {
        public string TransportId { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string GhiChu { get; set; }
    }
}
