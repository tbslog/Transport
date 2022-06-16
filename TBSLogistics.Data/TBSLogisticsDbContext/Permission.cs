using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class Permission
    {
        public Permission()
        {
            InversePearent = new HashSet<Permission>();
            RoleHasPermissions = new HashSet<RoleHasPermission>();
            UserHasPermissions = new HashSet<UserHasPermission>();
        }

        public int Id { get; set; }
        public int MId { get; set; }
        public string Name { get; set; }
        public int? PearentId { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual Permission Pearent { get; set; }
        public virtual ICollection<Permission> InversePearent { get; set; }
        public virtual ICollection<RoleHasPermission> RoleHasPermissions { get; set; }
        public virtual ICollection<UserHasPermission> UserHasPermissions { get; set; }
    }
}
