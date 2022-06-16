using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungYeuCauTuyenDung
    {
        public TuyendungYeuCauTuyenDung()
        {
            TuyendungPheDuyetYeuCauTds = new HashSet<TuyendungPheDuyetYeuCauTd>();
            TuyendungThongTinUngViens = new HashSet<TuyendungThongTinUngVien>();
        }

        public int Id { get; set; }
        public string MaBoPhan { get; set; }
        public string NguoiDeXuat { get; set; }
        public string LoaiNguyenNhanTd { get; set; }
        public string LyDoTuyen { get; set; }
        public int GioiTinh { get; set; }
        public int ThoiHanTuyenDung { get; set; }
        public int SoLuongTuyen { get; set; }
        public string YeuCauChuyenMon { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public int IdviTriTuyenDung { get; set; }
        public int IdnguoiDung { get; set; }
        public DateTime? ThoiGianBatDauTd { get; set; }
        public DateTime? ThoiGianKetThucTd { get; set; }
        public DateTime? ThoiGianTrinhDuyet { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual ViTriCongViec IdviTriTuyenDungNavigation { get; set; }
        public virtual ICollection<TuyendungPheDuyetYeuCauTd> TuyendungPheDuyetYeuCauTds { get; set; }
        public virtual ICollection<TuyendungThongTinUngVien> TuyendungThongTinUngViens { get; set; }
    }
}
