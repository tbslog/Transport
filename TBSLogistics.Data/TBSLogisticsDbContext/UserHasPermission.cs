using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class UserHasPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual Permission Permission { get; set; }
        public virtual User User { get; set; }
    }
}
