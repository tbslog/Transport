using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ReportModel
{
    public class TransportReport
    {
        public List<CustomerReport> customerReports { get; set; }
        public List<CustomerReport> supllierReports { get; set; }
    }

    public class CustomerReport
    {
        public string CustomerName { get; set; }
        public int Total { get; set; }
        public int InPut { get; set; }
        public int OutPut { get; set; }
        public int FCL { get; set; }
        public int FTL { get; set; }
        public int LTL { get; set; }
        public int LCL { get; set; }
    }
}
