using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
    public DateTime Date { get; set; }
    public string? ShippingAddress { get; set; }
    public string? BillingAddress { get; set; }
    public string? Notes { get; set; }

    [NotMapped]
    public decimal? TotalAmount => OrderItems?.Sum(item => item.SubTotal) ?? 0;
    
    public ICollection<OrderItem> OrderItems { get; set; }
    public Guid CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public OrderStatus? Status { get; set; }
    
    protected Order()
    {
        OrderItems = new List<OrderItem>();
    }
    public Order(string? shippingAddress, string? billingAddress,
        string? notes, Guid customerId, List<(int, Product)> orderItems)
        : this()
    {
        Date = DateTime.UtcNow;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Notes = notes;
        CustomerId = customerId;
        Status = OrderStatus.PENDING;
        OrderItems = orderItems.Select(p => new OrderItem(p.Item1, p.Item2)).ToList();
    }
}
