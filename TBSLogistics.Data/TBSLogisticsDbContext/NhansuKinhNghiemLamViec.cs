using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuKinhNghiemLamViec
    {
        public int Id { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChiCongTy { get; set; }
        public string ThoiGianCongTac { get; set; }
        public string ChucVu { get; set; }
        public string MucLuongCuoi { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
