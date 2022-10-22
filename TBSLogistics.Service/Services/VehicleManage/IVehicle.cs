using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.VehicleModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.VehicleManage
{
    public interface IVehicle
    {
        Task<BoolActionResult> CreateVehicle(CreateVehicleRequest request);
        Task<BoolActionResult> EditVehicle(string vehicleId,EditVehicleRequest request);

        Task<GetVehicleRequest> GetVehicleById(string vehicleId);
        Task<BoolActionResult> DeleteVehicle(string vehicleId);
        Task<PagedResponseCustom<ListVehicleRequest>> getListVehicle(PaginationFilter filter);
    }
}
