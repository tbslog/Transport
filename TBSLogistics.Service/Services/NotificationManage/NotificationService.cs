using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.NotificationModel;

namespace TBSLogistics.Service.Services.NotificationManage
{
    public class NotificationService : INotification
    {
        private readonly TMSContext _TMSContext;

        public NotificationService(TMSContext tMSContext)
        {
            _TMSContext = tMSContext;
        }

        public async Task<List<GetNotificationByLangFunc>> GetNotificationByLangFunc(string LangId, string FuncId)
        {
            try
            {
                var list = await _TMSContext.ThongBao.Where(x => x.LangId == LangId && x.FunctionId == FuncId).Select(x => new GetNotificationByLangFunc()
                {
                    TextId = x.TextId,
                    TextContent = x.TextContent,
                    TextType = x.TextType,
                }).ToListAsync();

                return list;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
