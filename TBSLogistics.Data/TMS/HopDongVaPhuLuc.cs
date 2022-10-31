using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class HopDongVaPhuLuc
    {
        public HopDongVaPhuLuc()
        {
            Attachment = new HashSet<Attachment>();
            BangGia = new HashSet<BangGia>();
            InverseMaHopDongChaNavigation = new HashSet<HopDongVaPhuLuc>();
            SubFeePrice = new HashSet<SubFeePrice>();
        }

        public string MaHopDong { get; set; }
        public string MaHopDongCha { get; set; }
        public string TenHienThi { get; set; }
        public string MaKh { get; set; }
        public string MaLoaiHopDong { get; set; }
        public DateTime ThoiGianBatDau { get; set; }
        public DateTime ThoiGianKetThuc { get; set; }
        public string GhiChu { get; set; }
        public string MaPhuPhi { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }

        public virtual HopDongVaPhuLuc MaHopDongChaNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<Attachment> Attachment { get; set; }
        public virtual ICollection<BangGia> BangGia { get; set; }
        public virtual ICollection<HopDongVaPhuLuc> InverseMaHopDongChaNavigation { get; set; }
        public virtual ICollection<SubFeePrice> SubFeePrice { get; set; }
    }
}
