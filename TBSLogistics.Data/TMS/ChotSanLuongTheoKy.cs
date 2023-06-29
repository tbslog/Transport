using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ChotSanLuongTheoKy
    {
        public ChotSanLuongTheoKy()
        {
            PhuPhiTheoKy = new HashSet<PhuPhiTheoKy>();
        }

        public long Id { get; set; }
        public string MaKh { get; set; }
        public string MaLoaiKh { get; set; }
        public string MaAccount { get; set; }
        public long MaDieuPhoi { get; set; }
        public decimal DonGia { get; set; }
        public string MaLoaiTienTe { get; set; }
        public decimal DonGiaQuyDoi { get; set; }
        public int KyChot { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public DateTime? TimeBlock { get; set; }
        public string Blocker { get; set; }

        public virtual AccountOfCustomer MaAccountNavigation { get; set; }
        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<PhuPhiTheoKy> PhuPhiTheoKy { get; set; }
    }
}
