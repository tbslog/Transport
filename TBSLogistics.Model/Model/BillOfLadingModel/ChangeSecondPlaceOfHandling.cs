using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
	public class ChangeSecondPlaceOfHandling
	{
		public string transportId { get; set; }
		public long handlingId { get; set; }
		public int newSecondPlace { get; set; }
	}

	public class SetSupplierForHandling
	{
		public string supplierId { get; set; }
		public List<long> handlingIds { get; set; }
	}
}
