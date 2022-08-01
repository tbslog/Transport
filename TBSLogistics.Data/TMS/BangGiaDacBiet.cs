using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class BangGiaDacBiet
    {
        public string MaBangGiaDb { get; set; }
        public string TenBangGiaDb { get; set; }
        public double? Tan { get; set; }
        public double? Khoi { get; set; }
        public double? SoLuong { get; set; }
        public double? Km { get; set; }
        public double? HeSo { get; set; }
    }
}
