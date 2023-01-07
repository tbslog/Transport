using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class XeVanChuyen
    {
        public XeVanChuyen()
        {
            DieuPhoi = new HashSet<DieuPhoi>();
        }

        public string MaSoXe { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaTaiXeMacDinh { get; set; }
        public double? TrongTaiToiThieu { get; set; }
        public double? TrongTaiToiDa { get; set; }
        public string MaGps { get; set; }
        public string MaGpsmobile { get; set; }
        public string MaTaiSan { get; set; }
        public int? ThoiGianKhauHao { get; set; }
        public DateTime? NgayHoatDong { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual TaiXe MaTaiXeMacDinhNavigation { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoi { get; set; }
    }
}
