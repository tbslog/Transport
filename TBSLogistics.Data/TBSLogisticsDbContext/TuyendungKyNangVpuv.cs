using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungKyNangVpuv
    {
        public int Id { get; set; }
        public string ChuongTrinh { get; set; }
        public string XepLoai { get; set; }
        public string VanBang { get; set; }
        public string TbsdanhGia { get; set; }
        public int? IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
