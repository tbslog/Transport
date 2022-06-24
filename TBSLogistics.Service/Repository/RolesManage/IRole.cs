using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Data.TBSLogisticsDbContext;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PermissionModel;
using TBSLogistics.Model.Model.RoleModel;

namespace TBSLogistics.Service.Repository.RolesManage
{
    public interface IRole
    {
        Task<List<RoleRequest>> GetListRoles();

        Task<List<TreePermissionRequest>> GetListPermissionsRole(int roleId);

        Task<BoolActionResult> CreateNewRole(RoleRequest request);

        Task<BoolActionResult> UpdateRole(int id, RoleRequest request);

        Task<BoolActionResult> DeleteRole(int id);

        Task<BoolActionResult> AssignPermissionForRole(List<int> Permissions, int RoleId);
    }
}