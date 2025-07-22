using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Application.Validations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Dsw2025Tpi.Application.Dtos.OrderItemModel;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService : IOrdersManagementService
{
    IRepository _repository;
    public OrdersManagementService(IRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<OrderModel.ResponseOrder>?> GetOrders()
    {
        var orders = await _repository
            .GetFiltered<Order>(
                o => o.Status.Value != OrderStatus.CANCELLED,
                include: new[] { "OrderItems" }
            );

        return orders?.Select(order => new OrderModel.ResponseOrder(
            order.Id,
            order.Date,
            order.ShippingAddress,
            order.BillingAddress,
            order.Notes,
            order.CustomerId,
            order.Status?.ToString(),
            order.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
            order.TotalAmount)
        );
    }

    public async Task<OrderModel.ResponseOrder?> GetOrderById(Guid id)
    {
        await OrderValidations.ValidateExistingOrder(id, _repository);
        var order = await _repository.GetById<Order>(id, include: new[] { "OrderItems" });
        OrderValidations.ValidateCancelledOrder(order!);
        return order != null ?
            new OrderModel.ResponseOrder(
                order.Id,
                order.Date,
                order.ShippingAddress,
                order.BillingAddress,
                order.Notes,
                order.CustomerId,
                order.Status?.ToString(),
                order.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                    i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
                order.TotalAmount) :
            null;
    }


    public async Task<OrderModel.ResponseOrder> AddOrder(OrderModel.RequestOrder request)
    {
        OrderValidations.ValidateItem(request);
        var items = request!.OrderItems!.Select(item => new RequestItem(
            item.ProductId,
            item.Quantity
        )).ToList();

        List<OrderItem> orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            var product = await _repository.GetById<Product>(item.ProductId);
            await ProductValidations.ValidateExistingProduct(item.ProductId, _repository);
            ProductValidations.ValidateActiveProduct(product!);

            ItemValidations.ValidateItem(item);
            ItemValidations.StockControl(item.Quantity, product!);

            OrderItem orderItem= new OrderItem(
                item.ProductId,
                product!.Name!,
                product.Description,
                product.CurrentUnitPrice,
                item.Quantity
            );
            orderItem.Product = product;
            
            orderItems.Add(orderItem);
        }

        OrderValidations.ValidateOrder(request);
        OrderValidations.ValidateCustomer(request.CustomerId, _repository);
        
        var order = new Order(request!.ShippingAddress!, request.BillingAddress!, request.CustomerId, orderItems);
        await _repository.Add(order);

        return new OrderModel.ResponseOrder(
            order.Id,
            order.Date,
            order.ShippingAddress,
            order.BillingAddress,
            order.Notes,
            order.CustomerId,
            order.Status?.ToString(),
            order.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
            order.TotalAmount
        );
    }

    public async Task<OrderModel.ResponseOrder> UpdateOrder(Guid id, OrderModel.RequestOrder request)
    {
        await OrderValidations.ValidateExistingOrder(id, _repository);
        var order = await _repository.First<Order>(p => p.Id == id, include: new[] { "OrderItems" });
        OrderValidations.ValidateOrder(request);
        OrderValidations.ValidateCancelledOrder(order!);
        OrderValidations.ValidateItem(request);

        order!.ShippingAddress = request.ShippingAddress;
        order.BillingAddress = request.BillingAddress;
        order.Notes = request.Notes;
        order.CustomerId = request.CustomerId;

        var items = request!.OrderItems!.Select(item => new RequestItem(
             item.ProductId,
             item.Quantity
         )).ToList();

        List<OrderItem> orderItems = new List<OrderItem>();

        foreach (var item in items)
        {
            var product = await _repository.GetById<Product>(item.ProductId);
            await ProductValidations.ValidateExistingProduct(item.ProductId, _repository);
            ProductValidations.ValidateActiveProduct(product!);

            ItemValidations.ValidateItem(item);
            ItemValidations.StockControl(item.Quantity, product!);

            OrderItem orderItem = new OrderItem(
                item.ProductId,
                product!.Name!,
                product.Description,
                product.CurrentUnitPrice,
                item.Quantity
            );
            orderItem.Product = product;

            orderItems.Add(orderItem);
        }

        order.OrderItems = orderItems;

        var updated = await _repository.Update(order);
        return new OrderModel.ResponseOrder(
            updated.Id,
            updated.Date,
            updated.ShippingAddress,
            updated.BillingAddress,
            updated.Notes,
            updated.CustomerId,
            updated.Status?.ToString(),
            updated.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
            updated.TotalAmount
        );
    }

    public async Task<OrderModel.ResponseOrder> DeleteOrder(Guid id)
    {
        var order = await _repository.First<Order>(p => p.Id == id);
        await OrderValidations.ValidateExistingOrder(id, _repository);
        order!.Status = OrderStatus.CANCELLED;
        var deleted = await _repository.Update(order);
        return new OrderModel.ResponseOrder(
            deleted!.Id,
            deleted.Date,
            deleted.ShippingAddress,
            deleted.BillingAddress,
            deleted.Notes,
            deleted.CustomerId,
            deleted.Status?.ToString(),
            deleted.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
            deleted.TotalAmount
        );
    }
    //Para el PUT
    public async Task<OrderModel.ResponseOrder?> ChangeOrderStatus(Guid id, OrderModel.RequestChangeStatus request)
    {
        var order = await _repository.First<Order>(p => p.Id == id, include: new[] { "OrderItems" });
        await OrderValidations.ValidateExistingOrder(id, _repository);
        OrderValidations.ValidateOrderStatus(order!, request.newStatus.ToString());

        order!.Status = Enum.Parse<OrderStatus>(request.newStatus.ToString(), true);
        var updated = await _repository.Update(order);

        return new OrderModel.ResponseOrder(
            updated.Id,
            updated.Date,
            updated.ShippingAddress,
            updated.BillingAddress,
            updated.Notes,
            updated.CustomerId,
            updated.Status?.ToString(),
            updated.OrderItems.Select(i => new OrderItemModel.ResponseItem(
                i.ProductId, i.Name, i.Description, i.UnitPrice, i.Quantity)).ToList(),
            updated.TotalAmount
            );
    }
}

