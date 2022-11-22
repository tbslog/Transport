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
        public List<TotalReport> TotalReports { get; set; }
        public List<DateTime> Labels { get; set; }
        public List<DataReport> Data { get; set; }
    }

    public class TotalReport
    {
        public string Title { get; set; }
        public int TotalInt { get; set; }
        public double TotalDouble { get; set; }
    }

    public class DataReport
    {
        public List<arrInt> DataInt { get; set; }
        public List<arrDouble> DataDouble { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class arrInt
    {
        public int count { get; set; }
        public DateTime date { get; set; }
    }

    public class arrDouble
    {
        public double value { get; set; }
        public DateTime date { get; set; }
    }
}
