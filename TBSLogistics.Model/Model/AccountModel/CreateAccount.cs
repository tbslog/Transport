using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AccountModel
{
    public class CreateOrUpdateAccount
    {
        public string AccountName { get; set; }
        public List<string> ListCustomer { get; set; }
    }
}
