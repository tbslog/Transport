using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class SetRole
    {
        public string RoleName { get; set; }
        public int RoleStatus { get; set; }
        public List<string> Permission { get; set; }
    }
}
