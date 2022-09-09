using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;

namespace TBSLogistics.Service.Repository.Common
{
    public interface ICommon
    {
        Task Log(string FileName, string LogMessage);

        string GetFileUrl(string fileName, string fileFolder);

        Task SaveFileAsync(Stream mediaBinaryStream, string fileName, string fileFolder);

        Task DeleteFileAsync(string fileName, string fileFolder);

        Task<BoolActionResult> AddAttachment(Attachment attachment);
        Task<Attachment> GetAttachmentById(int id);
    }
}
