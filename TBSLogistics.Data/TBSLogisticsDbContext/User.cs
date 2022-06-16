using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class User
    {
        public User()
        {
            UserHasPermissions = new HashSet<UserHasPermission>();
            UserHasRoles = new HashSet<UserHasRole>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public int Status { get; set; }

        public virtual NguoiDung IdNavigation { get; set; }
        public virtual ICollection<UserHasPermission> UserHasPermissions { get; set; }
        public virtual ICollection<UserHasRole> UserHasRoles { get; set; }
    }
}
