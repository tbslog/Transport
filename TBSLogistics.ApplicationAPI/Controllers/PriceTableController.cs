using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.PriceListModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.PricelistManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PriceTableController : ControllerBase
    {
        private readonly IPriceTable _priceTable;
        private readonly IPaginationService _paninationService;
        private readonly ICommon _common;
        private readonly TMSContext _context;

        public PriceTableController(IPriceTable priceTable, IPaginationService paninationService, ICommon common, TMSContext context)
        {
            _priceTable = priceTable;
            _paninationService = paninationService;
            _common = common;
            _context = context;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreatePriceTable(List<CreatePriceListRequest> request)
        {
            var checkPermission = await _common.CheckPermission("C0003");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var create = await _priceTable.CreatePriceTable(request);

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
        public async Task<IActionResult> GetListPriceTable([FromQuery] PaginationFilter filter)
        {
            var checkPermissionKH = await _common.CheckPermission("C0001");
            var checkPermissionNCC = await _common.CheckPermission("C0005");

            if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
            {
                return BadRequest("Bạn không có quyền hạn");
            }


            if (filter.customerType == "NCC")
            {
                if (!checkPermissionNCC.isSuccess)
                {
                    return BadRequest("Bạn không có quyền hạn");
                }
            }

            if(filter.customerType == "KH")
            {
                if (!checkPermissionKH.isSuccess)
                {
                    return BadRequest("Bạn không có quyền hạn");
                }
            }
           

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTable(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListCustomerOfPriceTable>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableByContractId([FromQuery] PaginationFilter filter, ListFilter listFilter, string Id, string onlyct = null)
        {
            var checkPermissionKH = await _common.CheckPermission("C0001");
            var checkPermissionNCC = await _common.CheckPermission("C0005");

            if (!checkPermissionKH.isSuccess && !checkPermissionNCC.isSuccess)
            {
                return BadRequest("Bạn không có quyền hạn");
            }

            var checkContract = await _context.HopDongVaPhuLuc.Where(x => x.MaHopDong == Id).FirstOrDefaultAsync();

            if (checkContract.MaKh.Contains("SUP"))
            {
                if (!checkPermissionNCC.isSuccess)
                {
                    return BadRequest("Bạn không có quyền hạn");
                }
            }

            if (checkContract.MaKh.Contains("CUS"))
            {
                if (!checkPermissionKH.isSuccess)
                {
                    return BadRequest("Bạn không có quyền hạn");
                }
            }

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableByContractId(Id, onlyct, listFilter, filter);
            var pagedReponse = PaginationHelper.CreatePagedReponse<GetPriceListRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableByCustomerId(string Id)
        {
            var checkPermission = await _common.CheckPermission("C0001");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var list = await _priceTable.GetListPriceTableByCustommerId(Id);

            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListPriceTableApprove([FromQuery] PaginationFilter filter)
        {
            var checkPermission = await _common.CheckPermission("C0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var route = Request.Path.Value;
            var pagedData = await _priceTable.GetListPriceTableApprove(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<ListApprove>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _paninationService, route);
            return Ok(pagedReponse);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ApprovePriceTable(ApprovePriceTable request)
        {
            var checkPermission = await _common.CheckPermission("C0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var approve = await _priceTable.ApprovePriceTable(request);

            if (approve.isSuccess == true)
            {
                return Ok(approve.Message);
            }
            else
            {
                return BadRequest(approve.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetPriceTableById(int id)
        {
            var checkPermission = await _common.CheckPermission("C0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var data = await _priceTable.GetPriceTableById(id);

            return Ok(data);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdatePriceTable(int id, GetPriceListById request)
        {
            var checkPermission = await _common.CheckPermission("C0004");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }

            var update = await _priceTable.UpdatePriceTable(id, request);

            if (update.isSuccess)
            {
                return Ok(update.Message);
            }
            else
            {
                return BadRequest(update.Message);
            }
        }
    }
}