using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record RequestOrder(string? shippingAddress, string? billingAddress,
        string? notes, Guid customerId, List<(int, Product)> orderItems);

    public record RequestChangeStatus(string newStatus);
    public record ResponseOrder(Guid id, DateTime date, string? shippingAddress, string? billingAddress,
        string? notes, Guid customerId, OrderStatus? status,List<(int, Product)> orderItems, decimal? totalAmount);
}
