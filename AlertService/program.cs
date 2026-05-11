// StockPro Inventory Management System
// Service: Alert Service | Entry Point
// Developer: Suru | April 2026

using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using AlertService.Consumers;
using AlertService.Data;
using AlertService.Repositories;
using AlertService.Services;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:80");

// DATABASE
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DEPENDENCY INJECTION
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IAlertService, AlertServiceImpl>();

// BACKGROUND SERVICE
builder.Services.AddHostedService<LowStockBackgroundService>();

// QUARTZ.NET
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("OverduePOAlertJob");
    q.AddJob<OverduePOAlertJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("OverduePOAlertJob-trigger")
        .WithCronSchedule("0 0 9 * * ?")
    );
});
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

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

// CONTROLLERS
builder.Services.AddControllers();
builder.Services.AddHttpClient();

// MASSTRANSIT - RABBITMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<LowStockEventConsumer>();
    x.AddConsumer<POApprovedEventConsumer>();
    x.AddConsumer<POSubmittedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ__Host"] ?? "rabbitmq", "/", h =>
        {
            h.Username(builder.Configuration["RabbitMQ__Username"] ?? "stockpro");
            h.Password(builder.Configuration["RabbitMQ__Password"] ?? "StockPro@2026");
        });

        cfg.ConfigureEndpoints(context);
    });
});

// SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StockPro Alert Service",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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

app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
