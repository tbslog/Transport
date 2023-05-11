using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class FieldOfFunction
    {
        public FieldOfFunction()
        {
            ValidateDataByCustomer = new HashSet<ValidateDataByCustomer>();
        }

        public string FieldId { get; set; }
        public string FieldName { get; set; }
        public string FunctionId { get; set; }

        public virtual FunctionsOfSystems Function { get; set; }
        public virtual ICollection<ValidateDataByCustomer> ValidateDataByCustomer { get; set; }
    }
}
