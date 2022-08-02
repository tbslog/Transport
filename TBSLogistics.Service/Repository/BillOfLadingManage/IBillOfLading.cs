using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.BillOfLadingModel;

namespace TBSLogistics.Service.Repository.BillOfLadingManage
{
    public interface IBillOfLading
    {
        Task<BoolActionResult> CreateBillOfLading(CreateBillOfLadingRequest request);
        Task<BoolActionResult> EditBillOfLading(string billOfLadingId, EditBillOfLadingRequest request);

        Task<GetBillOfLadingRequest> GetBillOfLadingById(string billOfLadingId);
    }
}
