using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Common;
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
        private readonly ICommon _common;
        public RomoocController(IRomooc romooc, IPaginationService uriService, ICommon common)
        {
            _common = common;
            _Romooc = romooc;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateRomooc(CreateRomooc request)
        {
            var checkPermission = await _common.CheckPermission("O0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("O0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

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
            var checkPermission = await _common.CheckPermission("O0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _Romooc.GetListRomooc(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListRomooc>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetRomoocById(string MaRomooc)
        {
            var checkPermission = await _common.CheckPermission("O0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListRomoocSelect()
        {
            var list = await _Romooc.GetListRomoocSelect();
            return Ok(list);
        }
    }
}