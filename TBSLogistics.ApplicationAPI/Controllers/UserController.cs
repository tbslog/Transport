using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.RomoocModel;
using TBSLogistics.Model.Model.UserModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Repository.UserManage;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly IPaginationService _uriService;

        public UserController(IUser user, IPaginationService uriService)
        {
            _user = user;
            _uriService = uriService;
        }

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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _user.GetUser(id);
            return Ok(user);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetTreePermission(int id)
        {
            var tree = await _user.GetTreePermission(id);
            return Ok(tree);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListUser([FromQuery] PaginationFilter filter)
        {
            var route = Request.Path.Value;
            var pagedData = await _user.GetListUser(filter);

            var pagedReponse = PaginationHelper.CreatePagedReponse<GetUserRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _uriService, route);
            return Ok(pagedReponse);
        }

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

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListRoleSelect()
        {
            var list = await _user.GetListRoleSelect();
            return Ok(list);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListDepartmentSelect()
        {
            var list = await _user.GetListDepartmentSelect();
            return Ok(list);
        }
    }
}