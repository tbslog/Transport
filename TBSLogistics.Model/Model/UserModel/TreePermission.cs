using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
    public class TreePermission
    {
        public string RoleName { get; set; }
        public int Status { get; set; }
        public List<string> IsChecked { get; set; }
        public List<ListTree> ListTree { get; set; }
    }

    public class TreeCustomer
    {
        public List<string> IsChecked { get; set; }
        public List<ListTree> ListTree { get; set; }
    }

    public class ListTree
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public List<ListTree> Children { get; set; }
    }
}
