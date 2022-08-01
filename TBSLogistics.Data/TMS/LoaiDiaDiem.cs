using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class LoaiDiaDiem
    {
        public LoaiDiaDiem()
        {
            DiaDiems = new HashSet<DiaDiem>();
        }

        public string MaLoaiDiaDiem { get; set; }
        public string TenPhanLoaiDiaDiem { get; set; }

        public virtual ICollection<DiaDiem> DiaDiems { get; set; }
    }
}
