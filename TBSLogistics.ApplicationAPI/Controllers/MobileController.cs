using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.BillOfLadingModel;
using TBSLogistics.Model.Model.FileModel;
using TBSLogistics.Model.Model.MobileModel;
using TBSLogistics.Model.Model.SFeeByTcommandModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Services.BillOfLadingManage;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.MobileManager;
using TBSLogistics.Service.Services.SubFeePriceManage;
using TBSLogistics.Service.Services.UserManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly ISubFeePrice _subFeePrice;
        private readonly IBillOfLading _billOfLading;
        private readonly IMobile _mobile;
        private readonly TMSContext _tMSContext;
        private readonly ICommon _common;
        private readonly IUser _user;

        public MobileController(IBillOfLading billOfLading, ICommon common, IMobile mobile, ISubFeePrice subFeePrice, TMSContext tMSContext, IUser user)
        {
            _billOfLading = billOfLading;
            _mobile = mobile;
            _common = common;
            _subFeePrice = subFeePrice;
            _tMSContext = tMSContext;
            _user = user;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ResetStatus(string maChuyen)
        {
            var reset = await _mobile.ResetStatus(maChuyen);

            return Ok(reset);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubFeeSelect(int placeId)
        {
            var list = await _subFeePrice.GetListSubFeeSelect(placeId);
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetDataTransport(string driver, bool isCompleted)
        {
            var data = await _mobile.GetDataTransportForMobile(driver, isCompleted);
            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSubfeeIncurred(string maChuyen, int placeId)
        {
            var data = await _mobile.GetListSubfeeIncurred(maChuyen, placeId);
            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateContNo(string maChuyen, string contNo)
        {
            var update = await _mobile.UpdateContNo(maChuyen, contNo);

            if (update.isSuccess)
            {
                return Ok(update);
            }
            else
            {
                return BadRequest(update);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateDoc([FromForm] CreateDoc request)
        {
            var checkPermission = await _common.CheckPermission("F0006");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var uploadFile = await _mobile.CreateDoc(request);

            if (uploadFile.isSuccess)
            {
                return Ok(uploadFile);
            }
            else
            {
                return BadRequest(uploadFile);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListDocType(int placeId)
        {
            var list = await _tMSContext.LoaiChungTu.ToListAsync();
            var getPlace = await _tMSContext.DiaDiem.Where(x => x.MaDiaDiem == placeId).FirstOrDefaultAsync();
            list = list.Where(x => x.MaLoaiDiaDiem == getPlace.NhomDiaDiem).ToList();
            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateSFeeByTCommand(List<CreateSFeeByTCommandMobile> request, string maChuyen)
        {
            var checkPermission = await _common.CheckPermission("F0009");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var Create = await _mobile.CreateSFeeByTCommand(request, maChuyen);

            if (Create.isSuccess == true)
            {
                return Ok(Create);
            }
            else
            {
                return BadRequest(Create);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangeStatusHandling(int id, string maChuyen)
        {
            var checkPermission = await _common.CheckPermission("F0007");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _billOfLading.ChangeStatusHandling(id, maChuyen);

            if (update.isSuccess)
            {
                return Ok(update);
            }
            else
            {
                return BadRequest(update);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> WriteNoteHandling(int handlingId, string note)
        {
            var update = await _mobile.WriteNoteHandling(handlingId, note);

            if (update.isSuccess)
            {
                return Ok(update);
            }
            else
            {
                return BadRequest(update);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListImage(int handlingId)
        {
            var checkPermission = await _common.CheckPermission("F0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var list = await _billOfLading.GetListImageByHandlingId(handlingId);
            var listImage = new List<GetListImage>();

            foreach (var item in list)
            {
                var image = await _billOfLading.GetImageById(item.MaHinhAnh);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), image.FilePath);
                if (!System.IO.File.Exists(filePath))
                    return NotFound();

                var memory = new MemoryStream();
                await using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var file = File(memory, "application/octet-stream", image.FileName);
                byte[] array = new byte[file.FileStream.Length];
                // reading the data
                file.FileStream.Read(array, 0, array.Length);
                // decod bytes in a row
                string textFromFile = System.Text.Encoding.Default.GetString(array);
                var base64 = Convert.ToBase64String(array);

                listImage.Add(new GetListImage()
                {
                    FileName = item.TenChungTu,
                    FileType = item.TenLoaiChungTu,
                    Note = item.GhiChu,
                    Image = base64
                });
            }

            return Ok(listImage);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LogGPS(string maChuyen, LogGPSByMobile request)
        {
            var log = await _mobile.LogGPS(request, maChuyen);

            if (log.isSuccess == true)
            {
                return Ok(log);
            }

            return BadRequest(log);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CancelHandling(int handlingId, string note)
        {
            var cancel = await _billOfLading.CancelHandling(handlingId, note);

            if (cancel.isSuccess)
            {
                return Ok(cancel);
            }
            else
            {
                return BadRequest(cancel);
            }
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword(string username, ChangePasswordModel model)
        {
            var changePass = await _user.ChangePassword(username, model);

            if (changePass.isSuccess)
            {
                return Ok(changePass);
            }

            return BadRequest(changePass);
        }
    }
}