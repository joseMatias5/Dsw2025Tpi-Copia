using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dsw2025Tpi.Application.Dtos.OrderItemModel;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record Request(DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, Guid CustomerId, List<OrderItemModel.OrderItemRequest> OrderItems);    
    public record Response(Guid Id, DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount, Guid CustomerId, List<OrderItemModel.Response> OrderItems);

}