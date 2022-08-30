using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class PhuPhi
    {
        public string MaPhuPhi { get; set; }
        public string MaLoaiPhuPhi { get; set; }
        public DateTime NgayHieuLuc { get; set; }
        public int ThoiGianHieuLuc { get; set; }
        public decimal GiaUsd { get; set; }
        public decimal GiaVnd { get; set; }
        public int SoLuong { get; set; }
        public string MaCungDuong { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPtvc { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual CungDuong MaCungDuongNavigation { get; set; }
    }
}
