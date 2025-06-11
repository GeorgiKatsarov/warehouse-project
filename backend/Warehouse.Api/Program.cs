using Microsoft.EntityFrameworkCore;
using Warehouse.Api.Data;
using Warehouse.Api.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using System.Threading;


var builder = WebApplication.CreateBuilder(args);

// Service configuration
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Middleware configuration
ConfigureMiddleware(app);

// API endpoints
ConfigureEndpoints(app);

app.Run();

// Service configuration method
void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();
    
    services.AddDbContext<WarehouseDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("WarehouseDb")));
    
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAngularApp",
            builder => builder
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader());
    });
}

// Middleware configuration method
void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse API V1");
            options.RoutePrefix = string.Empty;
            options.DocumentTitle = "Warehouse API Documentation";
        });
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowAngularApp");
}

// API endpoints configuration method
void ConfigureEndpoints(WebApplication app)
{
    app.MapGet("/api/products", async (WarehouseDbContext db) =>
        await db.Products.ToListAsync());

    app.MapGet("/api/products/{id}", async (int id, WarehouseDbContext db) =>
        await db.Products.FindAsync(id)
            is Product product
            ? Results.Ok(product)
            : Results.NotFound());

    app.MapPost("/api/products", async (Product product, WarehouseDbContext db) =>
    {
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return Results.Created($"/api/products/{product.Id}", product);
    });

    app.MapPut("/api/products/{id}", async (int id, Product inputProduct, WarehouseDbContext db) =>
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();
        
        product.Name = inputProduct.Name;
        product.Description = inputProduct.Description;
        product.Price = inputProduct.Price;
        product.SKU = inputProduct.SKU;
        product.QuantityInStock = inputProduct.QuantityInStock;
        
        await db.SaveChangesAsync();
        return Results.NoContent();
    });

    app.MapDelete("/api/products/{id}", async (int id, WarehouseDbContext db) =>
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();
        
        db.Products.Remove(product);
        await db.SaveChangesAsync();
        return Results.Ok(product);
    });
}