using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class ValidateDataByCustomer
    {
        public int Id { get; set; }
        public string MaKh { get; set; }
        public string MaAccount { get; set; }
        public string FunctionId { get; set; }
        public string FieldId { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }

        public virtual FieldOfFunction Field { get; set; }
        public virtual FunctionsOfSystems Function { get; set; }
        public virtual AccountOfCustomer MaAccountNavigation { get; set; }
        public virtual KhachHang MaKhNavigation { get; set; }
    }
}
