using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class PhuPhiTheoKy
    {
        public long Id { get; set; }
        public long IdsanLuongKy { get; set; }
        public long? IdphuPhiHopDong { get; set; }
        public long? IdphuPhiPhatSinh { get; set; }
        public decimal DonGia { get; set; }
        public string MaLoaiTienTe { get; set; }
        public decimal DonGiaQuyDoi { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }

        public virtual ChotSanLuongTheoKy IdsanLuongKyNavigation { get; set; }
    }
}
