using System.Net;
using System.Security.Claims;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/products")]
public class ProductController : Controller
{
    private readonly IProductsManagementService _service;

    public ProductController(IProductsManagementService service)
    {
        _service = service;
    }

    [HttpGet()]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts()
    {

        var products = await _service.GetProducts();
        if (products == null || !products.Any())
        {
            Response.Headers.Append("X-Message", "There are no active products");
            return NoContent();
        }
        return Ok(products);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "ADMIN,USER")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var product = await _service.GetProductById(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost()]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> AddProduct([FromBody] ProductModel.RequestProduct request)
    {
        if (request == null)
            return BadRequest("Product data is required");
        try
        {
            var product = await _service.AddProduct(request);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);

        }
        catch (DuplicatedEntityException de) 
        {
            return BadRequest(de.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Application.Exceptions.ApplicationException ape)
        {
            return Conflict(ape.Message);
        }
        catch (Exception)
        {
            return Problem("There was a problem saving the product");
        }
    }

    [HttpPut]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> UpdateProductAsync(Guid id, [FromBody] ProductModel.RequestProduct request)
    {
        if (request == null)
            return BadRequest("Product data is required");
        try
        {
            var updatedProduct = await _service.UpdateProduct(id, request);
            return Ok(updatedProduct);
        }
        catch (EntityNotFoundException enf)
        {
            return NotFound(enf.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Application.Exceptions.ApplicationException de)
        {
            return BadRequest(de.Message);
        }
        catch (Exception)
        {
            return Problem("There was a problem updating the product");
        }
    }

    [HttpPatch()]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> DeactivateProductAsync(Guid id)
    {
        try
        {
            var deactivatedProduct = await _service.DeactivateProduct(id);
            if (deactivatedProduct == null)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch(EntityNotFoundException en)
        {
            return NotFound(en.Message);
        }
        catch (ArgumentException ae)
        {
            return BadRequest(ae.Message);
        }
        catch (Application.Exceptions.ApplicationException de)
        {
            return BadRequest(de.Message);
        }
        catch (Exception)
        {
            return Problem("There was a problem deactivating the product");
        }
    }
}

