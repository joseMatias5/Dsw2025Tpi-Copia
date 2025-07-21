using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderItemModel
{
    //public record OrderItemRequest(int Quantity, ProductModel product);
    //public record ResponseItem(Guid Id, int Quantity, ProductModel.ResponseProduct product);
    public record RequestItem(Guid ProductId, string? Name, string? Description, decimal UnitPrice, int Quantity);
    public record ResponseItem(Guid productId, string? name, string? description, decimal? unitPrice, int? quantity);
}
  