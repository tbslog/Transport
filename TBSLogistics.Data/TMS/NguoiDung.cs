using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class NguoiDung
    {
        public NguoiDung()
        {
            UserHasCustomer = new HashSet<UserHasCustomer>();
            UserHasPermission = new HashSet<UserHasPermission>();
            UserHasRole = new HashSet<UserHasRole>();
        }

        public int Id { get; set; }
        public string HoVaTen { get; set; }
        public string MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public int RoleId { get; set; }
        public int TrangThai { get; set; }
        public string NguoiTao { get; set; }
        public string AccountType { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }

        public virtual Account IdNavigation { get; set; }
        public virtual ICollection<UserHasCustomer> UserHasCustomer { get; set; }
        public virtual ICollection<UserHasPermission> UserHasPermission { get; set; }
        public virtual ICollection<UserHasRole> UserHasRole { get; set; }
    }
}
