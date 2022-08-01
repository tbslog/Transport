using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class XaPhuong
    {
        public XaPhuong()
        {
            DiaDiems = new HashSet<DiaDiem>();
        }

        public int MaPhuong { get; set; }
        public string TenPhuong { get; set; }
        public string PhanLoai { get; set; }
        public int ParentCode { get; set; }

        public virtual ICollection<DiaDiem> DiaDiems { get; set; }
    }
}
