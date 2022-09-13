using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.Common;
using TBSLogistics.Service.Services.ContractManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
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
            var createContract = await _contract.CreateContract(request);

            if (createContract.isSuccess == true)
            {
                if (request.File != null)
                {
                    await UploadFile(request.File, request.TenHienThi, request.MaHopDong);
                }

                return Ok(createContract.Message);
            }
            else
            {
                return BadRequest(createContract.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> UpdateContract(string Id, [FromForm] EditContract request)
        {
            var editContract = await _contract.EditContract(Id, request);

            if (editContract.isSuccess == true)
            {
                if (request.File != null)
                {
                    await UploadFile(request.File, request.TenHienThi, Id);
                }
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
            var contract = await _contract.GetContractById(Id);
            return Ok(contract);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContract([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _contract.GetListContract(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListContract>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
            return Ok(pagedReponse);
        }

        [HttpGet, DisableRequestSizeLimit]
        [Route("[action]")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var getFilePath = await _common.GetAttachmentById(fileId);

            if (getFilePath == null)
            {
                return BadRequest("File không tồn tại");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), getFilePath.FilePath);
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


        private async Task<bool> UploadFile(IFormFile file, string cusName, string maHopDong)
        {
            var PathFolder = $"/Contract/{cusName}";

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), maHopDong);
            var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";

            var attachment = new Attachment()
            {
                FileName = fileName,
                FilePath = _common.GetFileUrl(fileName, PathFolder),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                FolderName = "Contract"
            };

            var add = await _common.AddAttachment(attachment);

            if (add.isSuccess == false)
            {
                return false;
            }

           
            await _common.SaveFileAsync(file.OpenReadStream(), fileName, PathFolder);


            return true;
        }
    }
}
