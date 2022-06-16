using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungKetQuaDanhGium
    {
        public int Id { get; set; }
        public int IddanhGia { get; set; }
        public int Idtcdg { get; set; }
        public int DiemDg { get; set; }
        public string GhiChu { get; set; }

        public virtual TuyendungDanhGiaUv IddanhGiaNavigation { get; set; }
        public virtual TuyendungTieuChiDanhGium IdtcdgNavigation { get; set; }
    }
}
