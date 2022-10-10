using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public interface IBillOfLading
    {
        public Task<BoolActionResult> CreateBillOfLading(CreateBillOfLadingRequest request);
        public Task<BoolActionResult> EditBillOfLading(string billOfLadingId, EditBillOfLadingRequest request);
        public Task<BoolActionResult> DeleteBillOfLading(DeleteBillOfLading request);
        public Task<GetBillOfLadingRequest> GetBillOfLadingById(string billOfLadingId);
        public Task<PagedResponseCustom<GetBillOfLadingRequest>> GetListBillOfLading(PaginationFilter filter);
    }
}
