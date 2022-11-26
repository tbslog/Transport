using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ThongBao
    {
        public int Id { get; set; }
        public string LangId { get; set; }
        public int TextId { get; set; }
        public string TextContent { get; set; }
        public string TextType { get; set; }
        public string FunctionId { get; set; }
    }
}
