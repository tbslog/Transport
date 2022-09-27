using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.ProductServiceManage
{
    public interface IProduct
    {
        public Task<BoolActionResult> CreateProductService(List<CreateProductServiceRequest> request);
        public Task<BoolActionResult> EditProductServiceRequest(int id, EditProductServiceRequest request);
        //public Task<GetProductServiceRequest> GetProductServiceByIdRequest(string id);
        //Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductService(PaginationFilter filter);
    }
}