using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.SubFeePriceModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.SubFeePriceManage
{
    public interface ISubFeePrice
    {
        Task<BoolActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request);
        Task<BoolActionResult> UpdateSubFeePrice(long id, UpdateSubFeePriceRequest request);
        Task<BoolActionResult> ApproveSubFeePrice(long[] ids, string HDPL);
        Task<GetSubFeePriceRequest> GetSubFeePriceById(long id);
        Task<PagedResponseCustom<ListSubFeePriceRequest>> GetListSubFeePrice(PaginationFilter filter);
        Task<BoolActionResult> DisableSubFeePrice(long[] ids, string HDPL);
        Task<BoolActionResult> DeleteSubFeePrice(long[] ids, string HDPL);
        Task<List<ListSubFee>> GetListSubFeeSelect();
    }
}
