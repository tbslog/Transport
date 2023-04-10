using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class TepChungTu
    {
        public long Id { get; set; }
        public long MaDieuPhoi { get; set; }
        public long MaHinhAnh { get; set; }
        public string TenChungTu { get; set; }
        public int LoaiChungTu { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }

        public virtual LoaiChungTu LoaiChungTuNavigation { get; set; }
        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
        public virtual Attachment MaHinhAnhNavigation { get; set; }
    }
}
