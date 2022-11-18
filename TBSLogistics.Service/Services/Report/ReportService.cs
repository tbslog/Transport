using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.ReportModel;
using TBSLogistics.Service.Repository.Common;

namespace TBSLogistics.Service.Services.Report
{
    public class ReportService : IReport
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;

        public ReportService(TMSContext context, ICommon common)
        {
            _context = context;
            _common = common;
        }

        public async Task<GetDataReport> GetReportTransportByMonth(DateTime dateTime)
        {
            var getAllDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
            .Select(day => new DateTime(dateTime.Year, dateTime.Month, day)).ToList();

            var firstDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            var lastDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));


            var getTransport = await _context.VanDon.Where(x =>
            x.ThoiGianTaoDon.Date >= firstDateOfMonth
            && x.ThoiGianTaoDon <= lastDateOfMonth && x.TrangThai == 22)
                .GroupBy(x => new { x.ThoiGianTaoDon.Date })
                .Select(g => new { g.Key, Count = g.Count() }).ToListAsync();
            var arrDataTransport = getTransport.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrDataReport()
            {
                count = x.Count,
                date = x.Key.Date,
            }).ToList();
            arrDataTransport = arrDataTransport.Concat(getAllDaysInMonth.Where(x => !arrDataTransport.Select(y => y.date).Contains(x.Date)).Select(x => new arrDataReport()
            {
                count = 0,
                date = x.Date,
            })).ToList();



            var getHandling = await _context.DieuPhoi.Where(x => x.CreatedTime.Date >= firstDateOfMonth
            && x.CreatedTime.Date <= lastDateOfMonth
            && x.TrangThai == 20)
            .GroupBy(x => new { x.CreatedTime.Date })
            .Select(x => new { x.Key, Count = x.Count() }).ToListAsync();
            var arrDataHandling = getHandling.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrDataReport()
            {
                count = x.Count,
                date = x.Key.Date,
            }).ToList();
            arrDataHandling = arrDataHandling.Concat(getAllDaysInMonth.Where(x => !arrDataHandling.Select(y => y.date).Contains(x.Date)).Select(x => new arrDataReport()
            {
                count = 0,
                date = x.Date,
            })).ToList();


            var arrHandling = new DataReport()
            {
                Name = "Số lượng chuyến",
                Color = "rgb(53, 162, 235)",
                arrData = arrDataHandling
            };

            var arrTransport = new DataReport()
            {
                Name = "Số Lượng Vận Đơn",
                Color = "rgb(255, 99, 132)",
                arrData = arrDataTransport,
            };

            return new GetDataReport()
            {
                Title = "Số Lượng Vận Đơn Và Chuyến Đã Hoàn Thành",
                TotalTransport = arrTransport.arrData.Sum(x => x.count),
                TotalHandling = arrHandling.arrData.Sum(x => x.count),
                Labels = getAllDaysInMonth.Select(x => x.Date).ToList(),
                Data = new List<DataReport> { arrHandling, arrTransport },

            };
        }
    }
}
