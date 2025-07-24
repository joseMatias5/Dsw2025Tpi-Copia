using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;

namespace Dsw2025Tpi.Application.Validations;

public static class RegisterValidation
{

    public static void ValidateModel(RegisterModel model)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        ValidatePassword(model.Password);
        ValidateEmail(model.Email);
        ValidateUsername(model.Username);
    }

    public static void ValidatePassword(string password)
    {
        if (password == null)
            throw new ArgumentNullException(nameof(password));
    }
    public static void ValidateUsername(string username)
    {
        if (username == null) 
            throw new ArgumentNullException("Username cannot be null");
        if (username.Length < 6)
            throw new ArgumentException("Username must have at least 6 characters");
    }
    public static void ValidateEmail(string email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));
        if (!email.Contains("@"))
            throw new ArgumentException("Email must contain '@'");
        if (!email.Contains("."))
            throw new ArgumentException("Email must contain '.'");

        var allowedDomains = new[] { 
            "gmail.com", 
            "hotmail.com", 
            "yahoo.com.ar", 
            "outlook.com",
            "alu.utn.frt.edu.ar",
            "doc.utn.frt.edu.ar",
            "confluencia.net"
        };

        var domain = email.Split('@').Last();
        if (!allowedDomains.Contains(domain))
        {
            throw new ArgumentException("Use an allowed domain.");
        }
    }
}
