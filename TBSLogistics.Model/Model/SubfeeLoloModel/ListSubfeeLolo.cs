using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.SubfeeLoloModel
{
	public class ListSubfeeLolo
	{
		public long Id { get; set; }
		public int MaDiaDiem { get; set; }
		public string TenDiaDiem { get; set; }
		public string MaKh { get; set; }
		public string TenKh { get; set; }
		public string HangTau { get; set; }
		public string LoaiCont { get; set; }
		public string LoaiPhuPhi { get; set; }
		public decimal DonGia { get; set; }
		public int TrangThai { get; set; }
		public string TenTrangThai { get; set; }
		public string Approver { get; set; }
		public DateTime Createdtime { get; set; }
		public DateTime? ApproveDate { get; set; }
		public DateTime? DisableDate { get; set; }
	}
}
