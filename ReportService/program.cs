// StockPro Inventory Management System
// Service: Report Service | Entry Point
// Developer: Suru | April 2026
// Description: Configures database, JWT, background snapshot service

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ReportService.Data;
using ReportService.Repositories;
using ReportService.Services;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DEPENDENCY INJECTION
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportServiceImpl>();

// BACKGROUND SERVICE - takes daily snapshot automatically
builder.Services.AddHostedService<SnapshotBackgroundService>();

// QUARTZ.NET - SCHEDULED JOBS
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailySnapshotJob");
    q.AddJob<DailySnapshotJob>(opts => opts.WithIdentity(jobKey));
    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("DailySnapshotJob-trigger")
        .WithCronSchedule("0 0 0 * * ?") // runs every day at midnight
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:3000"||"https://inventory-management-system-git-e0c2d7-tyagisuruchi59s-projects.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// CONTROLLERS
builder.Services.AddControllers();

// SWAGGER WITH JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockPro Report Service",
        Version = "v1",
        Description = "Inventory Analytics and Reporting for StockPro"
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

// JWT
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

// AUTO CREATE TABLES
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// MIDDLEWARE
app.UseCors("AllowReact");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
