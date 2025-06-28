using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderItemModel
{
    public record OrderItemRequest(int Quantity, ProductModel product);
    public record Response(Guid Id, int Quantity);

}