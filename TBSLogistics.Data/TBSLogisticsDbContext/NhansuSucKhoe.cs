using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuSucKhoe
    {
        public int MaNhanVien { get; set; }
        public int ChieuCao { get; set; }
        public int CanNang { get; set; }
        public string TheTrang { get; set; }
        public string GhiChu { get; set; }
        public string NgayKhamSucKhoe { get; set; }
        public string MaBenhVien { get; set; }
        public string TenBenhVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
