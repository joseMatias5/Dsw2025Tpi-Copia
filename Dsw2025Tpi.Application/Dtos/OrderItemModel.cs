using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderItemModel
{
<<<<<<< HEAD
    public record Request(int Quantity);
    public record Response(Guid Id, int Quantity, decimal UnitPrice, decimal Subtotal);
=======
    public record RequestItem(int Quantity, decimal UnitPrice, decimal Subtotal);
    public record ResponseItem(Guid Id, int Quantity, decimal UnitPrice, decimal Subtotal);
>>>>>>> 9ab154043441385fa5a2d5b5c642ea225522d06b

}
