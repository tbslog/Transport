using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.RoadManage
{
    public interface IRoad
    {
        Task<GetRoadRequest> GetRoadById(string MaCungDuong);

        Task<BoolActionResult> CreateRoad(CreateRoadRequest request);
        Task<BoolActionResult> UpdateRoad(string MaCungDuong, UpdateRoadRequest request);
        Task<PagedResponseCustom<ListRoadRequest>> GetListRoad(PaginationFilter request);
        Task<List<GetRoadRequest>> getListRoadOptionSelect(string MaKH);
        Task<List<GetRoadRequest>> getListRoadByPoint(int diemDau, int diemCuoi);
        Task<BoolActionResult> ImportExcel(IFormFile file, CancellationToken cancellationToken);

    }
}
