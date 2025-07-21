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

    private Product? _product;
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
        {
            if (product is null)
                throw new ArgumentNullException(nameof(product), "Product cannot be null");

            if (quantity < 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "The quantity must be positive");

            if (!product.StockControl(quantity))
                throw new ApplicationException("Not enough stock");

            Product = product;
            Quantity = quantity;
            UnitPrice = product.CurrentUnitPrice;
        }
    }
}
