using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.AccountModel
{
    public class GetAccountById
    {
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public List<string> ListCustomer { get; set; }
        public int? StatusId { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}
