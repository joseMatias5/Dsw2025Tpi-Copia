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
        GeneralValidations.ValidateNotNull(request, nameof(request));
        GeneralValidations.ValidateText(request.ShippingAddress, nameof(request.ShippingAddress));
        GeneralValidations.ValidateText(request.BillingAddress, nameof(request.BillingAddress));
        GeneralValidations.ValidateOptionalText(request.Notes, nameof(request.Notes));
        GeneralValidations.ValidateGuidAndCodes(request.CustomerId.ToString(), nameof(request.CustomerId));
    }

    public async static Task ValidateExistingOrder(Guid id, IRepository _repository)
    {
        GeneralValidations.ValidateGuidAndCodes(id.ToString(), nameof(id));
        if (await _repository.First<Order>(p => p.Id == id) == null)
            throw new Exceptions.EntityNotFoundException($"Order with ID {id} not found");
    }

    public static void ValidateOrderStatus(Order order, string status)
    {
        GeneralValidations.ValidateNotNull(order, nameof(order));
        GeneralValidations.ValidateText(status, nameof(status));

        if (order.Status.ToString().ToLower() == status)
            throw new ArgumentException($"Order is already in {status} status");
        var validStatuses = Enum.GetNames(typeof(OrderStatus))
            .Select(s => s.ToLowerInvariant());
        if (!validStatuses.Contains(status.ToLower()))
            throw new ArgumentException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");
    }
    public static void ValidateCancelledOrder(Order order)
    {
        if (order.Status.ToString().ToLower() == "cancelled")
            throw new ArgumentException($"Order with ID {order.Id} is cancelled");
    }

    public static void ValidateCustomer(Guid customerId, IRepository _repository)
    {
        GeneralValidations.ValidateGuidAndCodes(customerId.ToString(), nameof(customerId));
        var customer = _repository.First<Customer>(c => c.Id == customerId).Result;
        if (customer == null)
            throw new Exceptions.EntityNotFoundException($"Customer with ID {customerId} not found");
    }
}
