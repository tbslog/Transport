using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.ReportModel
{
    public class GetDataReport
    {
        public string Title { get; set; }
        public int TotalTransport { get; set; }
        public int TotalHandling { get; set; }
        public List<DateTime> Labels { get; set; }
        public List<DataReport> Data{ get; set; }
    }

    public class DataReport
    {
        public List<arrDataReport> arrData { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class arrDataReport
    {
        public int count { get; set; }
        public DateTime date { get; set; }
    }
}
