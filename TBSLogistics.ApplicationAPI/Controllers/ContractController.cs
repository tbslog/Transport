using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.ContractManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContract _contract;
        private readonly ICommon _common;
        private readonly IPaginationService _pagination;

        public ContractController(IContract contract, IPaginationService pagination, ICommon common)
        {
            _contract = contract;
            _pagination = pagination;
            _common = common;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateContract([FromForm] CreateContract request)
        {
            var checkPermission = await _common.CheckPermission("B0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var createContract = await _contract.CreateContract(request);

            if (createContract.isSuccess == true)
            {
                return Ok(createContract.Message);
            }
            else
            {
                return BadRequest(createContract.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListOptionSelect()
        {
            var list = await _contract.GetListOptionSelect();
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateContract(string Id, [FromForm] EditContract request)
        {
            var checkPermission = await _common.CheckPermission("B0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var editContract = await _contract.EditContract(Id, request);

            if (editContract.isSuccess == true)
            {
                return Ok(editContract.Message);
            }
            else
            {
                return BadRequest(editContract.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetContractById(string Id)
        {
            var checkPermission = await _common.CheckPermission("B0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var contract = await _contract.GetContractById(Id);
            return Ok(contract);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContract([FromQuery] PaginationFilter filter)
        {
            if (filter.customerType == "KH")
            {
                var checkPermission = await _common.CheckPermission("B0004");
                if (checkPermission.isSuccess == false)
                {
                    return BadRequest(checkPermission.Message);
                }
            }

            if (filter.customerType == "NCC")
            {
                var checkPermission = await _common.CheckPermission("B0005");
                if (checkPermission.isSuccess == false)
                {
                    return BadRequest(checkPermission.Message);
                }
            }

            var route = Request.Path.Value;
            var pagedData = await _contract.GetListContract(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListContract>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContractApprove([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _contract.GetListContractApprove(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListContract>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApproveContract(List<ApproveContract> request)
        {
            var checkPermission = await _common.CheckPermission("B0005");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var add = await _contract.ApproveContract(request);

            if (add.isSuccess == true)
            {
                return Ok(add.Message);
            }
            else
            {
                return BadRequest(add.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContractSelect(string MaKH = null, bool getChild = true, bool getProductService = false, bool getListApprove = false)
        {
            var list = await _contract.GetListContractSelect(MaKH, getChild, getProductService, getListApprove);
            return Ok(list);
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("[action]")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var checkPermission = await _common.CheckPermission("B0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var getFilePath = await _common.GetAttachmentById(fileId);

            if (getFilePath == null)
            {
                return BadRequest("File không tồn tại");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _common.GetFile(getFilePath.FilePath));
            if (!System.IO.File.Exists(filePath))
                return NotFound();
            var memory = new MemoryStream();
            await using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), filePath);
        }

        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;

            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }

            return contentType;
        }
    }
}