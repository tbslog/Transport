using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungDanhGiaUv
    {
        public TuyendungDanhGiaUv()
        {
            TuyendungKetQuaDanhGia = new HashSet<TuyendungKetQuaDanhGium>();
        }

        public int Id { get; set; }
        public DateTime ThoiGian { get; set; }
        public int XacXuatThanhCong { get; set; }
        public int DeNghiTuyenDung { get; set; }
        public string LyDoDeNghi { get; set; }
        public string DeXuatHl { get; set; }
        public string NhanXetThemKndp { get; set; }
        public string NhanXetThemTph { get; set; }
        public int IdnguoiDung { get; set; }
        public int IdungVien { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
        public virtual ICollection<TuyendungKetQuaDanhGium> TuyendungKetQuaDanhGia { get; set; }
    }
}
