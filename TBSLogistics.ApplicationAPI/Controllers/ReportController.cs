using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.Report;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReport _report;
        private readonly ICommon _common;

        public ReportController(IReport report,ICommon common)
        {
            _common = common;
            _report = report;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetReportTransportByMonth(DateTime dateTime)
        {
            var checkPermission = await _common.CheckPermission("H0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var data = await _report.GetReportTransportByMonth(dateTime);

            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetReportRevenue(DateTime dateTime)
        {
            var checkPermission = await _common.CheckPermission("H0002");
            if (checkPermission.isSuccess == false)
            {
                return BadRequest(checkPermission.Message);
            }
            var data = await _report.GetRevenue(dateTime);
            return Ok(data);
        }
    }
}
