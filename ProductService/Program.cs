using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔐 SAME key as Auth Service
var key = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";

// =========================
// 🔧 SERVICES
// =========================
builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer YOUR_TOKEN'"
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

// 🔐 Authentication
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

// 🔐 Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// =========================
// 🔧 MIDDLEWARE
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// 🧠 In-memory DB
// =========================
var products = new List<Product>();

// =======================
// ➕ CREATE PRODUCT
// =======================
app.MapPost("/products", (Product product) =>
{
    products.Add(product);
    return Results.Ok(product);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// =======================
// 📥 GET ALL PRODUCTS
// =======================
app.MapGet("/products", () => products)
.RequireAuthorization();

// =======================
// 🔍 GET PRODUCT BY ID
// =======================
app.MapGet("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.RequireAuthorization();

// =======================
// ✏️ UPDATE PRODUCT
// =======================
app.MapPut("/products/{id}", (int id, Product updatedProduct) =>
{
    var index = products.FindIndex(p => p.Id == id);
    if (index == -1) return Results.NotFound();

    products[index] = updatedProduct;
    return Results.Ok(updatedProduct);
})
.RequireAuthorization(policy => policy.RequireRole("Admin", "Manager"));

// =======================
// ❌ DELETE PRODUCT
// =======================
app.MapDelete("/products/{id}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null) return Results.NotFound();

    products.Remove(product);
    return Results.Ok("Deleted");
})
.RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run();