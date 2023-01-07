using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class TaiXe
    {
        public TaiXe()
        {
            DieuPhoi = new HashSet<DieuPhoi>();
            XeVanChuyen = new HashSet<XeVanChuyen>();
        }

        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GhiChu { get; set; }
        public string MaNhaCungCap { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public bool TaiXeTbs { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual KhachHang MaNhaCungCapNavigation { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoi { get; set; }
        public virtual ICollection<XeVanChuyen> XeVanChuyen { get; set; }
    }
}
