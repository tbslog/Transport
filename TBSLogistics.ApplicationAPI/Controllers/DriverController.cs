using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.DriverModel;
using TBSLogistics.Service.Repository.DriverManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriver _driver;

        public DriverController(IDriver driver)
        {
            _driver = driver;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateDriver(CreateDriverRequest request)
        {
            var create = await _driver.CreateDriver(request);

            if (create.isSuccess == true)
            {
                return Ok(create.Message);
            }
            else
            {
                return BadRequest(create.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> EditDriver(string driverId, EditDriverRequest request)
        {
            var update = await _driver.EditDriver(driverId, request);

            if (update.isSuccess == true)
            {
                return Ok(update.Message);
            }
            else
            {
                return BadRequest(update.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> getDriverById(string driverId)
        {
            var driver = await _driver.GetDriverById(driverId);
            return Ok(driver);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> getDriverCardId(string cccd)
        {
            var driver = await _driver.GetDriverByCardId(cccd);
            return Ok(driver);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> getListDriverByStatus(string status)
        {
            var driver = await _driver.GetListByStatus(status);
            return Ok(driver);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListByType(string driverType)
        {
            var driver = await _driver.GetListByType(driverType);
            return Ok(driver);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListByVehicleType(string vehicleType)
        {
            var driver = await _driver.GetListByVehicleType(vehicleType);
            return Ok(driver);
        }


        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListDriver()
        {
            var driver = await _driver.GetListDriver();
            return Ok(driver);
        }

    }
}
