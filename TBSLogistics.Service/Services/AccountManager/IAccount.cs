using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.AccountModel;

namespace TBSLogistics.Service.Services.AccountManager
{
    public interface IAccount
    {
        Task<BoolActionResult> CreateAccount(CreateOrUpdateAccount request);
        Task<GetAccountById> GetAccountById(string accountId);
        Task<BoolActionResult> UpdateAccount(string accountId, CreateOrUpdateAccount request);
        Task<List<GetAccountById>> GetListAccountSelectByCus(string cusId);
    }
}
