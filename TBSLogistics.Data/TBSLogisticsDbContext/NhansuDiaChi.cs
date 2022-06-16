using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuDiaChi
    {
        public int Id { get; set; }
        public string DiaChi { get; set; }
        public int? MaXaPhuong { get; set; }
        public string XaPhuong { get; set; }
        public int? MaQuanHuyen { get; set; }
        public string QuanHuyen { get; set; }
        public int? MaTinhThanh { get; set; }
        public string TinhThanhPho { get; set; }
        public string PhanLoaiCho { get; set; }
        public string PhanLoaiDiaChi { get; set; }
        public int MaNhanVien { get; set; }

        public virtual NhansuThongTinNhanVien MaNhanVienNavigation { get; set; }
    }
}
