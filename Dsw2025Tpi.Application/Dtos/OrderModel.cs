using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record RequestOrder(DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount);
    public record ResponseOrder(Guid Id, DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount);

}
