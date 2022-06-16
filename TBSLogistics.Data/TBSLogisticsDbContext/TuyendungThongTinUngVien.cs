using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungThongTinUngVien
    {
        public TuyendungThongTinUngVien()
        {
            TuyendungDanhGiaUvs = new HashSet<TuyendungDanhGiaUv>();
            TuyendungDiaChiUngViens = new HashSet<TuyendungDiaChiUngVien>();
            TuyendungKhoaHuanLuyenUvs = new HashSet<TuyendungKhoaHuanLuyenUv>();
            TuyendungKinhNghiemLamViecUvs = new HashSet<TuyendungKinhNghiemLamViecUv>();
            TuyendungKyNangVpuvs = new HashSet<TuyendungKyNangVpuv>();
            TuyendungNgoaiNguUvs = new HashSet<TuyendungNgoaiNguUv>();
            TuyendungPheDuyetUngViens = new HashSet<TuyendungPheDuyetUngVien>();
            TuyendungThamChieuUvs = new HashSet<TuyendungThamChieuUv>();
            TuyendungThongTinGiaDinhUvs = new HashSet<TuyendungThongTinGiaDinhUv>();
            TuyendungThongTinKhacUvs = new HashSet<TuyendungThongTinKhacUv>();
            TuyendungVanBangUvs = new HashSet<TuyendungVanBangUv>();
        }

        public int Id { get; set; }
        public string Hinhanh { get; set; }
        public string Cv { get; set; }
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
        public string Sdt { get; set; }
        public string Email { get; set; }
        public string TinhTrangHonNhan { get; set; }
        public string TrinhDoHocVan { get; set; }
        public string HinhThucPhongVan { get; set; }
        public string NguoiTiepNhan { get; set; }
        public DateTime ThoiGian { get; set; }
        public int TrangThai { get; set; }
        public int HoSoDinhKem { get; set; }
        public int IdnguoiDung { get; set; }
        public int IdviTriTuyenDung { get; set; }
        public int IdyeuCau { get; set; }
        public decimal LuongYeuCau { get; set; }
        public string NgayHenPhongVan { get; set; }
        public int NguonUngVien { get; set; }
        public DateTime? ThoiGianTrangThai { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual ViTriCongViec IdviTriTuyenDungNavigation { get; set; }
        public virtual TuyendungYeuCauTuyenDung IdyeuCauNavigation { get; set; }
        public virtual ICollection<TuyendungDanhGiaUv> TuyendungDanhGiaUvs { get; set; }
        public virtual ICollection<TuyendungDiaChiUngVien> TuyendungDiaChiUngViens { get; set; }
        public virtual ICollection<TuyendungKhoaHuanLuyenUv> TuyendungKhoaHuanLuyenUvs { get; set; }
        public virtual ICollection<TuyendungKinhNghiemLamViecUv> TuyendungKinhNghiemLamViecUvs { get; set; }
        public virtual ICollection<TuyendungKyNangVpuv> TuyendungKyNangVpuvs { get; set; }
        public virtual ICollection<TuyendungNgoaiNguUv> TuyendungNgoaiNguUvs { get; set; }
        public virtual ICollection<TuyendungPheDuyetUngVien> TuyendungPheDuyetUngViens { get; set; }
        public virtual ICollection<TuyendungThamChieuUv> TuyendungThamChieuUvs { get; set; }
        public virtual ICollection<TuyendungThongTinGiaDinhUv> TuyendungThongTinGiaDinhUvs { get; set; }
        public virtual ICollection<TuyendungThongTinKhacUv> TuyendungThongTinKhacUvs { get; set; }
        public virtual ICollection<TuyendungVanBangUv> TuyendungVanBangUvs { get; set; }
    }
}
