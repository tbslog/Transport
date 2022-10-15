using System;
using System.Collections.Generic;

#nullable disable

namespace TBSLogistics.Data.TMS
{
    public partial class StatusText
    {
        public int Id { get; set; }
        public string LangId { get; set; }
        public int StatusId { get; set; }
        public string StatusContent { get; set; }
        public string FunctionId { get; set; }
    }
}
