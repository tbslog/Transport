using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuKyNangChuyenMon
    {
        public int Id { get; set; }
        public string TenKyNang { get; set; }
        public int ThoiGianHuanLuyen { get; set; }
        public string DonViHuanLuyen { get; set; }
        public string XepLoai { get; set; }
        public string NoiDung { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
