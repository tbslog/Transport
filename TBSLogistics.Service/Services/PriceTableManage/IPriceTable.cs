using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PriceListModel;

namespace TBSLogistics.Service.Repository.PricelistManage
{
    public interface IPriceTable
    {
        Task<BoolActionResult> CreatePriceTable(CreatePriceListRequest request);
        Task<BoolActionResult> EditPriceTable(string PriceListId,UpdatePriceListRequest request);

        Task<GetPriceListRequest> GetPriceTableById(string PriceListId);
        Task<List<GetPriceListRequest>> GetListPriceTable();
        Task<List<GetPriceListRequest>> GetListPriceTableByCusId(string CustomerId);
    }
}
