using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderItemModel
{
    public record RequestItem(int Quantity, decimal UnitPrice, decimal Subtotal);
    public record ResponseItem(Guid Id, int Quantity, decimal UnitPrice, decimal Subtotal);

}
