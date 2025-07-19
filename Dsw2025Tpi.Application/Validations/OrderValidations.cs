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
    public static void ValidateOrder(OrderModel.RequestOrder request)
    {
        if (request.orderItems == null || !request.orderItems.Any())
            throw new ArgumentException("Order must contain at least one item", nameof(request.orderItems));

        if (string.IsNullOrWhiteSpace(request.shippingAddress))
            throw new ArgumentException("Shipping address cannot be null or empty", nameof(request.shippingAddress));

        if (string.IsNullOrWhiteSpace(request.billingAddress))
            throw new ArgumentException("Billing address cannot be null or empty", nameof(request.billingAddress));

        if (request.customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(request.customerId));
    }

    public async static Task ValidateExistingOrder(Guid id, IRepository _repository)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(id));
        if (await _repository.First<Order>(p => p.Id == id) == null)
            throw new EntityNotFoundException($"Order with ID {id} not found");
    }

    public static void ValidateOrderStatus(Order order, string status)
    {
        if(order.Status.ToString() == status)
            throw new ArgumentException($"Order is already in {status} status", nameof(status));
        var validStatuses = Enum.GetNames(typeof(OrderStatus));
        if (!validStatuses.Contains(status))
            throw new ArgumentException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}", nameof(status));
    }
}
