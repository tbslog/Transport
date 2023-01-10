using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class AddCusForUser
    {
        public int UserID { get; set; }
        public List<string> CusId { get; set; }
    }
}
