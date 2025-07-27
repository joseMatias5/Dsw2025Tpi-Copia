using System.Text.RegularExpressions;
using Dsw2025Tpi.Application.Dtos;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace Dsw2025Tpi.Application.Validations;

public static class AuthenticateValidations
{
    public static void ValidateLogin(LoginModel.RequestLogin request, IdentityUser user)
    {
        GeneralValidations.ValidateNotNull(request, nameof(request));

        ValidateUsername(request.Username);
        ValidatePassword(request.Password);

        if (user == null)
            throw new Application.Exceptions.ApplicationException("The username or password is incorrect");
    }
    public static void ValidateRegistration(RegisterModel.RequestRegister model, UserManager<IdentityUser> _userManager)
    {
        GeneralValidations.ValidateNotNull(model, nameof(model));

        ValidatePassword(model.Password);
        ValidateEmail(model.Email);
        ValidateUsername(model.Username);
        ValidateRole(model.Role);

        var user = _userManager.FindByNameAsync(model.Username).Result;
        if (user != null)
            throw new ArgumentException($"Username {model.Username} already exists");

        user = _userManager.FindByEmailAsync(model.Email).Result;
        if (user != null)
            throw new ArgumentException($"Email {model.Email} already exists");
    }
    public static void ValidatePassword(string password)
    {
        GeneralValidations.ValidateNotNull(password, nameof(password));
        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$"))
        {
            throw new ArgumentException($"{password}: invalid password");
        }
    }
    public static void ValidateUsername(string username)
    {
        GeneralValidations.ValidateText(username, nameof(username));
        if (username.Length < 6)
            throw new ArgumentException("Username must have at least 6 characters");
    }
    public static void ValidateEmail(string email)
    {
        GeneralValidations.ValidateNotNull(email, nameof(email));
        var pattern = @"^[a-zA-Z0-9.!#$%&'+-/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)+$";

        var regex = new Regex(pattern);
        if (!regex.IsMatch(email))
            throw new ArgumentException("Invalid email adress");
    }

    public static void ValidateRole(string role)
    {
        GeneralValidations.ValidateText(role, nameof(role));
        var validRoles = new[] { "ADMIN", "USER" };
        if (!validRoles.Contains(role.ToUpper()))
            throw new ArgumentException($"Invalid role: {role}. Valid roles are: {string.Join(", ", validRoles)}");
    }
}
