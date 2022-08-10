using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;

namespace TBSLogistics.Model.Wrappers
{
    public class PagedResponseCustom<T>
    {
        public PaginationFilter paginationFilter { get; set; }
        public int totalCount { get; set; }
        public List<T> dataResponse { get; set; }
    }
}
