using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Dsw2025Tpi.Application.Validations;

public class OrderValidations
{
    public static void ValidateFilteredArguments(OrderModel.FilterOrder request, IRepository _repository)
    {
        GeneralValidations.ValidateNotNull(request, nameof(request));
        if(request.CustomerId is not null)
        {
            var customer = request.CustomerId.ToString() ?? throw new ArgumentNullException("Customer id is null");
            GeneralValidations.ValidateGuidAndCodes(customer, nameof(request.CustomerId));
            Guid customerId = (Guid)request.CustomerId;
            ValidateCustomer(customerId, _repository);
        }

        if (request.Status != null)
        {
            ValidateFilteredStatus(request.Status);
        }

        if (!string.IsNullOrEmpty(request.PageNumber.ToString()))
        {
            GeneralValidations.ValidatePositiveWholeNumberAndCero(request.PageNumber.ToString()!, nameof(request.PageNumber));
            if(request.PageNumber > 1000)
                throw new ArgumentOutOfRangeException(nameof(request.PageNumber), "PageNumber number cannot be greater than 1000");
        }
        if (request.PageSize is not null)
        {
            GeneralValidations.ValidatePositiveWholeNumberAndCero(request.PageSize.ToString()!, nameof(request.PageSize));
            if (request.PageSize > 15)
                throw new ArgumentOutOfRangeException(nameof(request.PageSize), "PageNumber size cannot be greater than 15");
        }
    }
    public static void ValidateNotNullOrders(IEnumerable<Order>? orders, OrderModel.FilterOrder? request)
    {
        GeneralValidations.ValidateNotNull(request, nameof(request));
        if (orders == null || !orders.Any()  && request != null && request.CustomerId.HasValue)
            throw new NotFoundException($"No orders found for customer with ID {request!.CustomerId}");
        if (orders == null || !orders.Any() && request != null && !request.Status.IsNullOrEmpty())
            throw new NotFoundException($"No orders found with status {request!.Status}");
        if (orders == null || !orders.Any())
            throw new NoContentException("No orders found");

    }
    public static void ValidateOrder(OrderModel.RequestOrder request)
    {
        GeneralValidations.ValidateNotNull(request, nameof(request));
        GeneralValidations.ValidateNotNull(request.OrderItems, nameof(request.OrderItems));
        GeneralValidations.ValidateText(request.ShippingAddress!, nameof(request.ShippingAddress));
        GeneralValidations.ValidateText(request.BillingAddress!, nameof(request.BillingAddress));
        GeneralValidations.ValidateOptionalText(request.Notes!, nameof(request.Notes));
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

        var statusUpper = status.ToUpper();

        if (order.Status.ToString() == statusUpper)
            throw new InvalidStatusException($"Order is already in {status} status");

        var validStatuses = Enum.GetNames(typeof(OrderStatus))
            .Select(s => s.ToLowerInvariant());
        if (!validStatuses.Contains(status.ToLower()))
            throw new InvalidStatusException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");

        if (order.Status == OrderStatus.PENDING && statusUpper != "PROCESSING" && statusUpper != "CANCELLED")
            throw new InvalidStatusException($"Order status is PENDING, it cannot be changed to: {status}");
        if (order.Status == OrderStatus.PROCESSING && statusUpper != "SHIPPED" && statusUpper != "CANCELLED")
            throw new InvalidStatusException($"Order status is PROCESSING, it cannot be changed to: {status}");
        if (order.Status == OrderStatus.SHIPPED && statusUpper != "DELIVERED" && statusUpper != "CANCELLED")
            throw new InvalidStatusException($"Order status is SHIPPED, it cannot be changed to: {status}");
        if(order.Status == OrderStatus.CANCELLED)
            throw new InvalidStatusException($"Order status is CANCELLED, it cannot be changed to: {status}");
    }

    public static void ValidateFilteredStatus(string status)
    {
        GeneralValidations.ValidateText(status, nameof(status));
        var validStatuses = Enum.GetNames(typeof(OrderStatus))
            .Select(s => s.ToLowerInvariant());
        if (!validStatuses.Contains(status.ToLower()))
            throw new InvalidStatusException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", validStatuses)}");
    }
    public static void ValidateCancelledOrder(Order order)
    {
        if (order.Status.ToString() == "CANCELLED")
            throw new InvalidStatusException($"Order with ID {order.Id} is CANCELLED");
    }

    public static void ValidateCustomer(Guid customerId, IRepository _repository)
    {
        GeneralValidations.ValidateGuidAndCodes(customerId.ToString(), nameof(customerId));
        var customer = _repository.First<Customer>(c => c.Id == customerId).Result;
        if (customer == null)
            throw new Exceptions.EntityNotFoundException($"Customer with ID {customerId} not found");
    }
}
