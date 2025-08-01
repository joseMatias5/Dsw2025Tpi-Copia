
using System.Text;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Dsw2025Tpi.Api.Configurations;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;
using Serilog;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build())
            .Enrich.FromLogContext()
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog();

        builder.Services.AddRateLimiterService();

        builder.Services.AddControllers();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerConfiguration();

        builder.Services.AddIdentityAndAuthenticationServices(builder.Configuration);

        builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();

        builder.Services.AddHealthChecks();

        builder.Services.AddDomainServices(builder.Configuration);

        builder.Services.AddCorsService();

        builder.Services.AddTransient<CustomExceptionHandlingMiddleware>();

        var app = builder.Build();

        var rolesToCreate = builder.Configuration.GetSection("Roles").Get<List<string>>();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var roleName in rolesToCreate!)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors("PermitirFrontend");
        app.UseAuthentication();
        app.UseRateLimiter();
        app.UseAuthorization();

        app.UseMiddleware<CustomExceptionHandlingMiddleware>();
        app.MapControllers().RequireRateLimiting("fixed");


        app.MapHealthChecks("/health-check");

        app.Run();
    }
}
