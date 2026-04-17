// StockPro Inventory Management System
// Service: Auth Service
// Developer: Suru

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;

// In-memory storage
var users = new List<User>();

var builder = WebApplication.CreateBuilder(args);

// 🔐 Strong secret key
var key = "MyVeryStrongSecretKeyForJwtAuth1234567890Secure";

// =========================
// SERVICES
// =========================
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
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
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
// MIDDLEWARE
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// REGISTER
// =========================
app.MapPost("/register", (User user) =>
{
    if (users.Any(u => u.Username == user.Username))
        return Results.BadRequest("User already exists");

    users.Add(user);
    return Results.Ok($"User registered as {user.Role}");
});

// =========================
// LOGIN
// =========================
app.MapPost("/login", (User loginUser) =>
{
    var user = users.FirstOrDefault(u =>
        u.Username == loginUser.Username &&
        u.Password == loginUser.Password);

    if (user == null)
        return Results.Unauthorized();

    var token = GenerateJwtToken(user.Username, user.Role, key);
    return Results.Ok(new { token });
});

// =========================
// 🔐 PROTECTED API
// =========================
app.MapGet("/secure", (HttpContext context) =>
{
    var username = context.User.Identity?.Name;
    var role = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

    return $"Hello {username}, Role: {role}";
})
.RequireAuthorization();


// =========================
// 🔐 JWT TOKEN GENERATION (UPDATED)
// =========================
string GenerateJwtToken(string username, string role, string key)
{
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role),
        new Claim("userId", Guid.NewGuid().ToString())
    };

    var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(2),
        signingCredentials: credentials
    );

    return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
}
app.MapGet("/admin", () => "Welcome Admin")
   .RequireAuthorization(policy => policy.RequireRole("Admin"));
   app.MapGet("/staff", () => "Welcome Staff")
   .RequireAuthorization(policy => policy.RequireRole("Staff"));
   app.MapGet("/manager", () => "Welcome Manager")
   .RequireAuthorization(policy => policy.RequireRole("Manager"));

app.Run();