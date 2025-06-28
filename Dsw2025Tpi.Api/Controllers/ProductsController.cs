using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace Dsw2025Tpi.Api.Controllers;


[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductsManagementServices _service;

    public ProductsController(ProductsManagementServices service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _service.GetProduct();

        return products?.Any() == true ? Ok(products) : NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.RequestProduct request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var product = await _service.AddProduct(request);
            return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("There was a problem saving the product");
        }
    }

    [HttpPut]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProductAsync(Guid id, [FromBody] ProductModel.RequestProduct request)
    {
        if (request == null)
        {
            return BadRequest("Product data is invalid.");
        }
        var updatedProduct = await _service.UpdateProduct(id, request);
        if (updatedProduct == null)
        {
            return NotFound();
        }
        return Ok(updatedProduct);
    }

    [HttpPatch()]
    [Route("{id:guid}")]
    public async Task<IActionResult> PatchProduct(Guid id)
    {
        try
        {
            var product = await _service.DeactivateProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (ApplicationException de)
        {
            return Conflict(de.Message);
        }
        catch (Exception)
        {
            return Problem("There was a problem updating the product");
        }
    }
}

