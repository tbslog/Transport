using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class XeVanChuyen
    {
        public XeVanChuyen()
        {
            VanDons = new HashSet<VanDon>();
        }

        public string MaSoXe { get; set; }
        public string MaNhaCungCap { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaTaiXeMacDinh { get; set; }
        public double TrongTaiToiThieu { get; set; }
        public double TrongTaiToiDa { get; set; }
        public string MaGps { get; set; }
        public string MaGpsmobile { get; set; }
        public string LoaiVanHanh { get; set; }
        public string MaTaiSan { get; set; }
        public int? ThoiGianKhauHao { get; set; }
        public DateTime? NgayHoatDong { get; set; }
        public string PhanLoaiXeVanChuyen { get; set; }
        public string TrangThai { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual NhaCungCap MaNhaCungCapNavigation { get; set; }
        public virtual TaiXe MaTaiXeMacDinhNavigation { get; set; }
        public virtual ICollection<VanDon> VanDons { get; set; }
    }
}
