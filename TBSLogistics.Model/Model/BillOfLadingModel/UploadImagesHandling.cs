using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.BillOfLadingModel
{
    public class UploadImagesHandling
    {
        public IFormFile file { get; set; }
        public string transportId { get; set; }
        public long handlingId { get; set; }
    }
}
