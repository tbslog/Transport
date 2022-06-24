using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PermissionModel
{
  public  class TreePermissionRequest
    {
        public string id { get; set; }
        public string text { get; set; }
        public object state { get; set; }
        public List<TreePermissionRequest> children { get; set; }
    }
}
