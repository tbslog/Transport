using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.RomoocManage;

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RomoocController : ControllerBase
    {
        private readonly IRomooc _Romooc;
        private readonly IPaginationService _uriService;

        public RomoocController(IRomooc romooc, IPaginationService uriService)
        {
            _Romooc = romooc;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateRomooc(CreateRomooc request)
        {
            var create = await _Romooc.CreateRomooc(request);

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
        public async Task<IActionResult> UpdateRomooc(string MaRomooc, EditRomooc request)
        {
            var Edit = await _Romooc.EditRomooc(MaRomooc, request);

            if (Edit.isSuccess == true)
            {
                return Ok(Edit.Message);
            }
            else
            {
                return BadRequest(Edit.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> DeleteRomooc(string MaRomooc)
        {
            var Edit = await _Romooc.DeleteRomooc(MaRomooc);

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
        public async Task<IActionResult> GetListRomooc([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _Romooc.GetListRomooc(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListRomooc>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetRomoocById(string MaRomooc)
        {
            var byId = await _Romooc.GetRomoocById(MaRomooc);

            return Ok(byId);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSelectRomoocType()
        {
            var list = await _Romooc.GetListSelectRomoocType();
            return Ok(list);
        }
    }
}