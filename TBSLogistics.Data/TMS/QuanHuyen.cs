using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class QuanHuyen
    {
        public QuanHuyen()
        {
            DiaDiems = new HashSet<DiaDiem>();
        }

        public int MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public string PhanLoai { get; set; }
        public int ParentCode { get; set; }

        public virtual ICollection<DiaDiem> DiaDiems { get; set; }
    }
}
