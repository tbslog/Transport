using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class UserHasPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual NguoiDung User { get; set; }
    }
}
