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

        var order = new Order(request.ShippingAddress, request.BillingAddress, request.Notes, request.CustomerId);
        await _repository.Add(order);
        return new OrderModel.Response(order.Id, order.Date, order.ShippingAddress, order.BillingAddress, order.Notes, order.TotalAmount, order.CustomerId, order.OrderItems);
    }
}
    
/*
public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Date.ToString()) ||
            string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress))
        {
            throw new ArgumentException("Invalid values for order");
        }

        // 1. Crear la orden
        var order = new Order(request.Date, request.ShippingAddress, request.BillingAddress, request.Notes, request.CustomerId);

        // 2. Preparar los ProductOrderInfo
        var productsInfo = new List<Order.ProductOrderInfo>();
        foreach (var item in request.Items) // request.Items: lista de productos y cantidades
        {
            var product = await _repository.GetById<Product>(item.Product.Id);
            if (product == null)
                throw new InvalidOperationException($"Producto con ID {item.Product} no encontrado.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");

            // Descontar stock
            product.StockQuantity -= item.Quantity;

            productsInfo.Add(new Order.ProductOrderInfo
            {
                Product = product,
                Quantity = item.Quantity
            });
        }

        // 3. Agregar los OrderItems a la orden
        order.AddOrderItems(productsInfo);

        // 4. Guardar la orden y los productos actualizados
        await _repository.Add(order);
        foreach (var info in productsInfo)
            await _repository.Update(info.Product);

        return new OrderModel.Response(order.Id, order.Date, order.ShippingAddress, order.BillingAddress, order.Notes, order.TotalAmount, order.CustomerId);
    }

    public async Task<OrderModel.Response> AddOrder(OrderModel.Request request)
    {
        if (string.IsNullOrWhiteSpace(request.Date.ToString()) ||
            string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress))
        {
            throw new ArgumentException("Invalid values for order");
        }

        // 1. Crear la orden
        var order = new Order(request.Date, request.ShippingAddress, request.BillingAddress, request.Notes, request.CustomerId);

        // 2. Preparar los ProductOrderInfo
        var productsInfo = new List<Product>();
        foreach (var item in request.Items) // request.Items: lista de productos y cantidades
        {
            var product = await _repository.GetById<Product>(item.Product.Id);
            if (product == null)
                throw new InvalidOperationException($"Producto con ID {item.Product} no encontrado.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");

            // Descontar stock
            product.StockQuantity -= item.Quantity;

            productsInfo.Add(new Order.ProductOrderInfo
            {
                Product = product,
                Quantity = item.Quantity
            });
        }

        // 3. Agregar los OrderItems a la orden
        order.AddOrderItems(productsInfo);

        // 4. Guardar la orden y los productos actualizados
        await _repository.Add(order);
        foreach (var info in productsInfo)
            await _repository.Update(info.Product);

        return new OrderModel.Response(order.Id, order.Date, order.ShippingAddress, order.BillingAddress, order.Notes, order.TotalAmount, order.CustomerId);
    }*/

    /*
public async Task<OrderModel.OrderItemResponse> AddOrder(OrderModel.Request request)
    {
        // Validación básica
        if (string.IsNullOrWhiteSpace(request.ShippingAddress) ||
            string.IsNullOrWhiteSpace(request.BillingAddress))
            throw new ArgumentException("Direcciones requeridas.");

        var order = new Order(request.ShippingAddress, request.BillingAddress, request.Notes, request.CustomerId);

        var productsInfo = new List<Order.ProductOrderInfo>();
        foreach (var item in request.Items)
        {
            var product = await _repository.GetById<Product>(item.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Producto con ID {item.ProductId} no encontrado.");

            if (product.StockQuantity < item.Quantity)
                throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}");

            product.StockQuantity -= item.Quantity;

            productsInfo.Add(new Order.ProductOrderInfo
            {
                Product = product,
                Quantity = item.Quantity
            });
        }

        order.AddOrderItems(productsInfo);

        await _repository.Add(order);
        foreach (var info in productsInfo)
            await _repository.Update(info.Product);

        return new OrderModel.OrderResponse(order.Id, order.ShippingAddress, order.BillingAddress, order.Notes, order.TotalAmount, order.CustomerId, order.OrderItems);
    }*/