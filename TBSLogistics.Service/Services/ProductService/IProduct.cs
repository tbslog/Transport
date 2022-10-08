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
        public Task<BoolActionResult> DeleteProductServiceRequest(int  Id);
        public Task<BoolActionResult> ApproveProductServiceRequestByMaHD(string MaHopDong);
        public Task<BoolActionResult> ApproveProductServiceRequestById(List<int> Id);
        public Task<ListProductServiceRequest> GetProductServiceByIdRequest(int Id);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaKH(PaginationFilter filter, string MaKH );
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByMaHD(PaginationFilter filter, string MaKH);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductService(PaginationFilter filter, int trangthai);
        public Task<PagedResponseCustom<ListProductServiceRequest>> GetListProductServiceByDate(PaginationFilter filter, DateTime date);

    }
}