using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Services.RomoocManage
{
    public interface IRomooc
    {
        public Task<BoolActionResult> CreateRomooc(CreateRomooc request);
        public Task<BoolActionResult> EditRomooc(string MaRomooc, EditRomooc request);
        public Task<BoolActionResult> DeleteRomooc(string MaRomooc);
        public Task<PagedResponseCustom<ListRomooc>> GetListRomooc(PaginationFilter filter);
        public Task<ListRomooc> GetRomoocById(string MaRomooc);
        Task<List<LoaiRomooc>> GetListSelectRomoocType();
        Task<List<ListRomoocSelect>> GetListRomoocSelect();
    }
}
