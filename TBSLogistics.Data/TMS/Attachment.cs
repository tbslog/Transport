using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class Attachment
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public string FolderName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public long? DieuPhoiId { get; set; }
        public string MaHopDong { get; set; }
        public DateTime? UploadedTime { get; set; }

        public virtual DieuPhoi DieuPhoi { get; set; }
        public virtual HopDongVaPhuLuc MaHopDongNavigation { get; set; }
    }
}
