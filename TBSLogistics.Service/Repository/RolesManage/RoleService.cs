using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TBSLogisticsDbContext;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.PermissionModel;
using TBSLogistics.Model.Model.RoleModel;

namespace TBSLogistics.Service.Repository.RolesManage
{
    public class RoleService : IRole
    {
        private readonly TBSTuyenDungContext _context;

        public RoleService(TBSTuyenDungContext context)
        {
            _context = context;
        }

        public async Task<BoolActionResult> AssignPermissionForRole(List<int> Permissions, int RoleId)
        {
            try
            {
                var GetListPermissions = await _context.RoleHasPermissions.Where(x => x.RoleId == RoleId).ToListAsync();
                var GetListNewChecked = Permissions.Where(x => !GetListPermissions.Select(x => x.PermissionId).Contains(x)).ToList();
                var GetListUnChecked = GetListPermissions.Where(x => !Permissions.Contains(x.PermissionId)).Select(x => x.PermissionId).ToList();


                if (GetListNewChecked.Any())
                {
                    foreach (var item in GetListNewChecked)
                    {
                        await _context.RoleHasPermissions.AddAsync(new RoleHasPermission()
                        {
                            PermissionId = item,
                            RoleId = RoleId,
                            CreatedTime = DateTime.Now,
                        });
                    }
                }

                if (GetListUnChecked.Any())
                {
                    _context.RoleHasPermissions.RemoveRange(await _context.RoleHasPermissions.Where(x => GetListUnChecked.Contains(x.PermissionId)).ToListAsync());
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Assign permissions for role success" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Assign permissions for role Failed" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> CreateNewRole(RoleRequest request)
        {
            try
            {
                var checkExistsRole = await _context.Roles.Where(x => x.RoleName == request.Name).FirstOrDefaultAsync();

                if (checkExistsRole != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Role name is Exists" };
                }

                var InsertRole = await _context.Roles.AddAsync(new Role()
                {
                    RoleName = request.Name,
                    Status = request.Status,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                var reuslt = await _context.SaveChangesAsync();
                if (reuslt > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "OK" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Errors" };
                }
            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<BoolActionResult> DeleteRole(int id)
        {
            try
            {
                var FindRole = await _context.Roles.FindAsync(id);
                FindRole.Status = 0;

            }
            catch (Exception ex)
            {
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<List<TreePermissionRequest>> GetListPermissionsRole(int roleId)
        {
            List<TreePermissionRequest> list = new List<TreePermissionRequest>();

            var getChecked = await _context.RoleHasPermissions.Where(x => x.RoleId == roleId).ToListAsync();
            var getChildPermission = await _context.Permissions.ToListAsync();

            foreach (var Catetegory in getChildPermission.Where(x => x.PearentId == null))
            {
                list.Add(new TreePermissionRequest
                {
                    id = Catetegory.MId.ToString(),
                    text = Catetegory.Name,
                    state = getChecked.Select(u => u.PermissionId).Contains(Catetegory.MId) ? new { selected = "true", disabled = "true" } : new { disabled = "true" },
                    children = getChildPermission.Where(x => x.PearentId == Catetegory.MId).Select(x => new TreePermissionRequest()
                    {
                        id = x.MId.ToString(),
                        text = x.Name,
                        state = getChecked.Select(u => u.PermissionId).Contains(x.MId) ? new { selected = "true", disabled = "true" } : new { disabled = "true" },
                        children = getChildPermission.Where(y => y.PearentId == x.MId).Select(y => new TreePermissionRequest()
                        {
                            id = y.MId.ToString(),
                            text = y.Name,
                            state = getChecked.Select(u => u.PermissionId).Contains(y.MId) ? new { selected = "true", disabled = "true" } : new { disabled = "true" },
                            children = getChildPermission.Where(z => z.PearentId == y.MId).Select(z => new TreePermissionRequest()
                            {
                                id = z.MId.ToString(),
                                text = z.Name,
                                state = getChecked.Select(u => u.PermissionId).Contains(z.MId) ? new { selected = "true", disabled = "true" } : new { disabled = "true" },
                            }).ToList()
                        }).ToList()
                    }).ToList()
                });
            }
            return list;
        }

        public Task<List<RoleRequest>> GetListRoles()
        {
            throw new NotImplementedException();
        }

        public async Task<BoolActionResult> UpdateRole(int id, RoleRequest request)
        {
            try
            {
                var FindRole = await _context.Roles.FindAsync(id);

                FindRole.RoleName = request.Name;
                FindRole.Status = request.Status;
                FindRole.UpdatedTime = DateTime.Now;

                _context.Update(FindRole);

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Update role success" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Update role Failed" };
                }

            }
            catch (Exception ex)
            {
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }
    }
}
