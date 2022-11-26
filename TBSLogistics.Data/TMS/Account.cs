using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class Account
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
    }
}
