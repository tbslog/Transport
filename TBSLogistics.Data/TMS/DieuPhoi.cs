using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class DieuPhoi
    {
        public DieuPhoi()
        {
            Attachment = new HashSet<Attachment>();
            SfeeByTcommand = new HashSet<SfeeByTcommand>();
            SubFeeByContract = new HashSet<SubFeeByContract>();
        }

        public long Id { get; set; }
        public string MaVanDon { get; set; }
        public string MaChuyen { get; set; }
        public string MaSoXe { get; set; }
        public string MaTaiXe { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public string MaDvt { get; set; }
        public string DonViVanTai { get; set; }
        public long BangGiaKh { get; set; }
        public long BangGiaNcc { get; set; }
        public decimal DonGiaKh { get; set; }
        public decimal DonGiaNcc { get; set; }
        public string MaRomooc { get; set; }
        public string ContNo { get; set; }
        public string SealNp { get; set; }
        public string SealHq { get; set; }
        public double? SoKien { get; set; }
        public double? KhoiLuong { get; set; }
        public double? TheTich { get; set; }
        public string GhiChu { get; set; }
        public int? DiemLayTraRong { get; set; }
        public DateTime? ThoiGianLayHangThucTe { get; set; }
        public DateTime? ThoiGianTraHangThucTe { get; set; }
        public DateTime? ThoiGianLayTraRongThucTe { get; set; }
        public DateTime? ThoiGianCoMatThucTe { get; set; }
        public DateTime? ThoiGianHoanThanh { get; set; }
        public int TrangThai { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }

        public virtual BangGia BangGiaKhNavigation { get; set; }
        public virtual BangGia BangGiaNccNavigation { get; set; }
        public virtual KhachHang DonViVanTaiNavigation { get; set; }
        public virtual Romooc MaRomoocNavigation { get; set; }
        public virtual XeVanChuyen MaSoXeNavigation { get; set; }
        public virtual TaiXe MaTaiXeNavigation { get; set; }
        public virtual VanDon MaVanDonNavigation { get; set; }
        public virtual ICollection<Attachment> Attachment { get; set; }
        public virtual ICollection<SfeeByTcommand> SfeeByTcommand { get; set; }
        public virtual ICollection<SubFeeByContract> SubFeeByContract { get; set; }
    }
}
