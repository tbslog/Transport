using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungTieuChiDanhGium
    {
        public TuyendungTieuChiDanhGium()
        {
            TuyendungKetQuaDanhGia = new HashSet<TuyendungKetQuaDanhGium>();
        }

        public int Id { get; set; }
        public string TenTcdg { get; set; }
        public string GhiChuTcdg { get; set; }
        public int Stt { get; set; }
        public int? TieuDeDanhGia { get; set; }
        public DateTime ThoiGian { get; set; }
        public int IdnguoiDung { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual ICollection<TuyendungKetQuaDanhGium> TuyendungKetQuaDanhGia { get; set; }
    }
}
