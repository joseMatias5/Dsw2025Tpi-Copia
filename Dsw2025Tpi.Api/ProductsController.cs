using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace Dsw2025Tpi.Api;


[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly Services _service;

    public ProductsController(Services service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _service.GetProduct();
        if (products == null || !products.Any()) return NoContent();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductBySku(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null) return NotFound();
        return Ok(product);
    }

    [HttpPost()]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.Request request)
    {
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
            return Problem("Se produjo un error al guardar el producto");
        }
    }
}

