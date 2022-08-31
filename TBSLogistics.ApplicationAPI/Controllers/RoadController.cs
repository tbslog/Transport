using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RoadModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.RoadManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoadController : ControllerBase
    {

        private readonly IRoad _road;
        private IPaginationService _uriService;

        public RoadController(IRoad road, IPaginationService uriService)
        {
            _road = road;
            _uriService = uriService;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateRoad(CreateRoadRequest request)
        {
            var create = await _road.CreateRoad(request);

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
        public async Task<IActionResult> UpdateRoad(string id, UpdateRoadRequest request)
        {
            var Update = await _road.UpdateRoad(id, request);

            if (Update.isSuccess == true)
            {
                return Ok(Update.Message);
            }
            else
            {
                return BadRequest(Update.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetById(string id)
        {
            var getById = await _road.GetRoadById(id);

            return Ok(getById);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ReadFileExcel(IFormFile formFile, CancellationToken cancellationToken)
        {
            var ImportExcel = await _road.ImportExcel(formFile, cancellationToken);

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
        public async Task<IActionResult> GetListRoad([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _road.GetListRoad(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListRoadRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }
    }
}
