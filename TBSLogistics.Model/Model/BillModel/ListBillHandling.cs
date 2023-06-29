using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
	public class ListBillHandling
	{
		public string HangTau { get; set; }
		public long handlingId { get; set; }
		public string ContNo { get; set; }
		public DateTime? CutOffDate { get; set; }
		public DateTime createdTime { get; set; }
		public DateTime? ThoiGianHoanThanh { get; set; }
		public string DiemDau { get; set; }
		public string DiemCuoi { get; set; }
		public string DiemLayRong { get; set; }
		public string DiemTraRong { get; set; }
		public string MaPTVC { get; set; }
		public string LoaiVanDon { get; set; }
		public string MaVanDonKH { get; set; }
		public string MaChuyen { get; set; }
		public string MaVanDon { get; set; }
		public string LoaiHangHoa { get; set; }
		public string LoaiPhuongTien { get; set; }
		public string MaKH { get; set; }
		public string MaNCC { get; set; }
		public string TenKH { get; set; }
		public string AccountName { get; set; }
		public string TenNCC { get; set; }
		public decimal? DonGiaKH { get; set; }
		public decimal? DonGiaNCC { get; set; }
		public string LoaiTienTeKH { get; set; }
		public string LoaiTienTeNCC { get; set; }
		public decimal DoanhThu { get; set; }
		public decimal LoiNhuan { get; set; }
		public decimal ChiPhiHopDong { get; set; }
		public decimal ChiPhiPhatSinh { get; set; }
		public string ListSubfeeContract { get; set; }
		public string ListSubfeeIncurred { get; set; }

		public string Reuse { get; set; }
	}
}
