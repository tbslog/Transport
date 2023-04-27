using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.Bill
{
    public interface IBill
    {
        Task<GetBill> GetBillByCustomerId(string customerId, DateTime fromDate, DateTime toDate);
        Task<PagedResponseCustom<ListVanDon>> GetListTransportByCustomerId(string customerId, int ky, PaginationFilter filter);
        Task<GetBill> GetBillByTransportId(string transportId, long? handlingId);
        Task<List<KyThanhToan>> GetListKyThanhToan(string customerId);
        Task<PagedResponseCustom<ListBillHandling>> GetListBillHandling(PaginationFilter filter);
        Task<PagedResponseCustom<ListBillTransportWeb>> GetListBillWeb(PaginationFilter filter);
    }
}
