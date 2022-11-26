using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class Log
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string Message { get; set; }
        public DateTime LogTime { get; set; }
    }
}
