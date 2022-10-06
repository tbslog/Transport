using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.RoadModel
{
    public class ListPoint
    {
        public List<Point> DiemLayRong { get; set; }
        public List<Point> DiemDau { get; set; }
        public List<Point> DiemCuoi { get; set; }

        public List<Road> CungDuong { get; set; }
    }
    public class Point
    {
        public int MaDiaDiem { get; set; }
        public string TenDiaDiem { get; set; }
    }

    public class Road
    {
        public string MaCungDuong { get; set; }
        public string TenCungDuong { get; set; }
        public double KM { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }
        public int? DiemLayRong { get; set; }
    }

}
