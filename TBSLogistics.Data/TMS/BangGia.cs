using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class BangGia
    {
        public BangGia()
        {
            DieuPhoiBangGiaKhNavigation = new HashSet<DieuPhoi>();
            DieuPhoiBangGiaNccNavigation = new HashSet<DieuPhoi>();
        }

        public long Id { get; set; }
        public string MaHopDong { get; set; }
        public string MaPtvc { get; set; }
        public string MaLoaiPhuongTien { get; set; }
        public decimal DonGia { get; set; }
        public string MaDvt { get; set; }
        public string MaLoaiHangHoa { get; set; }
        public string MaLoaiDoiTac { get; set; }
        public DateTime NgayApDung { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }
        public int TrangThai { get; set; }
        public DateTime UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? DeletedTime { get; set; }
        public string Approver { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }
        public int? DiemLayTraRong { get; set; }
        public int DiemDau { get; set; }
        public int DiemCuoi { get; set; }

        public virtual HopDongVaPhuLuc MaHopDongNavigation { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoiBangGiaKhNavigation { get; set; }
        public virtual ICollection<DieuPhoi> DieuPhoiBangGiaNccNavigation { get; set; }
    }
}
