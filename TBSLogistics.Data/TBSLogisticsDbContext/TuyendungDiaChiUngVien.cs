using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungDiaChiUngVien
    {
        public int Id { get; set; }
        public string DiaChi { get; set; }
        public int? MaXaPhuong { get; set; }
        public string XaPhuong { get; set; }
        public int? MaQuanHuyen { get; set; }
        public string QuanHuyen { get; set; }
        public int? MaTinhThanh { get; set; }
        public string TinhThanhPho { get; set; }
        public int? PhanLoaiCho { get; set; }
        public int? PhanLoaiDiaChi { get; set; }
        public int IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
