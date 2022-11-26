using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class UserHasRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual NguoiDung User { get; set; }
    }
}
