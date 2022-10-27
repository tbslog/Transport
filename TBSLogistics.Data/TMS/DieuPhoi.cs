using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class DieuPhoi
    {
        public DieuPhoi()
        {
            Attachment = new HashSet<Attachment>();
        }

        public long Id { get; set; }
        public string MaVanDon { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaPtvc { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaDvt { get; set; }
        public string DonViVanTai { get; set; }
        public long IdbangGia { get; set; }
        public decimal GiaThamChieu { get; set; }
        public string Tau { get; set; }
        public string HangTau { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public string GhiChu { get; set; }
        public int? DiemLayTraRong { get; set; }
        public DateTime? ThoiGianLayTraRong { get; set; }
        public DateTime? ThoiGianHaCong { get; set; }
        public DateTime? ThoiGianKeoCong { get; set; }
        public DateTime? ThoiGianHanLenh { get; set; }
        public DateTime ThoiGianCoMat { get; set; }
        public DateTime? ThoiGianCatMang { get; set; }
        public DateTime ThoiGianLayHang { get; set; }
        public DateTime ThoiGianTraHang { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }

        public virtual KhachHang DonViVanTaiNavigation { get; set; }
        public virtual BangGia IdbangGiaNavigation { get; set; }
        public virtual Romooc MaRomoocNavigation { get; set; }
        public virtual XeVanChuyen MaSoXeNavigation { get; set; }
        public virtual TaiXe MaTaiXeNavigation { get; set; }
        public virtual VanDon MaVanDonNavigation { get; set; }
        public virtual ICollection<Attachment> Attachment { get; set; }
    }
}
