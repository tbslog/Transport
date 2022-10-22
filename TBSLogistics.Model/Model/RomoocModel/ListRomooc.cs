using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.RomoocModel
{
    public class ListRomooc
    {
        public string MaRomooc { get; set; }
        public string KetCauSan { get; set; }
        public string SoGuRomooc { get; set; }
        public string ThongSoKyThuat { get; set; }
        public string MaLoaiRomooc { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }
    }
}
