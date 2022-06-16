using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuPhuongTienDiLai
    {
        public int MaNhanVien { get; set; }
        public string LoaiPhuongTien { get; set; }
        public string BienSo { get; set; }
        public string TenTuyenXe { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
