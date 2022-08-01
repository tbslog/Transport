using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class NhaPhanPhoi
    {
        public string MaNpp { get; set; }
        public string MaDoiTac { get; set; }
        public string TenNpp { get; set; }
        public int MaDiaDiem { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual DoiTac MaDoiTacNavigation { get; set; }
    }
}
