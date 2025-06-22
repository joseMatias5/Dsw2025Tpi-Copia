using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record ProductMode
{
    public record Request(string Sku, string InternalCode, decimal Price);

    public record Response(Guid Id, string Sku, string InternalCode, decimal Price);
}
