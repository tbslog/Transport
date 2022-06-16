using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class NguoiDungTheoDanhMuc
    {
        public int Id { get; set; }
        public int IddanhMuc { get; set; }
        public int DanhMucCha { get; set; }
        public int IdnguoiDung { get; set; }

        public virtual DanhMuc IddanhMucNavigation { get; set; }
        public virtual NguoiDung IdnguoiDungNavigation { get; set; }
    }
}
