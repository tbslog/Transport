using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.UserModel
{
	public class SetFieldRequired
	{
		public string CusId { get; set; }
		public string AccId { get; set; }
		public List<string> Fields { get; set; }
	}
}
