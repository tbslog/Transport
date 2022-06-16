using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NhansuThongTinNhanVien
    {
        public NhansuThongTinNhanVien()
        {
            NhansuDiaChis = new HashSet<NhansuDiaChi>();
            NhansuKinhNghiemLamViecs = new HashSet<NhansuKinhNghiemLamViec>();
            NhansuKyNangChuyenMons = new HashSet<NhansuKyNangChuyenMon>();
            NhansuNguoiGioiThieus = new HashSet<NhansuNguoiGioiThieu>();
            NhansuThamChieus = new HashSet<NhansuThamChieu>();
            NhansuThongTinGiaDinhs = new HashSet<NhansuThongTinGiaDinh>();
            NhansuVanBangs = new HashSet<NhansuVanBang>();
            NhansuVisaHoChieus = new HashSet<NhansuVisaHoChieu>();
        }

        public int Id { get; set; }
        public string Hinhanh { get; set; }
        public string HoVaTen { get; set; }
        public string GioiTinh { get; set; }
        public string NgaySinh { get; set; }
        public string NoiSinh { get; set; }
        public string NgayBatDauLam { get; set; }
        public string QuocTich { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public string Cccd { get; set; }
        public string NgayCapCccd { get; set; }
        public string NoiCap { get; set; }
        public string TrinhDoHocVan { get; set; }
        public string HinhThucPhongVan { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string EmailCongTy { get; set; }
        public string SoAnSinhXaHoi { get; set; }
        public string MaSoThue { get; set; }
        public string TinhTrangHonNhan { get; set; }
        public string NgayVaoDoan { get; set; }
        public string NgayVaoDang { get; set; }
        public string NgayVaoCongDoan { get; set; }
        public string NgayPhongVan { get; set; }
        public string NgayVaoLam { get; set; }
        public int? ThoiGianHocViec { get; set; }
        public int? ThoiGianThuViec { get; set; }
        public string NgayKyHopDong1Nam { get; set; }
        public string NgayKyHopDong3Nam { get; set; }
        public string NgayKyHopDongVoThoiHan { get; set; }
        public string ThoiGianNghi { get; set; }
        public string TenNganHang { get; set; }
        public string SoTaiKhoanNh { get; set; }
        public string NguoiTiepNhan { get; set; }
        public int TrangThai { get; set; }
        public int HoSoDinhKem { get; set; }
        public int? MaUngVien { get; set; }
        public int IdnguoiDung { get; set; }

        public virtual NhansuBoPhanLienQuan NhansuBoPhanLienQuan { get; set; }
        public virtual NhansuPhuongTienDiLai NhansuPhuongTienDiLai { get; set; }
        public virtual NhansuSucKhoe NhansuSucKhoe { get; set; }
        public virtual ICollection<NhansuDiaChi> NhansuDiaChis { get; set; }
        public virtual ICollection<NhansuKinhNghiemLamViec> NhansuKinhNghiemLamViecs { get; set; }
        public virtual ICollection<NhansuKyNangChuyenMon> NhansuKyNangChuyenMons { get; set; }
        public virtual ICollection<NhansuNguoiGioiThieu> NhansuNguoiGioiThieus { get; set; }
        public virtual ICollection<NhansuThamChieu> NhansuThamChieus { get; set; }
        public virtual ICollection<NhansuThongTinGiaDinh> NhansuThongTinGiaDinhs { get; set; }
        public virtual ICollection<NhansuVanBang> NhansuVanBangs { get; set; }
        public virtual ICollection<NhansuVisaHoChieu> NhansuVisaHoChieus { get; set; }
    }
}
