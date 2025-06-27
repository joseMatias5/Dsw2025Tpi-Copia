﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record ProductModel
{
    public record RequestProduct(string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity);
    public record ResponseProduct(Guid Id, string Sku, string InternalCode, string Name, string? Description, decimal CurrentUnitPrice, int StockQuantity);
}
