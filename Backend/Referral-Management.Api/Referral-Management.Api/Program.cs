using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Referral_Management.Api.Extensions;
using Referral_Management.Api.Middleware;
using Referral_Management.Api.Models;
using Referral_Management.Api.Services;
using Referral_Management.Api.Services.Implementations;
using Referral_Management.Api.Services.Interfaces;
using Serilog;
using System.Text;
using Referral_Management.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Extensions/Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7 // keep last 7 days
    )
    .CreateLogger();

builder.Host.UseSerilog();

//Dependency injection file
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
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