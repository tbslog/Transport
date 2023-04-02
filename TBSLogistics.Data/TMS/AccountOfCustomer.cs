using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class AccountOfCustomer
    {
        public AccountOfCustomer()
        {
            BangGia = new HashSet<BangGia>();
            KhachHangAccount = new HashSet<KhachHangAccount>();
            SubFeePrice = new HashSet<SubFeePrice>();
        }

        public string MaAccount { get; set; }
        public string TenAccount { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<KhachHangAccount> KhachHangAccount { get; set; }
        public virtual ICollection<SubFeePrice> SubFeePrice { get; set; }
    }
}
