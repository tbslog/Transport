using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubfeeLoloModel
{
	public class CreateOrUpdateSubfeeLolo
	{
		public int MaDiaDiem { get; set; }
		public string MaKh { get; set; }
		public string HangTau { get; set; }
		public string LoaiCont { get; set; }
		public int LoaiPhuPhi { get; set; }
		public decimal DonGia { get; set; }
	}

	public class GetSubfeeLoloById
	{
		public long Id { get; set; }
		public int MaDiaDiem { get; set; }
		public string MaKh { get; set; }
		public string HangTau { get; set; }
		public string LoaiCont { get; set; }
		public int LoaiPhuPhi { get; set; }
		public decimal DonGia { get; set; }
		public int TrangThai { get; set; }
	}
}
