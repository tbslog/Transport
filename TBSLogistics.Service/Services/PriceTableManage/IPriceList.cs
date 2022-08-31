using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PriceListModel;

namespace TBSLogistics.Service.Repository.PricelistManage
{
    public interface IPriceList
    {
        Task<BoolActionResult> CreatePriceList(CreatePriceListRequest request);
        Task<BoolActionResult> EditPriceList(string PriceListId,UpdatePriceListRequest request);

        Task<GetPriceListRequest> GetPriceListById(string PriceListId);
        Task<List<GetPriceListRequest>> GetListPriceList();
        Task<List<GetPriceListRequest>> GetListPriceListByCusId(string CustomerId);
    }
}
