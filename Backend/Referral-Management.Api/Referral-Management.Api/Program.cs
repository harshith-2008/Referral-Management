using Referral_Management.Api.Extensions;
using Serilog;
using Referral_Management.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

// ✅ SERILOG
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: "Extensions/Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7
    )
    .CreateLogger();

builder.Host.UseSerilog();

// ✅ SERVICES
builder.Services.AddApplicationServices(builder.Configuration);

// ✅ SIGNALR
builder.Services.AddSignalR();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("VuePolicy", policy =>
        policy
            .SetIsOriginAllowed(_ => true)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});


var app = builder.Build();

// ✅ SWAGGER
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ❌ KEEP THIS DISABLED FOR SIGNALR DEBUG
// app.UseMiddleware<ExceptionMiddleware>();

// ✅ HTTPS
app.UseHttpsRedirection();

// ✅ ✅ ✅ CRITICAL FIX → STATIC FILES MUST COME EARLY
app.UseStaticFiles();   // ⭐ THIS FIXES YOUR 404

// ✅ CORS
app.UseCors("VuePolicy");

// ✅ AUTH
app.UseAuthentication();
app.UseAuthorization();

// ✅ CONTROLLERS
app.MapControllers();

// ✅ SIGNALR HUB
app.MapHub<NotificationHub>("/hubs/notification");

// ✅ RUN
app.Run();