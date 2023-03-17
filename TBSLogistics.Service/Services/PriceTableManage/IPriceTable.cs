using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.PricelistManage
{
    public interface IPriceTable
    {
        Task<BoolActionResult> CreatePriceTable(List<CreatePriceListRequest> request);
        Task<PagedResponseCustom<GetListPiceTableRequest>> GetListPriceTable(PaginationFilter filter, ListFilter listFilter);
        Task<PagedResponseCustom<GetPriceListRequest>> GetListPriceTableByContractId(string contractId, string onlyContractId, ListFilter listFilter, PaginationFilter filter);
        Task<List<GetPriceListRequest>> GetListPriceTableByCustommerId(string MaKH);
        Task<PagedResponseCustom<ListApprove>> GetListPriceTableApprove(PaginationFilter filter);
        Task<BoolActionResult> ApprovePriceTable(ApprovePriceTable request);
        Task<BoolActionResult> UpdatePriceTable(int id, GetPriceListById request);
        Task<GetPriceListById> GetPriceTableById(int id);
    }
}
