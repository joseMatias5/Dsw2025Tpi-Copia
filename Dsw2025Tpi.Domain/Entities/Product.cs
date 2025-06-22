using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Product : EntityBase
{

    public Product (string sku, string internalCode, string name, string? description, decimal currentUnitPrice, int stockQuantity)
    {
        Sku = sku;
        InternalCode = internalCode;
        Name = name;
        Description = description;
        CurrentUnitPrice = currentUnitPrice; 
        StockQuantity = stockQuantity;  
        IsActive = true ;

    }
    public string Sku {  get; set; }
    public string InternalCode { get; set; }
    public  string Name { get; set; }
    public string? Description { get; set; }
    public  decimal CurrentUnitPrice 
    { 
        get => CurrentUnitPrice;
        set 
        {
            if(value <= 0)
            {
                throw new ArgumentOutOfRangeException("The unit price must be a positive number");
            }
        }
         
    }
    public  int StockQuantity 
    {
        get => StockQuantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("The stock quantity must be a positive number or 0");
            }
        }
    }
    public bool IsActive { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}
