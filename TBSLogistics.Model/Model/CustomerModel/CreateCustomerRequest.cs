using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.AddressModel;

namespace TBSLogistics.Model.Model.CustommerModel
{
   public class CreateCustomerRequest
    {
        public string TenKh { get; set; }
        public string MaSoThue { get; set; }
        public string LoaiKH { get; set; }
        public string NhomKH { get; set; }
        public string Sdt { get; set; }
        public string Email { get; set; }
        public int TrangThai { get; set; }
        public CreateAddressRequest Address { get; set; }
    }
}
