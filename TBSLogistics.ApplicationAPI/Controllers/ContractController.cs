using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> CreateContract([FromForm]CreateContract request)
        {
            var createContract = await _contract.CreateContract(request);

            if (createContract.isSuccess == true)
            {
                if (request.File != null )
                {
                    await SaveFile(request.File, request.TenHienThi, request.MaHopDong);
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
        public async Task<IActionResult> UpdateContract(string id,[FromForm] EditContract request)
        {
            var editContract = await _contract.EditContract(id, request);

            if (editContract.isSuccess == true)
            {
                if (request.File != null)
                {
                    await SaveFile(request.File, request.TenHienThi, id);
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
        public async Task<IActionResult> GetContractById(string id)
        {
            var contract = await _contract.GetContractById(id);
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


        private async Task<bool> SaveFile( IFormFile file, string cusName, string maHopDong)
        {
            var PathFolder = $"/Contract/{cusName}";

            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var reNameFile = originalFileName.Replace(originalFileName.Substring(0, originalFileName.LastIndexOf('.')), maHopDong);
            var fileName = $"{reNameFile.Substring(0, reNameFile.LastIndexOf('.'))}{Path.GetExtension(reNameFile)}";
            await _common.DeleteFileAsync(fileName, PathFolder);
            await _common.SaveFileAsync(file.OpenReadStream(), fileName, PathFolder);
            var attachment = new Attachment()
            {
                FileName = fileName,
                FilePath = _common.GetFileUrl(fileName, PathFolder),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                FolderName = "Contract"
            };

            var add = await _common.AddAttachment(attachment);

            return add.isSuccess;
        }
    }
}
