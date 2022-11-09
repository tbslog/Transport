using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.BillOfLadingManage;
using TBSLogistics.Service.Repository.Common;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillOfLadingController : ControllerBase
    {
        private readonly IBillOfLading _billOfLading;
        private IPaginationService _paninationService;
        private ICommon _common;

        public BillOfLadingController(IBillOfLading billOfLading, IPaginationService paginationService, ICommon common)
        {
            _billOfLading = billOfLading;
            _paninationService = paginationService;
            _common = common;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> LoadDataHandling(string RoadId)
        {
            var data = await _billOfLading.LoadDataHandling(RoadId);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTransportById(string transportId)
        {
            var data = await _billOfLading.GetTransportById(transportId);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransport([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListTransport(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListTransport>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListHandling([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _billOfLading.GetListHandling(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListHandling>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateTransport(CreateTransport request)
        {
            var create = await _billOfLading.CreateTransport(request);

            if (create.isSuccess)
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
        public async Task<IActionResult> CreateHanling(CreateHandling request)
        {
            var create = await _billOfLading.CreateHandling(request);

            if (create.isSuccess)
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
        public async Task<IActionResult> UpdateTransport(string transportId, UpdateTransport request)
        {
            var update = await _billOfLading.UpdateTransport(transportId, request);

            if (update.isSuccess)
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
        public async Task<IActionResult> GetListHandlingByTransportId(string transportId)
        {
            var list = await _billOfLading.GetListHandlingByTransportId(transportId);

            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetHandlingById(int id)
        {
            var data = await _billOfLading.GetHandlingById(id);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetRuning(int id)
        {
            var update = await _billOfLading.SetRunning(id);

            if (update.isSuccess)
            {
                return Ok(update.Message);
            }
            else
            {
                return BadRequest(update.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CancelHandling(int id)
        {
            var update = await _billOfLading.CancelHandling(id);

            if (update.isSuccess)
            {
                return Ok(update.Message);
            }
            else
            {
                return BadRequest(update.Message);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateHandling(int id, UpdateHandling request)
        {
            var update = await _billOfLading.UpdateHandling(id, request);

            if (update.isSuccess)
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
        public async Task<IActionResult> GetListImage(int handlingId)
        {
            var list = await _billOfLading.GetListImageByHandlingId(handlingId);
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> DeleteImage(int fileId)
        {
            var del = await _billOfLading.DeleteImageById(fileId);

            if (del.isSuccess)
            {
                return Ok(del.Message);
            }
            else
            {
                return BadRequest(del.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetImageById(int id)
        {
            var image = await _billOfLading.GetImageById(id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), image.FilePath);
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", image.FileName);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UploadFile([FromForm] UploadImagesHandling request)
        {
            var uploadFile = await _billOfLading.UploadFile(request);

            if (uploadFile.isSuccess)
            {
                return Ok(uploadFile.Message);
            }
            else
            {
                return BadRequest(uploadFile.Message);
            }
        }
    }
}