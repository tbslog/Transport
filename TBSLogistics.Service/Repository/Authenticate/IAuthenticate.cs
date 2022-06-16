using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.LoginModel;

namespace TBSLogistics.Service.Repository.Authenticate
{
    public interface IAuthenticate
    {
        Task<BoolActionResult> checkUser(LoginRequest request);
    }
}
