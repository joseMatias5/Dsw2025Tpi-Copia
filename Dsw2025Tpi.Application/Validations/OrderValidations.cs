using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;

namespace Dsw2025Tpi.Application.Validations;

public class OrderValidations
{
    public static void ValidateNotNullOrders(IEnumerable<Order>? orders)
    {
        if (orders == null || !orders.Any())
            throw new EntityNotFoundException("No orders found");
    }
    public static void ValidateOrder(OrderModel.RequestOrder request)
    {
        NullValidations.ValidateNotNull(request, nameof(request));

        if (request.OrderItems == null || !request.OrderItems.Any())
            throw new ArgumentException("Order must contain at least one item", nameof(request.OrderItems));

        if (string.IsNullOrWhiteSpace(request.ShippingAddress))
            throw new ArgumentException("Shipping address cannot be null or empty", nameof(request.ShippingAddress));

        if (string.IsNullOrWhiteSpace(request.BillingAddress))
            throw new ArgumentException("Billing address cannot be null or empty", nameof(request.BillingAddress));

        if (request.CustomerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(request.CustomerId));
    }

    public async static Task ValidateExistingOrder(Guid id, IRepository _repository)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(id));
        if (await _repository.First<Order>(p => p.Id == id) == null)
            throw new Exceptions.EntityNotFoundException($"Order with ID {id} not found");
    }

    public static void ValidateOrderStatus(Order order, string status)
    {
        if(order.Status.ToString().ToLower() == status)
            throw new ArgumentException($"Order is already in {status} status", nameof(status));
        var validStatuses = Enum.GetNames(typeof(OrderStatus))
            .Select(s => s.ToLowerInvariant()); ;
        if (!validStatuses.Contains(status.ToLower()))
            throw new ArgumentException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}", nameof(status));
    }
    public static void ValidateCancelledOrder(Order order)
    {
        if (order.Status.ToString().ToLower() == "cancelled")
            throw new ArgumentException($"Order with ID {order.Id} is cancelled");
    }

    public static void ValidateItem(OrderModel.RequestOrder order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any())
            throw new ArgumentException("Order must contain at least one item");
    }

    public static void ValidateCustomer(Guid customerId, IRepository _repository)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));
        var customer = _repository.First<Customer>(c => c.Id == customerId).Result;
        if (customer == null)
            throw new Exceptions.EntityNotFoundException($"Customer with ID {customerId} not found");
    }
}
