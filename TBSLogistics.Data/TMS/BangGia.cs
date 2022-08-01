using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class BangGia
    {
        public BangGia()
        {
            KhachHangs = new HashSet<KhachHang>();
        }

        public string MaBangGia { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal GiaVnd { get; set; }
        public decimal GiaUsd { get; set; }
        public string MaDvt { get; set; }
        public int SoLuong { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPtvc { get; set; }

        public virtual CungDuong MaCungDuongNavigation { get; set; }
        public virtual ICollection<KhachHang> KhachHangs { get; set; }
    }
}
