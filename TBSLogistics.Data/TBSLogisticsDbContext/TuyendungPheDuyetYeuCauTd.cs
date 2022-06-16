using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungPheDuyetYeuCauTd
    {
        public int Id { get; set; }
        public int YeuCauId { get; set; }
        public int IdnguoiDung { get; set; }
        public int KetQuaPheDuyet { get; set; }
        public string LyDo { get; set; }
        public DateTime ThoiGianPheDuyet { get; set; }

        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
        public virtual TuyendungYeuCauTuyenDung YeuCau { get; set; }
    }
}
