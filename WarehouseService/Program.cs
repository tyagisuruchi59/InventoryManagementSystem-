// StockPro Inventory Management System
// Service: Warehouse Service (UC3)
// Developer: Suru | April 2026
// Description: Manages warehouses and stock levels per warehouse
//              Supports inter-warehouse stock transfers

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// SECRET KEY — must be same as Auth Service
// =============================================
var key = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";

// =============================================
// SERVICES SETUP
// =============================================
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT Authorize button
builder.Services.AddSwaggerGen(options =>
{
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

// JWT Authentication — verifies token from Auth Service
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
// MIDDLEWARE
// =============================================
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

// =============================================
// IN-MEMORY DATA (like a temporary database)
// =============================================
var warehouses = new List<Warehouse>();
var stockLevels = new List<StockLevel>();

// =============================================
// WAREHOUSE APIS
// =============================================

// CREATE warehouse — Admin only
app.MapPost("/warehouses", (Warehouse warehouse) =>
{
    // Check if warehouse with same name already exists
    if (warehouses.Any(w => w.Name == warehouse.Name))
        return Results.BadRequest("Warehouse with this name already exists");

    warehouses.Add(warehouse);
    return Results.Ok(warehouse);
})
.RequireAuthorization(policy => policy.RequireRole("Admin"));

// GET all warehouses — all logged-in users
app.MapGet("/warehouses", () => warehouses)
.RequireAuthorization();

// GET single warehouse by ID
app.MapGet("/warehouses/{id}", (int id) =>
{
    var warehouse = warehouses.FirstOrDefault(w => w.Id == id);
    return warehouse is not null ? Results.Ok(warehouse) : Results.NotFound("Warehouse not found");
})
.RequireAuthorization();

// UPDATE warehouse — Admin only
app.MapPut("/warehouses/{id}", (int id, Warehouse updated) =>
{
    var index = warehouses.FindIndex(w => w.Id == id);
    if (index == -1) return Results.NotFound("Warehouse not found");

    warehouses[index] = updated;
    return Results.Ok(updated);
})
.RequireAuthorization(policy => policy.RequireRole("Admin"));

// DEACTIVATE warehouse — Admin only
app.MapPut("/warehouses/{id}/deactivate", (int id) =>
{
    var warehouse = warehouses.FirstOrDefault(w => w.Id == id);
    if (warehouse == null) return Results.NotFound("Warehouse not found");

    // Mark as inactive instead of deleting (preserve history)
    warehouse.IsActive = false;
    return Results.Ok("Warehouse deactivated");
})
.RequireAuthorization(policy => policy.RequireRole("Admin"));

// =============================================
// STOCK LEVEL APIS
// =============================================

// GET stock levels for a specific warehouse
app.MapGet("/warehouses/{warehouseId}/stock", (int warehouseId) =>
{
    // Return all stock records for this warehouse
    var stock = stockLevels.Where(s => s.WarehouseId == warehouseId).ToList();
    return Results.Ok(stock);
})
.RequireAuthorization();

// ADD or UPDATE stock level for a product in a warehouse
app.MapPost("/warehouses/stock", (StockLevel stockLevel) =>
{
    // Check if stock record already exists for this product+warehouse
    var existing = stockLevels.FirstOrDefault(s =>
        s.WarehouseId == stockLevel.WarehouseId &&
        s.ProductId == stockLevel.ProductId);

    if (existing != null)
    {
        // Update existing stock quantity
        existing.Quantity += stockLevel.Quantity;
        return Results.Ok(existing);
    }

    // Add new stock record
    stockLevels.Add(stockLevel);
    return Results.Ok(stockLevel);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// GET all low stock items — products below safe level
app.MapGet("/warehouses/stock/lowstock", () =>
{
    // Low stock = quantity is less than 10 (threshold)
    var lowStock = stockLevels.Where(s => s.Quantity < 10).ToList();
    return Results.Ok(lowStock);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// =============================================
// TRANSFER STOCK BETWEEN WAREHOUSES
// (This is the UNIQUE feature of this service)
// =============================================
app.MapPost("/warehouses/transfer", (TransferRequest request) =>
{
    // Step 1: Find stock in source warehouse
    var fromStock = stockLevels.FirstOrDefault(s =>
        s.WarehouseId == request.FromWarehouseId &&
        s.ProductId == request.ProductId);

    // Step 2: Check if source has enough stock
    if (fromStock == null || fromStock.AvailableQuantity < request.Quantity)
        return Results.BadRequest("Not enough stock in source warehouse");

    // Step 3: Deduct from source warehouse
    fromStock.Quantity -= request.Quantity;

    // Step 4: Find or create stock record in destination warehouse
    var toStock = stockLevels.FirstOrDefault(s =>
        s.WarehouseId == request.ToWarehouseId &&
        s.ProductId == request.ProductId);

    if (toStock != null)
    {
        // Add to existing stock in destination
        toStock.Quantity += request.Quantity;
    }
    else
    {
        // Create new stock record in destination
        stockLevels.Add(new StockLevel
        {
            WarehouseId = request.ToWarehouseId,
            ProductId = request.ProductId,
            Quantity = request.Quantity
        });
    }

    return Results.Ok(new
    {
        Message = "Stock transferred successfully",
        ProductId = request.ProductId,
        Quantity = request.Quantity,
        From = request.FromWarehouseId,
        To = request.ToWarehouseId
    });
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

app.Run();