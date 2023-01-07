using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class Role
    {
        public Role()
        {
            RoleHasPermission = new HashSet<RoleHasPermission>();
            UserHasRole = new HashSet<UserHasRole>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }
        public int Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual ICollection<RoleHasPermission> RoleHasPermission { get; set; }
        public virtual ICollection<UserHasRole> UserHasRole { get; set; }
    }
}
