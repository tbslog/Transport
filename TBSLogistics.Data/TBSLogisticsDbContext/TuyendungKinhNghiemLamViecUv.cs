using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungKinhNghiemLamViecUv
    {
        public int Id { get; set; }
        public string TenCongTy { get; set; }
        public string DiaChiCongTy { get; set; }
        public string NghanhNgheKd { get; set; }
        public int? SoLuongNhanVien { get; set; }
        public string ThoiGianCongTac { get; set; }
        public string ChucVu { get; set; }
        public string HoTenCapTrenTrucTiep { get; set; }
        public string ChucVuCapTren { get; set; }
        public string LyDoNghiViec { get; set; }
        public string MucLuongCuoi { get; set; }
        public int? IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
