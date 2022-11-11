using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.SFeeByTcommandManage
{
    public interface ISFeeByTcommand
    {
        Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request);
        Task<BoolActionResult> DeleteSFeeByTCommand(DeleteSFeeByTCommand request);

        Task<PagedResponseCustom<ListSubFeeIncurred>> GetListSubFeeIncurredApprove(PaginationFilter filter);
        Task<GetSubFeeIncurred> GetSubFeeIncurredById(int id);
        Task<BoolActionResult> ApproveSubFeeIncurred(List<ApproveSFeeByTCommand> request);
        Task<List<ListSubFeeIncurred>> GetListSubFeeIncurredByHandling(int id);


    }
}
