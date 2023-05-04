using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ThaoTacTaiXe
    {
        public long Id { get; set; }
        public long MaDieuPhoi { get; set; }
        public int TrangThai { get; set; }
        public DateTime ThoiGianThaoTac { get; set; }
        public string Creator { get; set; }

        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
    }
}
