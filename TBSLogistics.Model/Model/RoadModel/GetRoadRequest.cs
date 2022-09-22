using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.RoadModel
{
  public  class GetRoadRequest
    {
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string MaHopDong { get; set; }
        public double Km { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public int? DiemLayRong { get; set; }
        public string GhiChu { get; set; }
    }
}
