using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class TaiXe
    {
        public TaiXe()
        {
            VanDon = new HashSet<VanDon>();
            XeVanChuyen = new HashSet<XeVanChuyen>();
        }

        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GhiChu { get; set; }
        public string MaKh { get; set; }
        public string LoaiXe { get; set; }
        public bool TaiXeTbs { get; set; }
        public int? TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<VanDon> VanDon { get; set; }
        public virtual ICollection<XeVanChuyen> XeVanChuyen { get; set; }
    }
}
