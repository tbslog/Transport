using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.VehicleModel;
using TBSLogistics.Service.Repository.VehicleManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IVehicle _vehicle;

        public VehicleController(IVehicle vehicle)
        {
            _vehicle = vehicle;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateVehicle(CreateVehicleRequest request)
        {
            var create = await _vehicle.CreateVehicle(request);

            if (create.isSuccess == true)
            {
                return Ok(create.Message);
            }
            else
            {
                return BadRequest(create.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> EditVehicle(string vehicleId, EditVehicleRequest request)
        {
            var Edit = await _vehicle.EditVehicle(vehicleId, request);

            if (Edit.isSuccess == true)
            {
                return Ok(Edit.Message);
            }
            else
            {
                return BadRequest(Edit.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetVehicleById(string vehicleId)
        {
            var vehicle = await _vehicle.GetVehicleById(vehicleId);

            return Ok(vehicle);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListVehicle()
        {
            var list = await _vehicle.GetListVehicle();
            return Ok(list);
        }

    }
}
