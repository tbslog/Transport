using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class TepHopDong
    {
        public long Id { get; set; }
        public string MaHopDong { get; set; }
        public long MaTepHongDong { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }

        public virtual HopDongVaPhuLuc MaHopDongNavigation { get; set; }
        public virtual Attachment MaTepHongDongNavigation { get; set; }
    }
}
