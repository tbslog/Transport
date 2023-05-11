using System;
using System.Collections.Generic;

namespace TBSLogistics.Data.TMS
{
    public partial class FunctionsOfSystems
    {
        public FunctionsOfSystems()
        {
            FieldOfFunction = new HashSet<FieldOfFunction>();
            ValidateDataByCustomer = new HashSet<ValidateDataByCustomer>();
        }

        public string FunctionId { get; set; }
        public string FunctionName { get; set; }

        public virtual ICollection<FieldOfFunction> FieldOfFunction { get; set; }
        public virtual ICollection<ValidateDataByCustomer> ValidateDataByCustomer { get; set; }
    }
}
