using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TBSLogistics.Service.Services.NotificationManage;

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotification _notification;
        
        public NotificationController (INotification notification)
        {
            _notification = notification;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetNotificationByLangFunc(string LangId, string FuncId)
        {
            var listNotification = await _notification.GetNotificationByLangFunc(LangId, FuncId);
            return Ok(listNotification);
        }
    }
}
