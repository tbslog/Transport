using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Data.TBSLogisticsDbContext;
using TBSLogistics.Model.CommonModel;
using TBSLogistics.Model.Model.LoginModel;
using TBSLogistics.Model.TempModel;

namespace TBSLogistics.Service.Repository.Authenticate
{
    public class AuthenticateService : IAuthenticate
    {
        private readonly TBSTuyenDungContext _context;

        public AuthenticateService(TBSTuyenDungContext context)
        {
            _context = context;
        }

        public async Task<BoolActionResult> checkUser(LoginRequest request)
        {
            var checkUser = await _context.Users.Where(x => x.UserName == request.Username && x.PassWord == GetMD5(request.Password)).FirstOrDefaultAsync();

            if (checkUser == null)
            {
                return new BoolActionResult { isSuccess = false, Message = "Tài khoản không tồn tại" };
            }
            else
            {
                TempData.UserID = checkUser.Id;

                var getPermission = await _context.UserHasPermissions.Where(x => x.UserId == checkUser.Id).Select(x => x.PermissionId).ToListAsync();
                var getUserRolePermission = await _context.RoleHasPermissions.Where(x => x.RoleId == _context.UserHasRoles
                .Where(y => y.UserId == checkUser.Id).Select(y => y.RoleId).FirstOrDefault()).Select(x => x.PermissionId).ToListAsync();

                string permission = string.Join(':', getPermission);
                permission = string.Join(':', getUserRolePermission);

                return new BoolActionResult { isSuccess = true, Message = "Đăng nhập thành công!", DataReturn = permission };
            }
        }

        public async Task<bool> Logout()
        {
            var getToken = await _context.Tokens.Where(x => x.UserId == TempData.UserID).FirstOrDefaultAsync();

            _context.Tokens.Remove(getToken);
            await _context.SaveChangesAsync();

            return true;
           
        }

        public async Task<BoolActionResult> SaveToken(int UserId, string token, DateTime dateTime)
        {
            var checkTokenUser = await _context.Tokens.Where(x => x.UserId == UserId).FirstOrDefaultAsync();

            if (checkTokenUser == null)
            {
                await _context.AddAsync(new Token()
                {
                    UserId = UserId,
                    TokenCode = token,
                    TimeOut = dateTime
                });
            }
            else
            {
                _context.Tokens.Remove(checkTokenUser);

                await _context.AddAsync(new Token()
                {
                    UserId = UserId,
                    TokenCode = token,
                    TimeOut = dateTime
                });
            }

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new BoolActionResult { isSuccess = true, Message = "OK" };
            }
            else
            {
                return new BoolActionResult { isSuccess = false, Message = "Error" };
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
    }
}
