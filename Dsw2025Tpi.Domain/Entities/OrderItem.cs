using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{
    public int? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? SubTotal => Quantity * UnitPrice;
    public Guid ProductId { get; set; }

    private Product _product;
    public Product? Product
    {
        get=> _product;
        set
        {
            if (Product?.IsActive == false)
            {
                throw new InvalidOperationException("Product has to be active");
            }
            _product = value;
        }
    }
    public Guid OrderId { get; set; }
    public Order? Order { get; set; }

    public OrderItem(int quantity, Product product)
    {
        Quantity = quantity < 0? 
            throw new ArgumentOutOfRangeException(nameof(quantity), "The Unit Price must be positive") : 
            Product.StockControl(quantity)? quantity : throw new ArgumentException(nameof(quantity), "Stock problems");
        UnitPrice = Product?.CurrentUnitPrice;
        Product = product ?? throw new ArgumentNullException(nameof(product), "Product cannot be null");
    }
}
