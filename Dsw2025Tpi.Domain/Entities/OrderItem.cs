using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class OrderItem : EntityBase
{
    
    public OrderItem(int quantity, Guid orderId, Guid productId)  
    {
        Quantity = StockControl(quantity);
        UnitPrice = Product.CurrentUnitPrice;
        Subtotal = UnitPrice * Quantity;
        OrderId = orderId;
        ProductId = productId;

    }
    
    private int StockControl (int quantity) //------Consultar------
    {
        if (quantity <= Product.StockQuantity)
        {
            Product.StockQuantity -= quantity;
            return quantity;
        }
        else throw new Exception("Problema de stock"); //-------CAMBIAR-------
    }
    public  int Quantity 
    { 
        get => Quantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("The quantity must be a positive number or 0");
            }
        }
    }
    public  decimal UnitPrice 
    {
        get => UnitPrice; 
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("The unit price must be a positive number");
            }
        }
    }
    public decimal Subtotal { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; }
}
