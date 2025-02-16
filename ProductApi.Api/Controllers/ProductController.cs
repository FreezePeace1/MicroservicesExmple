using ECommerceSharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Dtos;
using ProductApi.Domain.Dtos.Conversions;

namespace ProductApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var products = await _productService.GetAllAsync();

        if (!products.Any())
        {
            return NotFound("No products are found in the DB");
        }

        var (_, list) = ProductConversions.FromEntity(null!, products);

        return list.Any() ? Ok(list) : NotFound("No product is found");
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductDto>> GetProductById(int id)
    {
        var product = await _productService.FindByIdAsync(id);

        if (product is null)
        {
            return NotFound("product is not found");
        }

        var (_product, _) = ProductConversions.FromEntity(product, null);

        return _product is not null ? Ok(_product) : NotFound("Product is not found");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> CreateProduct(ProductDto product)
    {
        // если не заполнены требуемые атрибуты пользователем
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = ProductConversions.ToEntity(product);
        var response = await _productService.CreateAsync(entity);
        
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> UpdateProduct(ProductDto product)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var entity = ProductConversions.ToEntity(product);
        var response = await _productService.UpdateAsync(entity);
        
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Response>> DeleteProduct(ProductDto product)
    {
        var entity = ProductConversions.ToEntity(product);
        var response = await _productService.DeleteAsync(entity);
        
        return response.Flag is true ? Ok(response) : BadRequest(response);
    }
}