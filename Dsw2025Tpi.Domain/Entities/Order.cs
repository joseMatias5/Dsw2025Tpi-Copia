using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public Order(string shippingAddress, string billingAddress, string? notes, Guid customerId)
    {
        Date = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Notes = notes;
        Status = OrderStatus.PENDING;
        CustomerId = customerId;
        OrderItems = new List<OrderItem>();
        TotalAmount = 0m;
    }
    public OrderStatus Status { get; set; }
    public DateTime Date { get; set; } 
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }

    public void AddOrderItems(IEnumerable<ProductOrderInfo> productsInfo)
    {
        if (productsInfo == null)
            throw new ArgumentNullException(nameof(productsInfo));

        foreach (var info in productsInfo)
        {
            if (info.Quantity <= 0)
                throw new ArgumentException("The quantity must be positive", nameof(info.Quantity));
            if (info.Product == null)
                throw new ArgumentNullException(nameof(info.Product));

            var orderItem = new OrderItem(
                info.Quantity,
                info.Product.CurrentUnitPrice,
                this.Id,
                info.Product.Id
            )
            {
                Product = info.Product,
                Order = this
            };

            this.OrderItems.Add(orderItem);
        }
        this.TotalAmount = this.OrderItems.Sum(oi => oi.Subtotal);
    }

    public void AddOrderItems(IEnumerable<Product> list)
    {
        if (list == null)
            throw new ArgumentNullException(nameof(list));
        foreach (var product in list)
        {
            if (product.StockQuantity <= 0)
                throw new ArgumentException("La cantidad debe ser positiva.", nameof(product.StockQuantity));
            var orderItem = new OrderItem(
                1, // Assuming quantity is always 1 for this method
                product.CurrentUnitPrice,
                this.Id,
                product.Id
            )
            {
                Product = product,
                Order = this
            };
            this.OrderItems.Add(orderItem);
        }
        this.TotalAmount = this.OrderItems.Sum(oi => oi.Subtotal);
    }

    public class ProductOrderInfo
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}