using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public Order() { }
    public Order(string shippingAddress, string billingAddress, string? notes, Guid customerId, List<(Product,int)> products )
    {
        Date = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Notes = notes;
        Status = OrderStatus.PENDING;
        CustomerId = customerId;
        OrderItems = products.Select(p=> new OrderItem(p.Item2, p.Item1)).ToList() ;
    }
    public OrderStatus Status { get; set; }
    public DateTime Date { get; set; } 
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }
    public decimal TotalAmount => OrderItems.Sum(oi => oi.Subtotal);
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}