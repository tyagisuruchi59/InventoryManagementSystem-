using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var key = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter Bearer token"
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

// Auth
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

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

var inventoryList = new List<Inventory>();

// ➕ Add stock
app.MapPost("/inventory", (Inventory item) =>
{
    inventoryList.Add(item);
    return Results.Ok(item);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// 📥 Get all
app.MapGet("/inventory", () => inventoryList)
.RequireAuthorization();

// 🔍 Get by product
app.MapGet("/inventory/{productId}", (int productId) =>
{
    var result = inventoryList.Where(i => i.ProductId == productId);
    return Results.Ok(result);
})
.RequireAuthorization();

// ✏️ Update stock
app.MapPut("/inventory/{id}", (int id, Inventory updated) =>
{
    var index = inventoryList.FindIndex(i => i.Id == id);
    if (index == -1) return Results.NotFound();

    inventoryList[index] = updated;
    return Results.Ok(updated);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// 🔁 Transfer stock
app.MapPost("/inventory/transfer", (int fromWarehouse, int toWarehouse, int productId, int qty) =>
{
    var fromItem = inventoryList.FirstOrDefault(i => i.ProductId == productId && i.WarehouseId == fromWarehouse);
    var toItem = inventoryList.FirstOrDefault(i => i.ProductId == productId && i.WarehouseId == toWarehouse);

    if (fromItem == null || fromItem.Quantity < qty)
        return Results.BadRequest("Not enough stock");

    // decrease from source
    var fromIndex = inventoryList.FindIndex(i => i.Id == fromItem.Id);
    inventoryList[fromIndex] = fromItem with { Quantity = fromItem.Quantity - qty };

    // increase destination
    if (toItem == null)
    {
        inventoryList.Add(new Inventory(inventoryList.Count + 1, productId, toWarehouse, qty));
    }
    else
    {
        var toIndex = inventoryList.FindIndex(i => i.Id == toItem.Id);
        inventoryList[toIndex] = toItem with { Quantity = toItem.Quantity + qty };
    }

    return Results.Ok("Transfer successful");
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

app.Run();