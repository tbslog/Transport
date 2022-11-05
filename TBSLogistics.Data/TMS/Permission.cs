using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class Permission
    {
        public Permission()
        {
            RoleHasPermission = new HashSet<RoleHasPermission>();
            UserHasPermission = new HashSet<UserHasPermission>();
        }

        public int Id { get; set; }
        public string Mid { get; set; }
        public string PermissionName { get; set; }
        public string ParentId { get; set; }
        public string Type { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }

        public virtual ICollection<RoleHasPermission> RoleHasPermission { get; set; }
        public virtual ICollection<UserHasPermission> UserHasPermission { get; set; }
    }
}
