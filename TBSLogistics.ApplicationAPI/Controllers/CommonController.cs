using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly TMSContext _tMSContext;

        public CommonController(TMSContext tMSContext)
        {
            _tMSContext = tMSContext;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListVehicleType()
        {
            var list = await _tMSContext.LoaiPhuongTien.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListDVT()
        {
            var list = await _tMSContext.DonViTinh.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListGoodsType()
        {
            var list = await _tMSContext.LoaiHangHoa.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListContractType()
        {
            var list = await _tMSContext.LoaiHopDong.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListTransportType()
        {
            var list = await _tMSContext.PhuongThucVanChuyen.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustommerGroup()
        {
            var list = await _tMSContext.NhomKhachHang.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListCustommerType()
        {
            var list = await _tMSContext.LoaiKhachHang.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListArea()
        {
            var list = await _tMSContext.KhuVuc.ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListShipping()
        {
            var list = await _tMSContext.ShippingInfomation.ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListStatus(List<string> funcId)
        {
            var list = await _tMSContext.StatusText.Where(x => x.LangId == "VI" && funcId.Contains(x.FunctionId)).Select(x => new { x.StatusId, x.StatusContent }).ToListAsync();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListUser()
        {
            var listUser = from user in _tMSContext.NguoiDung
                           join acc in _tMSContext.Account
                           on user.Id equals acc.Id
                           where user.MaBoPhan == "BP0004"
                           select new { acc, user };
            var list = await listUser.Select(x => new ListUser()
            {
                userName = x.acc.UserName,
                name = x.user.HoVaTen
            }).ToListAsync();

            return Ok(list);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListMessage(List<string> funcId)
        {
            var list = await _tMSContext.ThongBao.Where(x => x.LangId == "VI" && funcId.Contains(x.FunctionId)).Select(x => new { x.TextId, x.TextContent }).ToListAsync();
            return Ok(list);
        }
    }
}