using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;
using static Dsw2025Tpi.Application.Dtos.OrderItemModel;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record RequestChangeStatus(string newStatus);
    public record RequestOrder(string? ShippingAddress, string? BillingAddress, string? Notes, Guid CustomerId, List<RequestItem>? OrderItems);
    public record ResponseOrder(Guid id, DateTime date, string? shippingAddress, string? billingAddress,
        string? notes, Guid customerId, string status, List<ResponseItem> orderItems, decimal? totalAmount);

}
