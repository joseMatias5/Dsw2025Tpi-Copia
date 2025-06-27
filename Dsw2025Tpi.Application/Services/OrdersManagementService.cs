using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Services;

public class OrdersManagementService
{
    private readonly EfRepository _repository;
    public async Task<Order?> GetOrderById(Guid id)
    {
        return await _repository.GetById<Order>(id);
    }

    public async Task<IEnumerable<Order>?> GetOrder()
    {
        return await _repository.GetAll<Order>();

    }
    public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Date.ToString()) ||
            string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress))
        {
            throw new ArgumentException("Invalid values for order");
        }

        var order = new Order(request.Date, request.ShippingAddress, request.BillingAddress, request.Notes, request.CustomerId);
        await _repository.Add(order);
        return new OrderModel.Response(order.Id, order.Date, order.ShippingAddress, order.BillingAddress, order.Notes, order.TotalAmount, order.CustomerId);
    }
}
