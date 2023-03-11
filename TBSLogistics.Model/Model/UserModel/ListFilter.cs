using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class ListFilter
    {
        public List<string> customers { get; set; }
        public List<string> users { get; set; }
    }

    public class ListUser
    {
        public string userName { get; set; }
        public string name { get; set; }
    }


}
