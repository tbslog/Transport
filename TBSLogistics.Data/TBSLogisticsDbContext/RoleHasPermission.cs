using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class RoleHasPermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual Role Role { get; set; }
    }
}
