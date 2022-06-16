using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NguoiDung
    {
        public NguoiDung()
        {
            DanhMucs = new HashSet<DanhMuc>();
            NguoiDungTheoDanhMucs = new HashSet<NguoiDungTheoDanhMuc>();
            TuyendungDanhGiaUvs = new HashSet<TuyendungDanhGiaUv>();
            TuyendungPheDuyetUngViens = new HashSet<TuyendungPheDuyetUngVien>();
            TuyendungPheDuyetYeuCauTds = new HashSet<TuyendungPheDuyetYeuCauTd>();
            TuyendungThongTinUngViens = new HashSet<TuyendungThongTinUngVien>();
            TuyendungTieuChiDanhGia = new HashSet<TuyendungTieuChiDanhGium>();
            TuyendungYeuCauTuyenDungs = new HashSet<TuyendungYeuCauTuyenDung>();
            ViTriCongViecs = new HashSet<ViTriCongViec>();
        }

        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public string MaCapBac { get; set; }
        public int Role { get; set; }
        public int TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<DanhMuc> DanhMucs { get; set; }
        public virtual ICollection<NguoiDungTheoDanhMuc> NguoiDungTheoDanhMucs { get; set; }
        public virtual ICollection<TuyendungDanhGiaUv> TuyendungDanhGiaUvs { get; set; }
        public virtual ICollection<TuyendungPheDuyetUngVien> TuyendungPheDuyetUngViens { get; set; }
        public virtual ICollection<TuyendungPheDuyetYeuCauTd> TuyendungPheDuyetYeuCauTds { get; set; }
        public virtual ICollection<TuyendungThongTinUngVien> TuyendungThongTinUngViens { get; set; }
        public virtual ICollection<TuyendungTieuChiDanhGium> TuyendungTieuChiDanhGia { get; set; }
        public virtual ICollection<TuyendungYeuCauTuyenDung> TuyendungYeuCauTuyenDungs { get; set; }
        public virtual ICollection<ViTriCongViec> ViTriCongViecs { get; set; }
    }
}
