using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class KhuVuc
    {
        public KhuVuc()
        {
            DiaDiem = new HashSet<DiaDiem>();
        }

        public int Id { get; set; }
        public string TenKhuVuc { get; set; }

        public virtual ICollection<DiaDiem> DiaDiem { get; set; }
    }
}
