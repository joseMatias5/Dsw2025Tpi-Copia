using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Application.Dtos;

public record OrderItemModel
{
    /*public record Request(int Quantity)
    {
        public ProductModel? Product { get; init; }
    }
    public record Response(Guid Id, int Quantity, decimal UnitPrice, decimal Subtotal)
    {
        public ProductModel? Product { get; init; }

    }*/

    public record OrderItemRequest(Guid ProductId, int Quantity);
    public record Response(Guid Id, int Quantity, decimal UnitPrice, decimal Subtotal, ProductModel.ResponseProduct Product);

}