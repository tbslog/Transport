using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSLogistics.Model.Model.FileModel
{
    public class DocumentType
    {
        public long MaChungTu { get; set; }
        public long MaDieuPhoi { get; set; }
        public long MaHinhAnh { get; set; }
        public string contNo { get; set; }
        public string sealNp { get; set; }
        public IFormFile fileImage { get; set; }
        public string codeBase64 { get; set; }
        public string TenChungTu { get; set; }
        public int LoaiChungTu { get; set; }
        public string TenLoaiChungTu { get; set; }
        public string GhiChu { get; set; }
        public int TrangThai { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Creator { get; set; }
        public string Updater { get; set; }
    }
}
