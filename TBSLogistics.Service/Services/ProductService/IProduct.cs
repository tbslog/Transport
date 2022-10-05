using System;
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
        public Task<BoolActionResult> EditProductServiceRequest( EditProductServiceRequest request);
        public Task<BoolActionResult> DeleteProductServiceRequest(DeleteProductServiceRequest request);
        public Task<BoolActionResult> ApproveProductServiceRequestByMaHD(ApproveProductServiceRequestByMaHD request);
        public Task<BoolActionResult> ApproveProductServiceRequestById(List<ApproveProductServiceRequestById> request);
        public Task<ListProductServiceRequest> GetProductServiceByIdRequest(int id);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaKH(PaginationFilter filter, string MaKH );
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaHD(PaginationFilter filter, string MaKH);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductService(PaginationFilter filter, int trangthai);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByDate(PaginationFilter filter, DateTime date);

    }
}