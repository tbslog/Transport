using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class PhuPhiNangHa
    {
        public PhuPhiNangHa()
        {
            PhuPhiNangHaTheoChuyen = new HashSet<PhuPhiNangHaTheoChuyen>();
        }

        public long Id { get; set; }
        public int MaDiaDiem { get; set; }
        public string MaKh { get; set; }
        public string HangTau { get; set; }
        public string LoaiCont { get; set; }
        public int LoaiPhuPhi { get; set; }
        public decimal DonGia { get; set; }
        public int TrangThai { get; set; }
        public string Creator { get; set; }
        public string Approver { get; set; }
        public DateTime? NgayApDung { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        public virtual ShippingInfomation HangTauNavigation { get; set; }
        public virtual DiaDiem MaDiaDiemNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
        public virtual ICollection<PhuPhiNangHaTheoChuyen> PhuPhiNangHaTheoChuyen { get; set; }
    }
}
