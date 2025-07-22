using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Data.Helpers;

public static class DbContextExtensions
{
    public static void Seedwork<T>(this Dsw2025TpiContext context, string dataSource) where T : class
    {
        if (context.Set<T>().Any()) return;
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, dataSource));
        var entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        if (entities == null || entities.Count == 0) return;
        context.Set<T>().AddRange(entities);
        context.SaveChanges();
    }

    /*
    public static void SeedOrders(this Dsw2025TpiContext context, string jsonPath)
    {
        if (context.Orders.Any()) return;

        var fullPath = Path.Combine(AppContext.BaseDirectory, jsonPath);
        var json = File.ReadAllText(fullPath);

        var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (orders == null || orders.Count == 0) return;

        foreach (var order in orders)
        {
            // Setea fecha y status si no están en el JSON
            order.Date = DateTime.UtcNow;
            order.Status = OrderStatus.PENDING;

            // Vincular manualmente los productos
            foreach (var item in order.OrderItems)
            {
                var product = context.Products.Find(item.ProductId);
                if (product == null) continue;

                item.Product = product;
                item.UnitPrice = product.CurrentUnitPrice;
            }

            context.Orders.Add(order);
        }

        context.SaveChanges();
    }*/

    public static void SeedOrders(this Dsw2025TpiContext context, string jsonPath)
    {
        if (context.Orders.Any()) return;

        var fullPath = Path.Combine(AppContext.BaseDirectory, jsonPath);
        var json = File.ReadAllText(fullPath);

        var orders = JsonSerializer.Deserialize<List<Order>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (orders == null || orders.Count == 0) return;

        foreach (var dto in orders)
        {
            var customer = context.Customers.Find(dto.CustomerId);
            if (customer == null) continue;

            var order = new Order(dto.ShippingAddress, dto.BillingAddress, dto.CustomerId);

            foreach (var item in dto.OrderItems)
            {
                var product = context.Products.Find(item.ProductId);
                if (product == null) continue;

                // Evitás nulls que rompan el constructor
                var name = product.Name ?? "Producto sin nombre";
                var description = product.Description ?? "";

                order.AddOrderItem(product.Id, item.Quantity, name, description, product.CurrentUnitPrice);
            }

            context.Orders.Add(order);
        }

        context.SaveChanges();
    }
}
