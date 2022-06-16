using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuVanBang
    {
        public int Id { get; set; }
        public string MaBangCap { get; set; }
        public string TenBangCap { get; set; }
        public string NoiDaoTao { get; set; }
        public string NgonNgu { get; set; }
        public string ChuyenNghanh { get; set; }
        public string ThoiGianTotNghiep { get; set; }
        public string LoaiBangCap { get; set; }
        public string XepLoai { get; set; }
        public string PhanLoai { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
