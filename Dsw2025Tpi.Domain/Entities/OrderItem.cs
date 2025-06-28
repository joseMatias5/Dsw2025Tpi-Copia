using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{
    public OrderItem(int quantity,Product product )
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "The quantity must be postive.");
        if (product == null)
            throw new ArgumentNullException(nameof(product), "The product cannot be null.");
        
        Quantity = product.ControlStock(quantity)? quantity : throw new Exception("Stock problem") ;
        UnitPrice = product.CurrentUnitPrice;
    }

    public int Quantity { get;  set; }
    public decimal UnitPrice { get;  set; }
    public decimal Subtotal => UnitPrice * Quantity;

    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
}
