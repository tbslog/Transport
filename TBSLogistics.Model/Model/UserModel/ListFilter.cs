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
        public List<string> suppliers { get; set; }
        public List<string> users { get; set; }
        public List<string> accountIds { get; set; }
        public List<int> listDiemDau { get; set; }
        public List<int> listDiemCuoi { get; set; }
        public List<int?> listDiemLayTraRong { get; set; }
    }

    public class ListUser
    {
        public string userName { get; set; }
        public string name { get; set; }
    }


}
