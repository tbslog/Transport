﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Model.TempModel;

namespace TBSLogistics.Service.Services.Common
{
    public interface ICommon
    {
        Task Log(string FileName, string LogMessage);
        string GetFileUrl(string fileName, string fileFolder);
        string GetFile(string fileFolder);
        Task SaveFileAsync(Stream mediaBinaryStream, string fileName, string fileFolder);
        Task DeleteFileAsync(string fileName, string fileFolder);
        Task<BoolActionResult> AddAttachment(Attachment attachment);
        Task<Attachment> GetAttachmentById(int id);
        Task<BoolActionResult> CheckPermission(string permissionId);
        TempData DecodeToken(string token);
        Task SendMail(MailContent mailContent);
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task LogTimeUsedOfUser(string token);
	}
}
