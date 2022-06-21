using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.LoginModel;
using TBSLogistics.Model.TempModel;
using TBSLogistics.Service.Repository.Authenticate;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly IAuthenticate _authenticate;

        public AuthenticateController(IConfiguration configuration, IAuthenticate authenticate)
        {
            _configuration = configuration;
            _authenticate = authenticate;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            {
                return BadRequest("Thông tin đăng nhập không đúng");
            }

            var checkLogin = await _authenticate.checkUser(login);

            if (checkLogin.isSuccess == true)
            {
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
                    new Claim("Id",login.Username),
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.Now.AddHours(3), signingCredentials: signIn);

                await _authenticate.SaveToken(TempData.UserID, new JwtSecurityTokenHandler().WriteToken(token), DateTime.Now.AddHours(3));

                return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            else
            {
                return BadRequest(checkLogin.Message);
            }
        }
    }
}
