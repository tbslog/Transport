using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.PricelistManage
{
    public interface IPriceTable
    {
        Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request);

        Task<PagedResponseCustom<GetListPiceTableRequest>> GetListPriceTable(PaginationFilter filter);
        Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, string onlyContractId, int PageNumber, int PageSize);
        Task<List<GetPriceListRequest>> GetListPriceTableByCustommerId(string MaKH);
        Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(PaginationFilter filter);
        Task<BoolActionResult> ApprovePriceTable(ApprovePriceTable request);
        Task<BoolActionResult> UpdatePriceTable(int id, GetPriceListRequest request);
        Task<GetPriceListRequest> GetPriceTableById(int id);
    }
}
