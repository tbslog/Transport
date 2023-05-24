using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
	public class GetBill
	{
		public List<ListVanDon> BillReuslt { get; set; }
	}

	public class GetBillByTransport
	{
		public List<ListHandling> listHandlings { get; set; }
	}

	public class KyThanhToan
	{
		public int Ky { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}

	public class ListVanDon
	{
		public string MaVanDonKH { get; set; }
		public string MaVanDon { get; set; }
		public string MaKh { get; set; }
		public string TenKh { get; set; }
		public string AccountName { get; set; }
		public string LoaiVanDon { get; set; }
		public string DiemLayHang { get; set; }
		public string DiemTraHang { get; set; }
		public double? TongTheTich { get; set; }
		public double? TongKhoiLuong { get; set; }
		public double? TongSoKien { get; set; }
		public List<ListHandling> listHandling { get; set; }
	}

	public class ListHandling
	{
		public string MaSoXe { get; set; }
		public string DiemLayRong { get; set; }
		public string DiemTraRong { get; set; }
		public string MaRomooc { get; set; }
		public string TaiXe { get; set; }
		public string LoaiHangHoa { get; set; }
		public string LoaiPhuongTien { get; set; }
		public string DonViTinh { get; set; }
		public string DonViVanTai { get; set; }
		public string LoaiTienTe { get; set; }
		public decimal GiaQuyDoi { get; set; }
		public decimal DonGia { get; set; }
		public double? KhoiLuong { get; set; }
		public double? TheTich { get; set; }
		public double? SoKien { get; set; }

		public List<ListSubFeeByContract> listSubFeeByContract { get; set; }
		public List<ListSubFeeIncurred> listSubFeeIncurreds { get; set; }
	}

	public class ListSubFeeByContract
	{
		public string ContractId { get; set; }
		public string ContractName { get; set; }
		public string sfName { get; set; }
		public string goodsType { get; set; }
		public string AreaName { get; set; }
		public string TripName { get; set; }
		public double unitPrice { get; set; }
		public string priceType { get; set; }
		public double priceTransfer { get; set; }
	}

	public class ListSubFeeIncurred
	{
		public string sfName { get; set; }
		public double Price { get; set; }
		public string Note { get; set; }
	}

}
