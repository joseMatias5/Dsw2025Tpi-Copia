using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record Request(DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount);
    public record Response(Guid Id, DateTime Date, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount);

}
