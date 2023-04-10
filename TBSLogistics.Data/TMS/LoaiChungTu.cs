using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class LoaiChungTu
    {
        public LoaiChungTu()
        {
            TepChungTu = new HashSet<TepChungTu>();
        }

        public int MaLoaiChungTu { get; set; }
        public string TenLoaiChungTu { get; set; }
        public string MoTa { get; set; }

        public virtual ICollection<TepChungTu> TepChungTu { get; set; }
    }
}
