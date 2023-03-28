using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class KhachHangAccount
    {
        public string MaKh { get; set; }
        public string MaAccount { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public long Id { get; set; }

        public virtual AccountOfCustomer MaAccountNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
    }
}
