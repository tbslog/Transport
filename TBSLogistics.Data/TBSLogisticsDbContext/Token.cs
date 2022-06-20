using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TBSLogisticsDbContext
{
    public partial class Token
    {
        public int UserId { get; set; }
        public string TokenCode { get; set; }
        public DateTime TimeOut { get; set; }

        public virtual User User { get; set; }
    }
}
