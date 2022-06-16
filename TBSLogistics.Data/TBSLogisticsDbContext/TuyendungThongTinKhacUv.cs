using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class TuyendungThongTinKhacUv
    {
        public int Id { get; set; }
        public bool? DaLamTaiTbs { get; set; }
        public string ThongTinViTriCu { get; set; }
        public bool? CoNguoiQuenTrongCongTy { get; set; }
        public string ThongTinNguoiQuenCongTy { get; set; }
        public bool? CoNguoiQuenTrongCongTyDoiThu { get; set; }
        public string ThongTinNguoiQuenCongTyDoiThu { get; set; }
        public string LyDoNopDonVaoTbs { get; set; }
        public string HoatDongUaThich { get; set; }
        public string MonTheThaoUaThich { get; set; }
        public string MucDoLuyenTap { get; set; }
        public string SdtnguoiThan1 { get; set; }
        public string QuanHe1 { get; set; }
        public string SdtnguoiThan2 { get; set; }
        public string QuanHe2 { get; set; }
        public string SdtnguoiThan3 { get; set; }
        public string QuanHe3 { get; set; }
        public string BietThongTinTuyenDungQua { get; set; }
        public int? IdungVien { get; set; }

        public virtual TuyendungThongTinUngVien IdungVienNavigation { get; set; }
    }
}
