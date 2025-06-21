﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities;

public class Customer : EntityBase
{
    
    public Customer (string email, string name, string phoneNumber)
    {
        Email = email;
        Name = name;    
        PhoneNumber = phoneNumber;
    }
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string PhoneNumber { get; set; }

    public ICollection<Order?> Orders { get; set; }
}
