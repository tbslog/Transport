using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.BillOfLadingModel;
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
            var arrDataTransport = getTransport.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrInt()
            {
                count = x.Count,
                date = x.Key.Date,
            }).ToList();
            arrDataTransport = arrDataTransport.Concat(getAllDaysInMonth.Where(x => !arrDataTransport.Select(y => y.date).Contains(x.Date)).Select(x => new arrInt()
            {
                count = 0,
                date = x.Date,
            })).ToList();

            var getHandling = await _context.DieuPhoi.Where(x => x.CreatedTime.Date >= firstDateOfMonth
            && x.CreatedTime.Date <= lastDateOfMonth
            && x.TrangThai == 20)
            .GroupBy(x => new { x.CreatedTime.Date })
            .Select(x => new { x.Key, Count = x.Count() }).ToListAsync();
            var arrDataHandling = getHandling.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.Key.Date)).Select(x => new arrInt()
            {
                count = x.Count,
                date = x.Key.Date,
            }).ToList();
            arrDataHandling = arrDataHandling.Concat(getAllDaysInMonth.Where(x => !arrDataHandling.Select(y => y.date).Contains(x.Date)).Select(x => new arrInt()
            {
                count = 0,
                date = x.Date,
            })).ToList();


            var arrHandling = new DataReport()
            {
                Name = "Số lượng chuyến",
                Color = "rgb(53, 162, 235)",
                DataInt = arrDataHandling
            };

            var arrTransport = new DataReport()
            {
                Name = "Số Lượng Vận Đơn",
                Color = "rgb(255, 99, 132)",
                DataInt = arrDataTransport,
            };

            var listTotal = new List<TotalReport>();
            listTotal.Add(new TotalReport { Title = "Tổng Vận Đơn", TotalInt = arrTransport.DataInt.Sum(x => x.count) });
            listTotal.Add(new TotalReport { Title = "Tổng Số Chuyến", TotalInt = arrHandling.DataInt.Sum(x => x.count) });

            return new GetDataReport()
            {
                Title = "Số Lượng Vận Đơn Và Chuyến Đã Hoàn Thành",
                TotalReports = listTotal,
                Labels = getAllDaysInMonth.Select(x => x.Date).ToList(),
                Data = new List<DataReport> { arrHandling, arrTransport },

            };
        }

        public async Task<GetDataReport> GetRevenue(DateTime dateTime)
        {
            var getAllDaysInMonth = Enumerable.Range(1, DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
           .Select(day => new DateTime(dateTime.Year, dateTime.Month, day)).ToList();

            var firstDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, 1);
            var lastDateOfMonth = new DateTime(dateTime.Year, dateTime.Month, DateTime.DaysInMonth(dateTime.Year, dateTime.Month));


            var getData = from dp in _context.DieuPhoi
                          join vd in _context.VanDon
                          on dp.MaVanDon equals vd.MaVanDon
                          where dp.TrangThai == 20
                          && dp.ThoiGianHoanThanh.Value.Date >= firstDateOfMonth.Date
                          && dp.ThoiGianHoanThanh.Value.Date <= lastDateOfMonth.Date
                          select new { dp, vd };

            var SubFee = await getData.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.dp.ThoiGianHoanThanh.Value.Date))
                .Select(x => new
                {
                    x.dp.ThoiGianHoanThanh.Value.Date,
                    totalSf = _context.SubFeePrice.Where(y =>
                    _context.HopDongVaPhuLuc.Where(z => z.MaKh == x.vd.MaKh && z.ThoiGianBatDau.Date <= DateTime.Now.Date && (z.ThoiGianKetThuc.Date > DateTime.Now.Date))
                    .Select(z => z.MaHopDong).Contains(y.ContractId) && y.Status == 14).Sum(y => y.UnitPrice) +
                    _context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.FinalPrice),
                }).ToListAsync();
            var listSubFee = SubFee.GroupBy(x => x.Date).Select(x => new
            {
                x.Key.Date,
                totalSum = x.Sum(x => x.totalSf)
            }).Select(x => new arrDouble()
            {
                date = x.Date,
                value = x.totalSum
            });
            listSubFee = listSubFee.Concat(getAllDaysInMonth.Where(x => !listSubFee.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
            {
                date = x.Date,
                value = 0,
            })).ToList();


            var revenue = await getData.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.dp.ThoiGianHoanThanh.Value.Date))
                  .Select(x => new
                  {
                      x.dp.ThoiGianHoanThanh.Value.Date,
                      totalPrice = _context.SubFeePrice.Where(y =>
                      _context.HopDongVaPhuLuc.Where(z => z.MaKh == x.vd.MaKh && z.ThoiGianBatDau.Date <= DateTime.Now.Date && (z.ThoiGianKetThuc.Date > DateTime.Now.Date))
                      .Select(z => z.MaHopDong).Contains(y.ContractId) && y.Status == 14).Sum(y => y.UnitPrice) +
                    _context.SfeeByTcommand.Where(y => y.IdTcommand == x.dp.Id && y.ApproveStatus == 14).Sum(y => y.FinalPrice) + ((double)x.dp.DonGiaKh),
                  }).ToListAsync();
            var listRevenue = revenue.GroupBy(x => x.Date).Select(x => new
            {
                x.Key.Date,
                totalSum = x.Sum(x => x.totalPrice)
            }).Select(x => new arrDouble()
            {
                date = x.Date,
                value = x.totalSum
            });
            listRevenue = listRevenue.Concat(getAllDaysInMonth.Where(x => !listRevenue.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
            {
                date = x.Date,
                value = 0,
            })).ToList();


            var Profit = await getData.Where(x => getAllDaysInMonth.Select(y => y.Date).Contains(x.dp.ThoiGianHoanThanh.Value.Date))
                .GroupBy(x => new { x.dp.ThoiGianHoanThanh.Value.Date })
                .Select(x => new { date = x.Key.Date, sumProfit = x.Sum(x => x.dp.DonGiaNcc - x.dp.DonGiaKh) }).Select(x => new arrDouble()
                {
                    date = x.date,
                    value = ((double)x.sumProfit),
                }).ToListAsync();
            Profit = Profit.Concat(getAllDaysInMonth.Where(x => !Profit.Select(y => y.date.Date).Contains(x.Date)).Select(x => new arrDouble()
            {
                date = x.Date,
                value = 0,
            })).ToList();


            var arrSubfee = new DataReport()
            {
                Name = "Chi Phí",
                Color = "rgb(53, 162, 235)",
                DataDouble = listSubFee.ToList(),
            };

            var arrRevenue = new DataReport()
            {
                Name = "Doanh Thu",
                Color = "rgb(255, 99, 132)",
                DataDouble = listRevenue.ToList(),
            };

            var arrProfit = new DataReport()
            {
                Name = "Lợi Nhuận",
                Color = "rgba(233,157,79,255)",
                DataDouble = Profit,
            };

            var listTotal = new List<TotalReport>();
            listTotal.Add(new TotalReport { Title = "Tổng Chi Phí", TotalDouble = arrSubfee.DataDouble.Sum(x => x.value) });
            listTotal.Add(new TotalReport { Title = "Tổng Doanh Thu", TotalDouble = arrRevenue.DataDouble.Sum(x => x.value) });
            listTotal.Add(new TotalReport { Title = "Tổng Lợi Nhuận", TotalDouble = arrProfit.DataDouble.Sum(x => x.value) });

            return new GetDataReport()
            {
                Title = "Doanh Thu, Chi Phí Lợi Nhuận",
                TotalReports = listTotal,
                Labels = getAllDaysInMonth.Select(x => x.Date).ToList(),
                Data = new List<DataReport> { arrSubfee, arrRevenue, arrProfit },
            };

        }
    }
}
