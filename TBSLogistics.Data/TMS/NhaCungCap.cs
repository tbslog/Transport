using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class NhaCungCap
    {
        public NhaCungCap()
        {
            XeVanChuyen = new HashSet<XeVanChuyen>();
        }

        public string MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string LoaiDichVu { get; set; }
        public string MaSoThue { get; set; }
        public int MaDiaDiem { get; set; }
        public string LoaiNhaCungCap { get; set; }
        public string MaHopDong { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual HopDongVaPhuLuc MaHopDongNavigation { get; set; }
        public virtual ICollection<XeVanChuyen> XeVanChuyen { get; set; }
    }
}
