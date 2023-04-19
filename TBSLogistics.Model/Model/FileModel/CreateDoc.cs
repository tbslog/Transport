using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;

namespace TBSLogistics.Model.Model.FileModel
{
    public class CreateDoc
    {
        public long handlingId { get; set; }
        public int docType { get; set; }
        public IFormFile fileImage { get; set; }
        public string note { get; set; }
        public string contNo { get; set; }
        public string sealNp { get; set; }
    }

    public class UpdateDoc
    {
        public int docType { get; set; }
        public IFormFile fileImage { get; set; }
        public string note { get; set; }
    }
}
