using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.AddressModel;

namespace TBSLogistics.Model.Model.CustommerModel
{
   public class EditCustomerRequest
    {
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public UpdateAddressRequest Address { get; set; }
    }
}
