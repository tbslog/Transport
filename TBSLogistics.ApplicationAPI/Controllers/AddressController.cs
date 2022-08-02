using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Service.Repository.AddressManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddress _address;

        public AddressController(IAddress address)
        {
            _address = address;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateAddress(CreateAddressRequest request)
        {
            var CreateAddress = await _address.CreateAddress(request);

            if (CreateAddress.isSuccess == true)
            {
                return Ok(CreateAddress.Message);
            }
            else
            {
                return BadRequest(CreateAddress.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> EditAddress(int id, UpdateAddressRequest request)
        {
            var EditAddress = await _address.EditAddress(id, request);

            if (EditAddress.isSuccess == true)
            {
                return Ok(EditAddress.Message);
            }
            else
            {
                return BadRequest(EditAddress.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetAddressById(int AddressId)
        {
            var address = await _address.GetAddressById(AddressId);
            return Ok(address);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ListAddress()
        {
            var list = await _address.GetListAddress();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ListProvinces()
        {
            var list = await _address.GetProvinces();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ListDistricts(int ProvinceId)
        {
            var list = await _address.GetDistricts(ProvinceId);
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> ListWards(int DistrictId)
        {
            var list = await _address.GetWards(DistrictId);
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateProvince(int matinh, string tentinh, string phanloai)
        {
            var add = await _address.CreateProvince(matinh, tentinh, phanloai);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode)
        {
            var add = await _address.CreateDistricts(mahuyen, tenhuyen, phanloai, parentcode);
            return Ok();
        }

        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<JsonResult> CreateWard(string jsonResult)
        {
            List<WardModel> wardModels = JsonConvert.DeserializeObject<List<WardModel>>(jsonResult);

            var add = await _address.CreateWard(wardModels);
            return new JsonResult("OK");
        }
    }
}