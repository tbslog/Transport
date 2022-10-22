using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Filter
{
    public class PaginationFilter
    {
        public string contractType { get; set; }
        public string contractId { get; set; }
        public string customerId { get; set; }
        public int? statusId { get; set; }
        public DateTime date { get; set; }

        public string customerType { get; set; }
        public string customerGroup { get; set; }

        public string Status { get; set; }
        public string goodsType { get; set; }
        public string vehicleType { get; set; }

        public string Keyword { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public PaginationFilter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize;
        }
    }
}
