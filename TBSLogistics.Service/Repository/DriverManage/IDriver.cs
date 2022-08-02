using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.DriverModel;

namespace TBSLogistics.Service.Repository.DriverManage
{
    public interface IDriver
    {
        Task<BoolActionResult> CreateDriver(CreateDriverRequest request);

        Task<BoolActionResult> EditDriver(string driverId,EditDriverRequest request);

        Task<GetDriverRequest> GetDriverById(string driverId);

        Task<GetDriverRequest> GetDriverByCardId(string cccd);

        Task<List<GetDriverRequest>> GetListDriver();

        Task<List<GetDriverRequest>> GetListByType(string driverType);

        Task<List<GetDriverRequest>> GetListByVehicleType(string vehicleType);

        Task<List<GetDriverRequest>> GetListByStatus(string status);
    }
}