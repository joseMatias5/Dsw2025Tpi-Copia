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
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService : IOrdersManagementService
{
    IRepository _repository;
    private readonly ILogger<OrdersManagementService> _logger;
    public OrdersManagementService(IRepository repository,
         ILogger<OrdersManagementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<OrderModel.ResponseOrder>?> GetOrders(OrderModel.SearchOrder request)
    {
        OrderStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status))
            status = Enum.Parse<OrderStatus>(request.Status.ToUpper(), true);

        if (request is null)
        {
            _logger.LogInformation("Consulta de ordenes sin filtrar");
        }
        else
        {
            _logger.LogInformation("Consulta de ordenes filtradas");
        }

        var orders = await _repository
            .GetFiltered<Order>(
                o => 
                    o.Status.Value != OrderStatus.CANCELLED
                    && (o.CustomerId == request.CustomerId || !request.CustomerId.HasValue)
                    && (!status.HasValue || o.Status == status.Value)
                    , 
                include: new[] { "OrderItems" }
            );
        OrderValidations.ValidateNotNullOrders(orders, request);

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
        _logger.LogInformation("Consulta de orden por Id: {id}", id);
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
        _logger.LogInformation("Creacion de orden");
        GeneralValidations.ValidateNotNull(request, nameof(request));
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
                product!,
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
        
        var order = new Order(request!.ShippingAddress!, request.BillingAddress!, request.Notes, request.CustomerId, orderItems);
        await _repository.Add(order);
        _logger.LogInformation("Creacion de orden exitosa");
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

    //Para el PUT
    public async Task<OrderModel.ResponseOrder?> ChangeOrderStatus(Guid id, OrderModel.RequestChangeStatus request)
    {
        _logger.LogInformation("Cambiar estado de orden con Id: {id}", id);
        var order = await _repository.First<Order>(p => p.Id == id, include: new[] { "OrderItems" });
        await OrderValidations.ValidateExistingOrder(id, _repository);
        OrderValidations.ValidateOrderStatus(order!, request.NewStatus.ToString());

        order!.Status = Enum.Parse<OrderStatus>(request.NewStatus.ToString().ToUpper(), true);
        var updated = await _repository.Update(order);
        _logger.LogInformation("Modificacion de orden exitosa");
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

