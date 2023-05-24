using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class LogTimeUsedOfUsers
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string UserName { get; set; }
        public DateTime TimeLogin { get; set; }
        public DateTime? LastTimeRequest { get; set; }
    }
}
