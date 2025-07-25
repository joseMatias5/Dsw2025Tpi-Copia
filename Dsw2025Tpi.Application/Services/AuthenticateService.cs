using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Validations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Dsw2025Tpi.Application.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly IConfiguration _config;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthenticateService> _logger;

    public AuthenticateService(
        IConfiguration config,
        UserManager<IdentityUser> userManager,
        ILogger<AuthenticateService> logger)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config)); 
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager)); 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
    }

    public string GenerateToken(string username)
    {
        var jwtConfig = _config.GetSection("Jwt");
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt Key");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(double.Parse(jwtConfig["ExpireInMinutes"] ?? "60")),
            signingCredentials: creds
         );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<IdentityUser> Login(LoginModel.RequestLogin request)
    {
        _logger.LogInformation("Solicitud de ingreso");

        var user = await _userManager.FindByNameAsync(request.Username);
        
        Validations.AuthenticateValidations.ValidateLogin(request, user!);

        _logger.LogInformation("Solicitud de ingreso exitosa");
        return user!;
    }
    public async Task<RegisterModel.ResponseRegister> Register(RegisterModel.RequestRegister model)
    {
        _logger.LogInformation("Solicitud de registro");
        AuthenticateValidations.ValidateRegistration(model, _userManager);
        var user = new IdentityUser { UserName = model.Username, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            throw new ApplicationException(result.Errors.ToString());

        _logger.LogInformation("Solicitud de registro exitosa");
        return new RegisterModel.ResponseRegister();

    }
}
