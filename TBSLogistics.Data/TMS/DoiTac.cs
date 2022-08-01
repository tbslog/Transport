using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class DoiTac
    {
        public DoiTac()
        {
            NhaPhanPhois = new HashSet<NhaPhanPhoi>();
        }

        public string MaDoiTac { get; set; }
        public string MaKh { get; set; }
        public string TenDoiTac { get; set; }
        public string NhomDc { get; set; }
        public int MaDiaDiem { get; set; }
        public DateTime UpdateTime { get; set; }
        public DateTime Createdtime { get; set; }

        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<NhaPhanPhoi> NhaPhanPhois { get; set; }
    }
}
