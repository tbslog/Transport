using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class TrongTaiXe
    {
        public int Id { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string DonViTrongTai { get; set; }
        public double TrongTaiToiDa { get; set; }
    }
}
