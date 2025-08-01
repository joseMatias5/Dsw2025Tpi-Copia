using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{
   
    public string? Sku { get; set; }
    public string? InternalCode { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal CurrentUnitPrice { get; set; }
    public int? StockQuantity {  get; set;}
    public bool IsActive { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }

    public Product() { }
    public Product(string? sku, string? internalCode, string? name, string? description, 
        decimal currentUnitPrice, int? stockQuantity)
    {
        Sku = sku;
        InternalCode = internalCode;
        Name = name;
        Description = description;
        CurrentUnitPrice = currentUnitPrice <= 0? 
            throw new ArgumentOutOfRangeException(nameof(stockQuantity), "The Unit Price must be positive") : currentUnitPrice;
        StockQuantity = stockQuantity < 0 ?
            throw new ArgumentOutOfRangeException(nameof(stockQuantity), "The Quantity must be positive") : stockQuantity;
        IsActive = true;
    }
    public bool StockControl(int quantity)
    {
        if (quantity <= StockQuantity)
        {
            StockQuantity -= quantity;
            return true;
        }
        return false;
    }
}
