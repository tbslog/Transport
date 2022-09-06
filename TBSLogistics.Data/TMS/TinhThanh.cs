using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class TinhThanh
    {
        public TinhThanh()
        {
            DiaDiem = new HashSet<DiaDiem>();
        }

        public int MaTinh { get; set; }
        public string TenTinh { get; set; }
        public string PhanLoai { get; set; }

        public virtual ICollection<DiaDiem> DiaDiem { get; set; }
    }
}
