using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.ContractManage
{
    public interface IContract
    {
        public Task<BoolActionResult> CreateContract(CreateContract request);
        public Task<BoolActionResult> EditContract(string id, EditContract request);
        public Task<GetContractById> GetContractById(string id);
        Task<PagedResponseCustom<ListContract>> GetListContract(PaginationFilter filter);
        Task<List<GetContractById>> GetListContractSelect(string MaKH, bool getChild, bool getProductService);


    }
}
