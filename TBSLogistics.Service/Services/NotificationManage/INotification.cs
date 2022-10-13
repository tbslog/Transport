using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.NotificationModel;

namespace TBSLogistics.Service.Services.NotificationManage
{
    public interface INotification
    {
        Task<List<GetNotificationByLangFunc>> GetNotificationByLangFunc(string LangId, string FuncId);
    }
}
