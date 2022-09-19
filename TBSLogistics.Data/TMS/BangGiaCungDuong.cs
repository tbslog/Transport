using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class BangGiaCungDuong
    {
        public int Id { get; set; }
        public string MaBangGia { get; set; }
        public string MaPtvc { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal GiaVnd { get; set; }
        public decimal GiaUsd { get; set; }
        public string MaDvt { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual CungDuong MaCungDuongNavigation { get; set; }
    }
}
