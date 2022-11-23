using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ContractModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.Wrappers;

namespace TBSLogistics.Service.Repository.UserManage
{
    public interface IUser
    {
        Task<BoolActionResult> CreateUser(CreateUserRequest request);
        Task<BoolActionResult> UpdateUser(int id,UpdateUserRequest request);
        Task<GetUserRequest> GetUser(int id);
        Task<PagedResponseCustom<GetUserRequest>> GetListUser(PaginationFilter filter);
        Task<BoolActionResult> SetPermissionForRole(SetRole request);
        Task<TreePermission> GetTreePermission(int roleId);
        Task<List<Role>> GetListRoleSelect();
        Task<List<BoPhan>> GetListDepartmentSelect();
        Task<GetUserRequest> CheckLogin(LoginModel model);
        Task<GetUserRequest> GetUserByName(string username);
        Task<BoolActionResult> ChangePassword(string username, ChangePasswordModel model);
        Task<BoolActionResult> BlockUsers(List<int> userIds);

    }
}
