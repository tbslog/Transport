using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class HopDongVaPhuLuc
    {
        public HopDongVaPhuLuc()
        {
            NhaCungCaps = new HashSet<NhaCungCap>();
        }

        public string MaHopDong { get; set; }
        public string ParentId { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public string MaPtvc { get; set; }
        public string PhanLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public int ThoiHanHopDong { get; set; }
        public string GhiChu { get; set; }
        public bool? PhuPhi { get; set; }
        public bool? PhanLoaiPhuLuc { get; set; }
        public string TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<NhaCungCap> NhaCungCaps { get; set; }
    }
}
