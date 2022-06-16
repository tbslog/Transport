using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuNguoiGioiThieu
    {
        public int Id { get; set; }
        public string TenNguoiGioiThieu { get; set; }
        public string SoThe { get; set; }
        public string QuanHe { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
