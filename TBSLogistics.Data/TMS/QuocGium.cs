using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class QuocGium
    {
        public QuocGium()
        {
            DiaDiems = new HashSet<DiaDiem>();
        }

        public int MaQuocGia { get; set; }
        public string TenQuocGia { get; set; }
        public string Code { get; set; }

        public virtual ICollection<DiaDiem> DiaDiems { get; set; }
    }
}
