using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuBoPhanLienQuan
    {
        public int MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public string TenBoPhan { get; set; }
        public string MaChucVu { get; set; }
        public string TenChucVu { get; set; }
        public string TinhChatCv { get; set; }
        public string CachTinhLuong { get; set; }
        public string NoiLamViec { get; set; }
        public string MaViTriCongViec { get; set; }
        public string TenViTriCongViec { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
