// StockPro Inventory Management System
// Service: Supplier Service | Entry Point
// Developer: Suru | April 2026
// Description: Configures PostgreSQL database, JWT authentication,
// dependency injection, Swagger, and middleware pipeline.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using SupplierService.Data;
using SupplierService.Repositories;
using SupplierService.Services;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// DATABASE - Connect to PostgreSQL supplierdb
// =============================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================================
// DEPENDENCY INJECTION
// =============================================
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierServiceImpl>();

// =============================================
// CONTROLLERS
// =============================================
builder.Services.AddControllers();

// =============================================
// CORS - Allow React frontend
// =============================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// =============================================
// SWAGGER WITH JWT SUPPORT
// =============================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockPro Supplier Service",
        Version = "v1",
        Description = "Supplier Master Register Management for StockPro"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer YOUR_TOKEN"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// =============================================
// JWT AUTHENTICATION - same key as all services
// =============================================
var key = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// =============================================
// AUTO MIGRATE DATABASE ON STARTUP
// =============================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// =============================================
// MIDDLEWARE PIPELINE
// =============================================
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowReact");        // ← CORS added here
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();