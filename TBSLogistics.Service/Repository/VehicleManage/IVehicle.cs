using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.VehicleModel;

namespace TBSLogistics.Service.Repository.VehicleManage
{
    public interface IVehicle
    {
        Task<BoolActionResult> CreateVehicle(CreateVehicleRequest request);
        Task<BoolActionResult> EditVehicle(string vehicleId,EditVehicleRequest request);

        Task<GetVehicleRequest> GetVehicleById(string vehicleId);
        Task<List<GetVehicleRequest>> GetListVehicle();
    }
}
