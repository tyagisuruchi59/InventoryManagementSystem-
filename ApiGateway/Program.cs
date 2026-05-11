// StockPro Inventory Management System
// API Gateway | Entry Point
// Developer: Suru | April 2026

using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Force port 80 inside Docker
builder.WebHost.UseUrls("http://0.0.0.0:80");

// Load Ocelot config
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Ocelot
builder.Services.AddOcelot(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Swagger (for aggregated UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll");

app.UseSwagger();

// NOTE: Swagger endpoints go through the gateway using relative paths (/auth/swagger/...)
// This avoids CORS errors from the browser calling services directly.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "Auth Service");
    c.SwaggerEndpoint("/product/swagger/v1/swagger.json", "Product Service");
    c.SwaggerEndpoint("/warehouse/swagger/v1/swagger.json", "Warehouse Service");
    c.SwaggerEndpoint("/purchase/swagger/v1/swagger.json", "Purchase Service");
    c.SwaggerEndpoint("/supplier/swagger/v1/swagger.json", "Supplier Service");
    c.SwaggerEndpoint("/movement/swagger/v1/swagger.json", "Movement Service");
    c.SwaggerEndpoint("/alert/swagger/v1/swagger.json", "Alert Service");
    c.SwaggerEndpoint("/report/swagger/v1/swagger.json", "Report Service");
    c.RoutePrefix = "swagger";
});

// Ocelot MUST be last
await app.UseOcelot();

app.Run();