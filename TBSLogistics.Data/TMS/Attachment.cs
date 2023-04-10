using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class Attachment
    {
        public Attachment()
        {
            TepChungTu = new HashSet<TepChungTu>();
            TepHopDong = new HashSet<TepHopDong>();
        }

        public long Id { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public string MaHopDong { get; set; }
        public DateTime? UploadedTime { get; set; }
        public string Updater { get; set; }
        public string Creator { get; set; }
        public string Note { get; set; }

        public virtual ICollection<TepChungTu> TepChungTu { get; set; }
        public virtual ICollection<TepHopDong> TepHopDong { get; set; }
    }
}
