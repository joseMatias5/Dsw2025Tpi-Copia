using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Order : EntityBase
{
   public Order ( DateTime date, string shippingAddress, string billingAddress, string? notes, decimal totalAmount )
    {
        Date = date;    
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
        Notes = notes;
        TotalAmount = totalAmount;
        Status = OrderStatus.PENDING;

    }

    public required OrderStatus Status { get; set; }

    public required DateTime Date {  get; set; } //pendiente la validacion de fecha

    public required string ShippingAddress { get; set; }
    public required string BillingAddress { get; set; }
    public string? Notes {  get; set; }
    public decimal TotalAmount { get ; set; }

    public Guid? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}
