using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.ReportModel;

namespace TBSLogistics.Service.Services.Report
{
    public interface IReport
    {
        Task<GetDataReport> GetReportTransportByMonth(DateTime dateTime);
        Task<GetDataReport> GetRevenue(DateTime dateTime);
        Task<TransportReport> GetCustomerReport(DateTime fromDate, DateTime toDate);
    }
}
