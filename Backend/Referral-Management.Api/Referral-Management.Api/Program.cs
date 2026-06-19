using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc; // ✅ added
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Referral_Management.Api.DTOs.Common; // ✅ added
using Referral_Management.Api.Middleware;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services;
using Referral_Management.Api.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// Controllers + Validation Handling ✅ (added)
// ----------------------------------------------------

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Values.SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage);

        return new BadRequestObjectResult(new ApiErrorResponseDTO
        {
            Success = false,
            Message = string.Join(", ", errors),
            StatusCode = 400,
            Timestamp = DateTime.UtcNow
        });
    };
});

// ----------------------------------------------------
// Swagger
// ----------------------------------------------------

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter: Bearer {your JWT token}"
        });

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
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
                Array.Empty<string>()
            }
        });
});

// ----------------------------------------------------
// Database
// ----------------------------------------------------

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------
// Dependency Injection ✅ (updated)
// ----------------------------------------------------

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();

// ✅ ADD THIS FOR ADMIN
builder.Services.AddScoped<AdminService>();

// ----------------------------------------------------
// JWT Authentication
// ----------------------------------------------------

var jwtKey = builder.Configuration["Jwt:Key"];

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey!))
            };
    });

builder.Services.AddAuthorization();

// ----------------------------------------------------
// CORS
// ----------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("VuePolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------------------------------------
// Build App
// ----------------------------------------------------

var app = builder.Build();

// ----------------------------------------------------
// Middleware
// ----------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("VuePolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
