using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class Role
    {
        public Role()
        {
            RoleHasPermissions = new HashSet<RoleHasPermission>();
            UserHasRoles = new HashSet<UserHasRole>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; set; }
        public virtual ICollection<UserHasRole> UserHasRoles { get; set; }
    }
}
