using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.CommonModel
{
    public class BoolActionResult
    {
        public bool isSuccess { get; set; }
        public string Message { get; set; }
        public string DataReturn { get; set; }
    }
}
