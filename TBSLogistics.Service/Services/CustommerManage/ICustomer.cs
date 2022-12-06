using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.CustomerModel;
using TBSLogistics.Model.Model.CustommerModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.CustommerManage
{
    public interface ICustomer
    {
        Task<BoolActionResult> CreateCustomer(CreateCustomerRequest request);
        Task<BoolActionResult> EditCustomer(string CustomerId, EditCustomerRequest request);
        Task<GetCustomerRequest> GetCustomerById(string CustomerId);
        Task<PagedResponseCustom<ListCustommerRequest>> getListCustommer(PaginationFilter filter);
        Task<List<GetCustomerRequest>> getListCustomerOptionSelect(string type);
        //Task<BoolActionResult> ReadExcelFile(IFormFile formFile, CancellationToken cancellationToken);

    }
}
