using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Model.Wrappers;
using TBSLogistics.Service.Services.Common;

namespace TBSLogistics.Service.Services.UserManage
{
    public class UserService : IUser
    {
        private readonly TMSContext _context;
        private readonly ICommon _common;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TempData tempData;

        public UserService(TMSContext context, ICommon common, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _common = common;
            _context = context;
            tempData = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToArray().Length > 0 ? _common.DecodeToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"][0].ToString().Replace("Bearer ", "")) : new TempData();
        }

        public async Task<BoolActionResult> SetPermissionForRole(SetRole request)
        {
            try
            {
                var checkExists = await _context.Role.Where(x => x.RoleName == request.RoleName).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    await _context.Role.AddAsync(new Role()
                    {
                        RoleName = request.RoleName,
                        Status = request.RoleStatus,
                        CreatedDate = DateTime.Now,
                        UpdatedDate = DateTime.Now
                    });

                    var rs = await _context.SaveChangesAsync();
                    if (rs < 1)
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Không thể InsertDb, vui lòng thử lại sau" };
                    }
                }

                checkExists = await _context.Role.Where(x => x.RoleName == request.RoleName).FirstOrDefaultAsync();

                if (request.RoleStatus != checkExists.Status)
                {
                    checkExists.Status = request.RoleStatus;
                }

                var checkPermission = await _context.Permission.Where(x => request.Permission.Contains(x.Mid)).ToListAsync();

                if (request.Permission.Count != checkPermission.Count)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Permission không tồn tại" };
                }

                var listPermissionOfRole = await _context.RoleHasPermission.Where(x => x.RoleId == checkExists.Id).Select(x => x.PermissionId).ToListAsync();

