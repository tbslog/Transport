using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TBSLogistics.Model.Filter;
using TBSLogistics.Model.Model.ProductServiceModel;
using TBSLogistics.Service.Helpers;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.ProductServiceManage;

[Route("api/[controller]")]
[ApiController]
public class ProductServiceController : ControllerBase
{

    private IProduct _product;
    private readonly IPaginationService _pagination;

    public ProductServiceController(IProduct product, IPaginationService pagination)
    {
        _product = product;
        _pagination = pagination;
    }

    [HttpPost]
    [Route("[action]")]
    public async Task<IActionResult> CreateProductService( List < CreateProductServiceRequest> request)
    {
        var Create = await _product.CreateProductService(request);

        if (Create.isSuccess == true)
        {
            return Ok(Create.Message);
        }
        else
        {
            return BadRequest(Create.Message);
        }
    }


    //[HttpPut]
    //[Route("[action]")]
    //public async Task<IActionResult> UpdateProductService(string id, [FromForm] EditProductServiceRequest request)
    //{
    //    var editProductService = await _product.EditProductService(id, request);
    //    if (editProductService.isSuccess == true)
    //    {
    //        return Ok(editProductService.Message);
    //    }
    //    else
    //    {
    //        return BadRequest(editProductService.Message);
    //    }
    //}
    //[HttpGet]
    //[Route("[action]")]
    //public async Task<IActionResult> GetProductServiceByIdRequest(string id)
    //{
    //    var getProductServiceByIdRequest = await _product.GetProductServiceByIdRequest(id);
    //    return Ok(getProductServiceByIdRequest);

    //}
    //[HttpGet]
    //[Route("[action]")]
    //public async Task<IActionResult> GetListProductService([FromQuery] PaginationFilter filter)
    //{
    //    var route = Request.Path.Value;
    //    var pagedData = await _product.GetListProductService(filter);
    //    var pagedReponse = PaginationHelper.CreatePagedReponse<ListProductServiceRequest>(pagedData.dataResponse, pagedData.paginationFilter, pagedData.totalCount, _pagination, route);
    //    return Ok(pagedReponse);
    //}
}
