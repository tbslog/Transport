using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class DiaDiem
    {
        public DiaDiem()
        {
            InverseDiaDiemChaNavigation = new HashSet<DiaDiem>();
        }

        public int MaDiaDiem { get; set; }
        public int? DiaDiemCha { get; set; }
        public string LoaiDiaDiem { get; set; }
        public string TenDiaDiem { get; set; }
        public int? MaQuocGia { get; set; }
        public int? MaTinh { get; set; }
        public int? MaHuyen { get; set; }
        public int? MaPhuong { get; set; }
        public string SoNha { get; set; }
        public string DiaChiDayDu { get; set; }
        public string MaGps { get; set; }
        public string MaLoaiDiaDiem { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }

        public virtual DiaDiem DiaDiemChaNavigation { get; set; }
        public virtual QuanHuyen MaHuyenNavigation { get; set; }
        public virtual LoaiDiaDiem MaLoaiDiaDiemNavigation { get; set; }
        public virtual XaPhuong MaPhuongNavigation { get; set; }
        public virtual QuocGia MaQuocGiaNavigation { get; set; }
        public virtual TinhThanh MaTinhNavigation { get; set; }
        public virtual ICollection<DiaDiem> InverseDiaDiemChaNavigation { get; set; }
    }
}
