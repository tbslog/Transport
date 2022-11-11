using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.Bill
{
    public interface IBill
    {
        Task<GetBill> GetBillByCustomerId(string customerId, DateTime fromDate, DateTime toDate);
        Task<PagedResponseCustom<ListCustomerHasBill>> GetListCustomerHasBill(PaginationFilter filter);
    }
}
