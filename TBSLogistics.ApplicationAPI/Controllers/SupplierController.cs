using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TBSLogistics.Model.Model.SupplierModel;
using TBSLogistics.Service.Repository.SupplierManage;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TBSLogistics.ApplicationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplier _supplier;

        public SupplierController(ISupplier supplier)
        {
            _supplier = supplier;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> CreateSupplier(CreateSupplierRequest request)
        {
            var create = await _supplier.CreateSupplier(request);

            if (create.isSuccess == true)
            {
                return Ok(create.Message);
            }
            else
            {
                return BadRequest(create.Message);
            }
        }

        [HttpPut]
        [Route("[action]")]
        public async Task<IActionResult> EditSupplier(string SupplierId, UpdateSupplierRequest request)
        {
            var Update = await _supplier.EditSupplier(SupplierId, request);

            if (Update.isSuccess == true)
            {
                return Ok(Update.Message);
            }
            else
            {
                return BadRequest(Update.Message);
            }
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetSupplierById(string SupplierId)
        {
            var supplier = await _supplier.GetSupplierById(SupplierId);
            return Ok(supplier);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> GetListSupplier()
        {
            var list = await _supplier.GetListSupplier();
            return Ok(list);
        }
    }
}