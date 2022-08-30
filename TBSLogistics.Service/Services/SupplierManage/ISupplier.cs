using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SupplierModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.SupplierManage
{
    public interface ISupplier
    {
        Task<BoolActionResult> CreateSupplier(CreateSupplierRequest request);
        Task<BoolActionResult> EditSupplier(string SupplierId,UpdateSupplierRequest request);

        Task<GetSupplierRequest> GetSupplierById(string SupplierId);

        Task<PagedResponseCustom<ListSupplierRequest>> getListSupplier(PaginationFilter filter);
    }
}
