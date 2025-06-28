using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrdersManagementService _service;
    public OrdersController(IOrdersManagementService service)
    {
        _service = service;
    }
    [HttpGet()]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _service.GetOrder();
        return orders?.Any() == true ? Ok(orders) : NoContent();
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _service.GetOrderById(id);
        if (order == null) return NotFound();
        return Ok(order);
    }

    [HttpPost()]
    public async Task<IActionResult> AddOrder([FromBody] OrderModel.Request request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var product = await _service.AddOrder(request);
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

}
