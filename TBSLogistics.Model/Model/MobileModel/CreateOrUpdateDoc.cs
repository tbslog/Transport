using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.MobileModel
{
    public class CreateOrUpdateDoc
    {
        public long handlingId { get; set; }
        public string contNo { get; set; }
        public string sealNp { get; set; }
        public int docType { get; set; }
        public string note { get; set; }
        public IFormFile fileImage { get; set; }
    }
}
