using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.CommonModel
{
   public class PageResult<T>:PageResultBase
    {
        public PageResultBase PageResultBase { get; set; }
        public List<T> Items { get; set; }
    }
}
