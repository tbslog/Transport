using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.CommonModel
{
   public  class PagingViewModel<T>
    {
        public PageResult<T> Result { get; set; }
    }
}
