using System;
using System.Collections.Generic;

#nullable disable

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
