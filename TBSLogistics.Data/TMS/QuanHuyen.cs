using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class QuanHuyen
    {
        public QuanHuyen()
        {
            DiaDiem = new HashSet<DiaDiem>();
        }

        public int MaHuyen { get; set; }
        public string TenHuyen { get; set; }
        public string PhanLoai { get; set; }
        public int ParentCode { get; set; }

        public virtual ICollection<DiaDiem> DiaDiem { get; set; }
    }
}
