using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungPheDuyetUngVien
    {
        public int Id { get; set; }
        public decimal LuongPheDuyet { get; set; }
        public int QuyetDinh { get; set; }
        public string LyDo { get; set; }
        public int TrangThai { get; set; }
        public int IdungVien { get; set; }
        public int IdnguoiDung { get; set; }
        public DateTime ThoiGianPheDuyet { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
