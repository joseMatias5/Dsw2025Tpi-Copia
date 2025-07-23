
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Domain.Entities;
using Dsw2025Tpi.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        builder.Services.AddDbContext<Dsw2025TpiContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("Dsw2025TpiDb"));
            options.UseSeeding((c, t) =>
            {
                var dataDir = Path.Combine(AppContext.BaseDirectory, "Sources");

                ((Dsw2025TpiContext)c).Seedwork<Product>(Path.Combine(dataDir, "products.json"));
                ((Dsw2025TpiContext)c).Seedwork<Customer>(Path.Combine(dataDir, "customers.json"));
                ((Dsw2025TpiContext)c).SeedOrders(Path.Combine(dataDir, "orders.json"));
            });
        });
        builder.Services.AddScoped<IRepository, EfRepository>();
        builder.Services.AddTransient<IOrdersManagementService, OrdersManagementService>();
        builder.Services.AddTransient<IProductsManagementService, ProductsManagementService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health-check");

        app.Run();
    }
}
