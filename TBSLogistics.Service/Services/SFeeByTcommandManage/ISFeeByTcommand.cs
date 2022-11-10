using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.SFeeByTcommand;
using TBSLogistics.Model.Model.SFeeByTcommandModel;

namespace TBSLogistics.Service.Services.SFeeByTcommandManage
{
    public interface ISFeeByTcommand
    {
        Task<BoolActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandRequest> request);
        Task<BoolActionResult> DeleteSFeeByTCommand(DeleteSFeeByTCommand request);
        Task<List<GetListSubFeeByHandling>> GetListSubFeeByHandling(long IdTcommand1);
        Task<BoolActionResult> ApproveSFeeByTCommand(List<ApproveSFeeByTCommand> request);


    }
}
