using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 🔐 Secret key
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

// =========================
// MIDDLEWARE
// =========================
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// =========================
// IN-MEMORY DB
// =========================
var users = new List<User>();

// =========================
// REGISTER
// =========================
app.MapPost("/register", (User user) =>
{
    if (string.IsNullOrEmpty(user.Role))
        return Results.BadRequest("Role is required");

    users.Add(user);
    return Results.Ok("User registered");
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

    // ✅ CORRECT (use stored role)
    var token = GenerateJwtToken(user.Username, user.Role, key);

    return Results.Ok(new { token });
});

// =========================
// PROTECTED TEST
// =========================
app.MapGet("/secure", () => "Secure API working")
.RequireAuthorization();

// =========================
// ADMIN ONLY
// =========================
app.MapGet("/admin", () => "Admin only")
.RequireAuthorization(policy => policy.RequireRole("Admin"));

// =========================
// JWT GENERATION
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

app.Run();