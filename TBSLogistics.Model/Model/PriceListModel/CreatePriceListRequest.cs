using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.PriceListModel
{
	public class CreatePriceListRequest
	{
		public int? DiemLayTraRong { get; set; }
		public string AccountId { get; set; }
		public int DiemDau { get; set; }
		public int DiemCuoi { get; set; }
		public string MaHopDong { get; set; }
		public string MaKH { get; set; }
		public string MaPtvc { get; set; }
		public string MaLoaiPhuongTien { get; set; }
		public decimal DonGia { get; set; }
		public string LoaiTienTe { get; set; }
		public string MaDvt { get; set; }
		public string MaLoaiHangHoa { get; set; }
		public string MaLoaiDoiTac { get; set; }
		public DateTime? NgayHetHieuLuc { get; set; }
	}
}
