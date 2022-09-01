using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;

namespace TBSLogistics.Service.Panigation
{
    public interface IPaginationService
    {
        public Uri GetPageUri(PaginationFilter filter, string route);
    }
}
