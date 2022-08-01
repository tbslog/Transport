using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class TaiXe
    {
        public TaiXe()
        {
            VanDons = new HashSet<VanDon>();
            XeVanChuyens = new HashSet<XeVanChuyen>();
        }

        public string MaTaiXe { get; set; }
        public string Cccd { get; set; }
        public string HoVaTen { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GhiChu { get; set; }
        public string MaNhaThau { get; set; }
        public string LoaiXe { get; set; }
        public string PhanLoaiTaiXe { get; set; }
        public string TrangThai { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual ICollection<VanDon> VanDons { get; set; }
        public virtual ICollection<XeVanChuyen> XeVanChuyens { get; set; }
    }
}
