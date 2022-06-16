using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungKhoaHuanLuyenUv
    {
        public int Id { get; set; }
        public string DonViToChuc { get; set; }
        public string TenKhoaHoc { get; set; }
        public string ThoiGianHoc { get; set; }
        public string ChungChi { get; set; }
        public int? IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
