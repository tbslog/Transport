using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.AddressModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.AddressManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddress _address;
        private readonly IUriService _uriService;

        public AddressController(IAddress address, IUriService uriService)
        {
            _address = address;
            _uriService = uriService;
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
        public async Task<IActionResult> ListAddress([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _address.GetListAddress(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<GetAddressModel>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListTypeAddress()
        {
            var list = await _address.GetListTypeAddress();

            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            var ImportExcel = await _address.ReadExcelFile(formFile, cancellationToken);

            if (ImportExcel.isSuccess == true)
            {
                return Ok(ImportExcel.Message);
            }
            else
            {
                return BadRequest(ImportExcel.DataReturn + " --- " + ImportExcel.Message);
            }
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListAddress()
        {
            var list = await _address.GetListAddress();

            return Ok(list);
        }

        //[HttpPost]
        //[Route("[action]")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateProvince(int matinh, string tentinh, string phanloai)
        //{
        //    var add = await _address.CreateProvince(matinh, tentinh, phanloai);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("[action]")]
        //[AllowAnonymous]
        //public async Task<IActionResult> CreateDistricts(int mahuyen, string tenhuyen, string phanloai, int parentcode)
        //{
        //    var add = await _address.CreateDistricts(mahuyen, tenhuyen, phanloai, parentcode);
        //    return Ok();
        //}

        //[HttpPost]
        //[Route("[action]")]
        //[AllowAnonymous]
        //public async Task<JsonResult> CreateWard(string jsonResult)
        //{
        //    List<WardModel> wardModels = JsonConvert.DeserializeObject<List<WardModel>>(jsonResult);

        //    var add = await _address.CreateWard(wardModels);
        //    return new JsonResult("OK");
        //}
    }
}