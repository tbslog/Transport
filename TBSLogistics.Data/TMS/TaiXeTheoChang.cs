using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class TaiXeTheoChang
    {
        public long MaDieuPhoi { get; set; }
        public string MaTaiXe { get; set; }
        public int SoChan { get; set; }
        public int TrangThaiTheoChang { get; set; }

        public virtual DieuPhoi MaDieuPhoiNavigation { get; set; }
        public virtual TaiXe MaTaiXeNavigation { get; set; }
    }
}
