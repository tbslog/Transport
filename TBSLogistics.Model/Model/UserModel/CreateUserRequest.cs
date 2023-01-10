using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class CreateUserRequest
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string HoVaTen { get; set; }
        public string MaNhanVien { get; set; }
        public string MaBoPhan { get; set; }
        public string AccountType { get; set; }
        public int RoleId { get; set; }
        public int TrangThai { get; set; }
    }
}
