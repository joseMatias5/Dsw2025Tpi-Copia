using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Dsw2025Tpi.Application.Validations;

public static class AuthenticateValidations
{
    //public static void ValidateNotNull()
    public static void ValidateLogin(LoginModel.RequestLogin request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));
        
        ValidateUsername(request.Username);
        ValidatePassword(request.Password);
    }
    public static void ValidateRegistration(RegisterModel.RequestRegister model)
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

        //    var pattern = @"^[a-zA-Z0-9.!#$%&'+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)$";
        var name = email.Split('@').FirstOrDefault();
        if (string.IsNullOrEmpty(name) || name.Length < 3)
        {
            throw new ArgumentException("Email must have at least 3 characters before '@'");
        }
        var domain = email.Split('@').Last();
        if (!allowedDomains.Contains(domain))
        {
            throw new ArgumentException("Use an allowed domain.");
        }
    }
}
