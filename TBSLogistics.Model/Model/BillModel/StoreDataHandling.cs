using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
	public class StoreDataHandling
	{
		public string cusId { get; set; }
		public List<long> ids { get; set; }	
		public DateTime dateBlock { get; set; }	
	}
}
