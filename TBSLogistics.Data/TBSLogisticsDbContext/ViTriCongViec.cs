using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class ViTriCongViec
    {
        public ViTriCongViec()
        {
            TuyendungThongTinUngViens = new HashSet<TuyendungThongTinUngVien>();
            TuyendungYeuCauTuyenDungs = new HashSet<TuyendungYeuCauTuyenDung>();
        }

        public int Id { get; set; }
        public string MaBoPhan { get; set; }
        public string MaCapBac { get; set; }
        public string ViTriCongViec1 { get; set; }
        public string NoiLamViec { get; set; }
        public int ThoiGianTuyenDung { get; set; }
        public int ThoiGianThongBaoKq { get; set; }
        public int IdnguoiDung { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual ICollection<TuyendungThongTinUngVien> TuyendungThongTinUngViens { get; set; }
        public virtual ICollection<TuyendungYeuCauTuyenDung> TuyendungYeuCauTuyenDungs { get; set; }
    }
}
