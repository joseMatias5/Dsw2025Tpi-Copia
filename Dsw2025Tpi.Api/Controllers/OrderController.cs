using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : Controller
{
    private readonly IOrdersManagementService _service;

    public OrderController(IOrdersManagementService service)
    {
        _service = service;
    }

    [HttpGet()]

    public async Task<IActionResult> GetOrders()
    {

        var orders = await _service.GetOrders();
        if (orders == null || !orders.Any())
        {
            Response.Headers.Add("X-Message", "No se encontraron ordenes activas.");
            return NoContent();
        }
        return Ok(orders);
    }

    [HttpGet("{id}")]

    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _service.GetOrderById(id);
        if (order == null)
            return NotFound();
        return Ok(order);
    }

    [HttpPost()]

    public async Task<IActionResult> AddOrder([FromBody] OrderModel.RequestOrder request)
    {
        if (request == null)
            return BadRequest("Order data is required");
        try
        {
            var order = await _service.AddOrder(request);
            return CreatedAtAction(nameof(GetOrderById), new { id = order.id }, order);

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
            return Problem("There was a problem saving the Order");
        }
    }

    [HttpPut]
    [Route("{id:guid}")]

    public async Task<IActionResult> ChangeOrderStatusAsync(Guid id, [FromBody] OrderModel.RequestChangeStatus request)
    {
        if (request == null)
            return BadRequest("New order status is required");
        try
        {
            var changedOrder = await _service.ChangeOrderStatus(id, request);
            if (changedOrder == null)
            {
                return NotFound();
            }
            return Ok(changedOrder);
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
            return Problem("There was a problem changing the status of the Order");
        }
    }
}