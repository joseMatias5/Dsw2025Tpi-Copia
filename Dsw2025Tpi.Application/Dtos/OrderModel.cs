using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Domain.Entities;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderModel
{
    public record Request(string ShippingAddress, string BillingAddress, string? Notes, Guid CustomerId, List<(Product, int)> products);    
    public record Response(Guid Id, string ShippingAddress, string BillingAddress, string? Notes, decimal TotalAmount, Guid CustomerId, List<(Product, int)> products);

}