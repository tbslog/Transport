using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillModel
{
    public class ListBillTransportWeb
    {
        public string MaVanDon { get; set; }
        public string BookingNo { get; set; }
        public decimal TongTien { get; set; }
        public double TongPhuPhi { get; set; }
        public string HangTau { get; set; }
        public string TenKH { get; set; }
        public string Account { get; set; }
        public string LoaiVanDon { get; set; }
        public string MaPTVC { get; set; }
        public List<ListBillHandlingWeb> listBillHandlingWebs { get; set; }
    }

    public class ListBillHandlingWeb
    {
        public long handlingId { get; set; }
        public string DonViVanTai { get; set; }
        public string DiemDau { get; set; }
        public string DiemCuoi { get; set; }
        public string LoaiHangHoa { get; set; }
        public string LoaiPhuongTien { get; set; }
        public string MaSoXe { get; set; }
        public string TaiXe { get; set; }
        public decimal DonGiaKH { get; set; }
        public double PhuPhiHD { get; set; }
        public double PhuPhiPhatSinh { get; set; }
        public string ContNo { get; set; }
        public string SealNP { get; set; }
        public string SealHQ { get; set; }
        public string DiemLayRong { get; set; }
        public string DiemTraRong { get; set; }
    }
}
