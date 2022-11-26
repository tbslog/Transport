using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class QuocGia
    {
        public QuocGia()
        {
            DiaDiem = new HashSet<DiaDiem>();
        }

        public int MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public string Code { get; set; }

        public virtual ICollection<DiaDiem> DiaDiem { get; set; }
    }
}
