using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class UserHasCustomer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CustomerId { get; set; }
        public string AccountId { get; set; }

        public virtual KhachHang Customer { get; set; }
        public virtual NguoiDung User { get; set; }
    }
}