                if (listPermissionOfRole.Count == 0)
                {
                    var list = request.Permission.Select(x => new RoleHasPermission()
                    {
                        RoleId = checkExists.Id,
                        PermissionId = checkPermission.Where(y => y.Mid == x).Select(y => y.Id).FirstOrDefault(),
                        CreatedDate = DateTime.Now
                    }).ToList();
                    await _context.RoleHasPermission.AddRangeAsync(list);
                }
                else
                {
                    var listAddNewPermission = request.Permission.Except(checkPermission.Where(x => listPermissionOfRole.Contains(x.Id)).Select(x => x.Mid)).ToList();
                    if (listAddNewPermission.Count > 0)
                    {
                        var list = listAddNewPermission.Select(x => new RoleHasPermission()
                        {
                            RoleId = checkExists.Id,
                            PermissionId = checkPermission.Where(y => y.Mid == x).Select(y => y.Id).FirstOrDefault(),
                            CreatedDate = DateTime.Now
                        }).ToList();
                        await _context.RoleHasPermission.AddRangeAsync(list);
                    }

                    var CheckRemovePermission = await _context.Permission.Where(x => listPermissionOfRole.Contains(x.Id)).Select(x => x.Mid).ToListAsync();

                    var GetlistRemove = CheckRemovePermission.Except(request.Permission).ToList();

                    if (GetlistRemove.Count > 0)
                    {
                        var listRemove = await _context.Permission.Where(x => GetlistRemove.Contains(x.Mid)).Select(x => x.Id).ToListAsync();
                        _context.RoleHasPermission.RemoveRange(_context.RoleHasPermission.Where(x => listRemove.Contains(x.PermissionId) && x.RoleId == checkExists.Id));
                    }
                }

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("AddPermissionForRole", "UserId: " + tempData.UserID + " Set Permission For Role with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Add permissions for role success" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Add permissions for role failed" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("AddPermissionForRole", ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Add permissions for role failed, Contact to IT" };
            }
        }

        public async Task<BoolActionResult> BlockUsers(List<int> userIds)
        {
            var checkUser = await _context.NguoiDung.Where(x => userIds.Contains(x.Id)).ToListAsync();

            foreach (var user in checkUser)
            {
                user.TrangThai = user.TrangThai == 1 ? 2 : 1;
            }

            _context.UpdateRange(checkUser);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                await _common.Log("User", "UserId: " + tempData.UserID + " Block Users: " + JsonSerializer.Serialize(userIds));
                return new BoolActionResult { isSuccess = true, Message = "Ok" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Errors" };
            }
        }

        public async Task<BoolActionResult> CreateUser(CreateUserRequest request)
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var checkExists = await _context.NguoiDung.Where(x => x.MaNhanVien == request.MaNhanVien).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Nhân viên này đã có Tài Khoản" };
                }

                var checkUserName = await _context.Account.Where(x => x.UserName == request.UserName).FirstOrDefaultAsync();

                if (checkExists != null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tên tài khoản đã tồn tại" };
                }

                if (Regex.IsMatch(request.UserName, "^[a-z0-9.]+$") == false)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Vui lòng không ký tự đặc biệt, không viết dấu ,không khoảng trống cho tên đăng nhập" };
                }

                var addacc = await _context.Account.AddAsync(new Account()
                {
                    UserName = request.UserName,
                    PassWord = GetMD5(request.PassWord)
                });

                await _context.SaveChangesAsync();

                await _context.NguoiDung.AddAsync(new NguoiDung()
                {
                    Id = addacc.Entity.Id,
                    HoVaTen = request.HoVaTen,
                    MaBoPhan = request.MaBoPhan,
                    MaNhanVien = request.MaNhanVien,
                    RoleId = request.RoleId,
                    TrangThai = request.TrangThai,
                    NguoiTao = tempData.UserName,
                    CreatedTime = DateTime.Now,
                    UpdatedTime = DateTime.Now
                });

                await _context.SaveChangesAsync();

                await _context.UserHasRole.AddAsync(new UserHasRole()
                {
                    UserId = addacc.Entity.Id,
                    RoleId = request.RoleId,
                    CreatedDate = DateTime.Now
                });

                var result = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    await _common.Log("User", "UserId: " + tempData.UserID + " Create User with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Thêm mới người dùng thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Thêm mới người dùng không thành công" };
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await _common.Log("Errors User", ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = "Thêm mới người dùng không thành công, Liên Hệ IT \r\n" + ex.ToString() };
            }
        }

        public async Task<PagedResponseCustom<GetUserRequest>> GetListUser(PaginationFilter filter)
        {
            try
            {
                var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
                var getList = from user in _context.NguoiDung
                              join acc in _context.Account
                              on user.Id equals acc.Id
                              join bp in _context.BoPhan
                              on user.MaBoPhan equals bp.MaBoPhan
                              join status in _context.StatusText
                              on user.TrangThai equals status.StatusId
                              where status.LangId == tempData.LangID
                              orderby user.CreatedTime descending
                              select new { user, acc, bp, status };

                if (!string.IsNullOrEmpty(filter.Keyword))
                {
                    getList = getList.Where(x => x.acc.UserName.Contains(filter.Keyword) || x.user.HoVaTen.Contains(filter.Keyword) || x.user.MaNhanVien.Contains(filter.Keyword));
                }

                if (!string.IsNullOrEmpty(filter.statusId))
                {
                    getList = getList.Where(x => x.user.TrangThai == int.Parse(filter.statusId));
                }

                if (!string.IsNullOrEmpty(filter.fromDate.ToString()) && !string.IsNullOrEmpty(filter.toDate.ToString()))
                {
                    getList = getList.Where(x => x.user.CreatedTime.Date >= filter.fromDate.Value.Date && x.user.CreatedTime <= filter.toDate.Value.Date);
                }

                var totalRecords = await getList.CountAsync();

                var pagedData = await getList.Skip((validFilter.PageNumber - 1) * validFilter.PageSize).Take(validFilter.PageSize).Select(x => new GetUserRequest()
                {
                    Id = x.user.Id,
                    UserName = x.acc.UserName,
                    HoVaTen = x.user.HoVaTen,
                    MaNhanVien = x.user.MaNhanVien,
                    MaBoPhan = x.bp.TenBoPhan,
                    RoleId = _context.Role.Where(y => y.Id == x.user.RoleId).Select(x => x.RoleName).FirstOrDefault(),
                    TrangThai = x.status.StatusContent,
                    NguoiTao = x.user.NguoiTao,
                    CreatedTime = x.user.CreatedTime,
                    UpdatedTime = x.user.UpdatedTime
                }).ToListAsync();

                return new PagedResponseCustom<GetUserRequest>()
                {
                    paginationFilter = validFilter,
                    totalCount = totalRecords,
                    dataResponse = pagedData
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<TreePermission> GetTreePermission(int roleId)
        {
            try
            {
                var listPermission = await _context.Permission.ToListAsync();
                var listTree = listPermission.Where(x => x.ParentId == null).Select(o => new ListTree()
                {
                    Label = o.PermissionName,
                    Value = o.Mid,
                    Children = listPermission.Where(x => x.ParentId == o.Mid).Select(x => new ListTree()
                    {
                        Label = x.PermissionName,
                        Value = x.Mid,
                        Children = listPermission.Where(y => y.ParentId == x.Mid).Select(y => new ListTree()
                        {
                            Label = y.PermissionName,
                            Value = y.Mid,
                        }).ToList(),
                    }).ToList(),
                }).ToList();

                var checkExists = await _context.Role.Where(x => x.Id == roleId).FirstOrDefaultAsync();
                if (checkExists == null)
                {
                    return new TreePermission()
                    {
                        ListTree = listTree
                    };
                }

                var getPermissionByRole = await _context.RoleHasPermission.Where(x => x.RoleId == roleId).ToListAsync();
                var listPermissionOfRole = listPermission.Where(x => getPermissionByRole.Select(y => y.PermissionId).Contains(x.Id)).Select(y => y.Mid).ToList();

                return new TreePermission()
                {
                    RoleName = checkExists.RoleName,
                    Status = checkExists.Status,
                    IsChecked = listPermissionOfRole,
                    ListTree = listTree
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<GetUserRequest> GetUser(int id)
        {
            var GetById = await _context.NguoiDung.Where(x => x.Id == id).FirstOrDefaultAsync();
            var Account = await _context.Account.Where(x => x.Id == id).FirstOrDefaultAsync();

            return new GetUserRequest()
            {
                Id = id,
                UserName = Account.UserName,
                HoVaTen = GetById.HoVaTen,
                MaNhanVien = GetById.MaNhanVien,
                MaBoPhan = GetById.MaBoPhan,
                RoleId = GetById.RoleId.ToString(),
                TrangThai = GetById.TrangThai.ToString(),
            };
        }

        public async Task<BoolActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            try
            {
                var checkExists = await _context.NguoiDung.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (checkExists == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài khoản không tồn tại" };
                }

                checkExists.HoVaTen = request.HoVaTen;
                checkExists.MaBoPhan = request.MaBoPhan;
                checkExists.MaNhanVien = request.MaNhanVien;
                checkExists.TrangThai = request.TrangThai;

                if (checkExists.RoleId != request.RoleId)
                {
                    var getRole = await _context.UserHasRole.Where(x => x.UserId == id).FirstOrDefaultAsync();
                    if (getRole == null)
                    {
                        await _context.UserHasRole.AddAsync(new UserHasRole()
                        {
                            UserId = id,
                            RoleId = request.RoleId,
                            CreatedDate = DateTime.Now
                        });
                    }
                    checkExists.RoleId = request.RoleId;
                }

                _context.Update(checkExists);

                if (!string.IsNullOrEmpty(request.Password))
                {
                    var account = await _context.Account.Where(x => x.Id == id).FirstOrDefaultAsync();
                    account.PassWord = GetMD5(request.Password);
                    _context.Update(account);
                }

                //if (!string.IsNullOrEmpty(request.NewPassWord))
                //{
                //    var account = await _context.Account.Where(x => x.Id == id).FirstOrDefaultAsync();

                //    if (account.PassWord == GetMD5(request.OldPassWord))
                //    {
                //        account.PassWord = GetMD5(request.NewPassWord);
                //        _context.Update(account);
                //    }
                //    else
                //    {
                //        return new BoolActionResult { isSuccess = false, Message = "Mật khẩu cũ không đúng" };
                //    }
                //}

                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    await _common.Log("User", "UserId: " + tempData.UserID + " Update User with data: " + JsonSerializer.Serialize(request));
                    return new BoolActionResult { isSuccess = true, Message = "Cập nhật thông tin thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Cập nhật thông tin thất bại" };
                }
            }
            catch (Exception ex)
            {
                await _common.Log("Errors User", ex.ToString());
                return new BoolActionResult { isSuccess = false, Message = ex.ToString() };
            }
        }

        public async Task<List<Role>> GetListRoleSelect()
        {
            var list = await _context.Role.ToListAsync();
            return list;
        }

        public async Task<List<BoPhan>> GetListDepartmentSelect()
        {
            var list = await _context.BoPhan.ToListAsync();
            return list;
        }

        public async Task<GetUserRequest> GetUserByName(string username)
        {
            var Account = await _context.Account.Where(x => x.UserName == username).FirstOrDefaultAsync();
            var GetById = await _context.NguoiDung.Where(x => x.Id == Account.Id).FirstOrDefaultAsync();

            return new GetUserRequest()
            {
                Id = GetById.Id,
                RoleName = _context.Role.Where(x => x.Id == GetById.RoleId).Select(x => x.RoleName).FirstOrDefault(),
                UserName = Account.UserName,
                HoVaTen = GetById.HoVaTen,
                MaNhanVien = GetById.MaNhanVien,
                MaBoPhan = GetById.MaBoPhan,
                TenBoPhan = _context.BoPhan.Where(x => x.MaBoPhan == GetById.MaBoPhan).Select(x => x.TenBoPhan).FirstOrDefault(),
                RoleId = GetById.RoleId.ToString(),
                TrangThai = GetById.TrangThai.ToString(),
            };
        }

        public async Task<BoolActionResult> ChangePassword(string username, ChangePasswordModel model)
        {
            var transaction = _context.Database.BeginTransaction();
            try
            {
                var getAccount = await _context.Account.Where(x => x.UserName == username).FirstOrDefaultAsync();

                if (getAccount == null)
                {
                    return new BoolActionResult { isSuccess = false, Message = "Tài khoản không tồn tại!" };
                }

                if (getAccount.PassWord == model.OldPassword.ToUpper())
                {
                    if (model.NewPassword == model.ReNewPassword)
                    {
                        getAccount.PassWord = model.NewPassword.ToUpper();
                        _context.Update(getAccount);
                    }
                    else
                    {
                        return new BoolActionResult { isSuccess = false, Message = "Mật khẩu mới và nhập lại mật khẩu không khớp" };
                    }
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Mật khẩu cũ không đúng!" };
                }

                var result = await _context.SaveChangesAsync();

                transaction.Commit();

                if (result > 0)
                {
                    return new BoolActionResult { isSuccess = true, Message = "Đổi Mật Khẩu thành công" };
                }
                else
                {
                    return new BoolActionResult { isSuccess = false, Message = "Đổi Mật Khẩu không thành công" };
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new BoolActionResult { isSuccess = false, Message = "Đổi Mật Khẩu không thành công" };
            }
        }

        private string GetMD5(string str)
        {
            string str_md5_out = string.Empty;
            using (MD5 md5 = MD5.Create())
            {
                byte[] bytes_md5_in = Encoding.UTF8.GetBytes(str);
                byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);

                str_md5_out = BitConverter.ToString(bytes_md5_out);
                str_md5_out = str_md5_out.Replace("-", "");
            }
            return str_md5_out;
        }

        public async Task<GetUserRequest> CheckLogin(LoginModel model)
        {
            try
            {
                var checkUser = await _context.Account.Where(x => x.UserName == model.UserName && x.PassWord == model.Password.ToUpper()).FirstOrDefaultAsync();

                if (checkUser == null)
                {
                    return null;
                }

                var getUser = await _context.NguoiDung.Where(x => x.Id == checkUser.Id).FirstOrDefaultAsync();

                if (getUser.TrangThai == 2)
                {
                    return null;
                }

                return new GetUserRequest
                {
                    UserName = checkUser.UserName,
                    Id = checkUser.Id,
                    HoVaTen = getUser.HoVaTen,
                    MaBoPhan = getUser.MaBoPhan,
                    RoleId = getUser.RoleId.ToString(),
                    TrangThai = getUser.TrangThai.ToString(),
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}