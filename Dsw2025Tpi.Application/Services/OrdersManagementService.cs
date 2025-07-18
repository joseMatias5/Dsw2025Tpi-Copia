using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService : IOrdersManagementService
{
    IRepository _repository;
    public OrdersManagementService(IRepository repository)
    {
        _repository = repository;
    }
    public async Task<OrderModel.Response?> GetOrderById(Guid id)
    {
        var order = await _repository.GetById<Order>(id);
        return order != null ?
            new OrderModel.Response(
                order.Id,
                order.Date,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status,
                order.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
                order.TotalAmount) :
            null;
    }

    public async Task<IEnumerable<OrderModel.Response>?> GetOrders()
    {
        return (await _repository
            .GetFiltered<Order>(o => o.Status.Value == OrderStatus.CANCELLED))?
            .Select(order => new OrderModel.Response(
                order.Id,
                order.Date,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status,
                order.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
                order.TotalAmount)
            );
    }

    public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
    {
        Validations.OrderValidations.ValidateOrder(request);

        var order = new Order(request.shippingAddress, request.billingAddress, request.notes,
            request.customerId, request.products);
        return new OrderModel.Response(
            order.Id,
            order.Date,
            order.ShippingAddress,
            order.BillingAddress,
            order.Notes,
            order.CustomerId,
            order.Status,
            order.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
            order.TotalAmount);
    }

    public async Task<OrderModel.Response> UpdateOrder(Guid id, OrderModel.Request request)
    {
        Validations.OrderValidations.ValidateExistingOrder(id, _repository);
        var order = await _repository.First<Order>(p => p.Id == id);
        Validations.OrderValidations.ValidateOrder(request);

        order.ShippingAddress = request.shippingAddress;
        order.BillingAddress = request.billingAddress;
        order.Notes = request.notes;
        order.CustomerId = request.customerId;

        var updated = await _repository.Update(order);
        return new OrderModel.Response(
            updated.Id,
            updated.Date,
            updated.ShippingAddress,
            updated.BillingAddress,
            updated.Notes,
            updated.CustomerId,
            updated.Status,
            updated.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
            updated.TotalAmount
            );
    }

    public async Task<OrderModel.Response> DeleteOrder(Guid id)
    {
        var order = await _repository.First<Order>(p => p.Id == id);
        Validations.OrderValidations.ValidateExistingOrder(id, _repository);

        var deleted = await _repository.Update(order);
        return new OrderModel.Response(
            deleted.Id,
            deleted.Date,
            deleted.ShippingAddress,
            deleted.BillingAddress,
            deleted.Notes,
            deleted.CustomerId,
            deleted.Status,
            deleted.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
            deleted.TotalAmount
        );
    }
    //Para el PUT
    public async Task<OrderModel.Response?> ChangeOrderStatus(Guid id, OrderModel.Request request)
    {
        var order = await _repository.GetById<Order>(id);
        Validations.OrderValidations.ValidateExistingOrder(id, _repository);

        order.Status = request.status;
        var updated = await _repository.Update(order);

        return new OrderModel.Response(
            updated.Id,
            updated.Date,
            updated.ShippingAddress,
            updated.BillingAddress,
            updated.Notes,
            updated.CustomerId,
            updated.Status,
            updated.OrderItems.Select(oi => (oi.Quantity, oi.Product!)).ToList(),
            updated.TotalAmount
            );
    }
}
