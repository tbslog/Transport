using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class LoaiDiaDiem
    {
        public LoaiDiaDiem()
        {
            DiaDiem = new HashSet<DiaDiem>();
        }

        public string MaLoaiDiaDiem { get; set; }
        public string TenPhanLoaiDiaDiem { get; set; }

        public virtual ICollection<DiaDiem> DiaDiem { get; set; }
    }
}
