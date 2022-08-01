using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.UserModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        // GET: api/<UserController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string html = "<div class='col-md-6'><form method='post' id='frmUser'>" +
                "<label>Name</label><input class='form-control' type='text' id ='Name' /> <br>" +
                "<label>Address</label><input class='form-control' type='text'  id ='Address' /> <br>" +
                "<label>Email</label><input class='form-control' type='text'  id ='Email' /> <br>" +
                "<label>Phone</label><input class='form-control' type='number'  id ='Phone' /> <br>" +
                "<button class='btn btn-primary' type='button' id='btnSubmit' >submit</button>" +
            "</form>";

            var Js = JsonConvert.SerializeObject(html);
            return new JsonResult(Js);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<IActionResult> Post(string value)
        {
            var json = JsonConvert.DeserializeObject<UserRequest>(value);

            json.Name = "haile";
            json.Address = "TBS";
            json.Email = "haile@gmail.com";

            return new JsonResult(json);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}