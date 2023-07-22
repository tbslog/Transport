using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class KhachHang
    {
        public KhachHang()
        {
            ChotSanLuongTheoKy = new HashSet<ChotSanLuongTheoKy>();
            DieuPhoi = new HashSet<DieuPhoi>();
            HopDongVaPhuLuc = new HashSet<HopDongVaPhuLuc>();
            KhachHangAccount = new HashSet<KhachHangAccount>();
            PhuPhiNangHa = new HashSet<PhuPhiNangHa>();
            TaiXe = new HashSet<TaiXe>();
            UserHasCustomer = new HashSet<UserHasCustomer>();
            ValidateDataByCustomer = new HashSet<ValidateDataByCustomer>();
            XeVanChuyen = new HashSet<XeVanChuyen>();
        }

        public string MaKh { get; set; }
        public string TenKh { get; set; }
        public string Chuoi { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string MaNhomKh { get; set; }
        public string MaLoaiKh { get; set; }
        public int MaDiaDiem { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }
        public string TenRutGon { get; set; }

        public virtual ICollection<ChotSanLuongTheoKy> ChotSanLuongTheoKy { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoi { get; set; }
        public virtual ICollection<HopDongVaPhuLuc> HopDongVaPhuLuc { get; set; }
        public virtual ICollection<KhachHangAccount> KhachHangAccount { get; set; }
        public virtual ICollection<PhuPhiNangHa> PhuPhiNangHa { get; set; }
        public virtual ICollection<TaiXe> TaiXe { get; set; }
        public virtual ICollection<UserHasCustomer> UserHasCustomer { get; set; }
        public virtual ICollection<ValidateDataByCustomer> ValidateDataByCustomer { get; set; }
        public virtual ICollection<XeVanChuyen> XeVanChuyen { get; set; }
    }
}
