using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class BangGia
    {
        public string MaBangGia { get; set; }
        public string MaKh { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal GiaVnd { get; set; }
        public decimal GiaUsd { get; set; }
        public string MaDvt { get; set; }
        public int SoLuong { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPtvc { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual CungDuong MaCungDuongNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
    }
}
