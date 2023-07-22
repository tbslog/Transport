using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ShippingInfomation
    {
        public ShippingInfomation()
        {
            PhuPhiNangHa = new HashSet<PhuPhiNangHa>();
        }

        public string ShippingCode { get; set; }
        public string ShippingLineName { get; set; }

        public virtual ICollection<PhuPhiNangHa> PhuPhiNangHa { get; set; }
    }
}
