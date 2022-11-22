using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using TBSLogistics.Service.Services.Report;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReport _report;

        public ReportController(IReport report)
        {
            _report = report;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetReportTransportByMonth(DateTime dateTime)
        {
            var data = await _report.GetReportTransportByMonth(dateTime);

            return Ok(data);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetReportRevenue(DateTime dateTime)
        {
            var data = await _report.GetRevenue(dateTime);
            return Ok(data);
        }
    }
}
