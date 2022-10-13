using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.SubFeePriceModel;

namespace TBSLogistics.Service.Services.SubFeePriceManage
{
    public interface ISubFeePrice
    {
        Task<BoolActionResult> CreateSubFeePrice(CreateSubFeePriceRequest request);
        Task<BoolActionResult> UpdateSubFeePrice(long id, UpdateSubFeePriceRequest request);
        Task<BoolActionResult> ApproveSubFeePrice(long[] ids, string HDPL);
        Task<GetSubFeePriceRequest> GetSubFeePriceById(long id);
        Task<List<GetSubFeePriceRequest>> GetListSubFeePrice(string KH, string HDPL, DateTime ngayapdung, byte trangthai);
        Task<BoolActionResult> DisableSubFeePrice(long[] ids, string HDPL);
        Task<BoolActionResult> DeleteSubFeePrice(long[] ids, string HDPL);
    }
}
