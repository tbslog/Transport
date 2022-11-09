using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.UserManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IPaginationService _uriService;
        private readonly IConfiguration _config;

        public UserController(IUser user, IPaginationService uriService, IConfiguration config)
        {
            _config = config;
            _user = user;
            _uriService = uriService;
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateUser(CreateUserRequest request)
        {
            try
            {
                var create = await _user.CreateUser(request);

                if (create.isSuccess)
                {
                    return Ok(create.Message);
                }
                else
                {
                    return BadRequest(create.Message);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            var update = await _user.UpdateUser(id, request);

            if (update.isSuccess)
            {
                return Ok(update.Message);
            }
            else
            {
                return BadRequest(update.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _user.GetUser(id);
            return Ok(user);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTreePermission(int id)
        {
            var tree = await _user.GetTreePermission(id);
            return Ok(tree);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListUser([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _user.GetListUser(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<GetUserRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> SetPermissionForRole(SetRole request)
        {
            var add = await _user.SetPermissionForRole(request);

            if (add.isSuccess)
            {
                return Ok(add.Message);
            }
            else
            {
                return BadRequest(add.Message);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListRoleSelect()
        {
            var list = await _user.GetListRoleSelect();
            return Ok(list);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListDepartmentSelect()
        {
            var list = await _user.GetListDepartmentSelect();
            return Ok(list);
        }

        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            var user = await _user.GetUserByName(username);
            return Ok(user);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel _userData)
        {
            if (_userData != null && _userData.UserName != null && _userData.Password != null)
            {
                var user = await _user.CheckLogin(_userData);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("UserName", user.UserName),
                        new Claim("FullName", user.HoVaTen),
                        new Claim("Department", user.MaBoPhan),
                        new Claim("Role", user.RoleId.ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _config["Jwt:Issuer"],
                        _config["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(60),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Tài khoản hoặc mật khẩu không đúng");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ChangePassword(string username, ChangePasswordModel model)
        {
            var changePass = await _user.ChangePassword(username, model);

            if (changePass.isSuccess)
            {
                return Ok(changePass.Message);
            }

            return BadRequest(changePass.Message);
        }
    }
}