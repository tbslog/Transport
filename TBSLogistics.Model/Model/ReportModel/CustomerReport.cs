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
		public int totalBooking { get; set; }
		public int Total { get; set; }
		public double totalMoney { get; set; }
		public double totalSf { get; set; }
		public double profit { get; set; }
		public int CONT20 { get; set; }
		public int CONT40 { get; set; }
		public int CONT40RF { get; set; }
		public int CONT45 { get; set; }
		public int TRUCK1 { get; set; }
		public int TRUCK15 { get; set; }
		public int TRUCK17 { get; set; }
		public int TRUCK10 { get; set; }
		public int TRUCK150 { get; set; }
		public int TRUCK2 { get; set; }
		public int TRUCK25 { get; set; }
		public int TRUCK3 { get; set; }
		public int TRUCK35 { get; set; }
		public int TRUCK5 { get; set; }
		public int TRUCK7 { get; set; }
		public int TRUCK8 { get; set; }
		public int TRUCK9 { get; set; }
	}

	public class DataReportOfCustomer
	{
		public string customer { get; set; }
		public double totalSf { get; set; }
		public double totalMoney { get; set; }
		public double profit { get; set; }
	}
}
