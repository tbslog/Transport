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
        Task<GetBill> GetBillByCustomerId(string customerId,int ky);
        Task<PagedResponseCustom<ListVanDon>> GetListTransportByCustomerId(string customerId, int ky, PaginationFilter filter);
        Task<GetBill> GetBillByTransportId(string customerId, string transportId, int ky);
        Task<List<KyThanhToan>> GetListKyThanhToan(string customerId);
    }
}
