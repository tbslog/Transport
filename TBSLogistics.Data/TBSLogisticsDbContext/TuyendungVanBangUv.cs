using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungVanBangUv
    {
        public int Id { get; set; }
        public string TenTruong { get; set; }
        public string ChuyenNghanh { get; set; }
        public string NamTotNghiep { get; set; }
        public string BangCap { get; set; }
        public string XepLoai { get; set; }
        public int? IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
