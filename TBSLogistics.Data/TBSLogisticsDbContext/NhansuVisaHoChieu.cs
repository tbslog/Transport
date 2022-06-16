using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuVisaHoChieu
    {
        public int Id { get; set; }
        public string MaSo { get; set; }
        public string NgayLam { get; set; }
        public string NgayHetHan { get; set; }
        public string PhanLoai { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
