using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.RoadModel
{
   public class ListRoadRequest
    {
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public string MaHopDong { get; set; }
        public double Km { get; set; }
        public string DiemDau { get; set; }
        public string DiemCuoi { get; set; }
        public string DiemLayRong { get; set; }
        public string PhanLoaiDiaDiem { get; set; }
        public string GhiChu { get; set; }
    }
}
