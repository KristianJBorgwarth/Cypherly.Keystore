using System.Reflection;
using Cypherly.Keystore.API.Configuration;
using Cypherly.Keystore.Application.Configuration;
using Cypherly.Keystore.Infrastructure.Configuration;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

#region Configuration

var env = builder.Environment;

var configuration = builder.Configuration;
configuration.AddJsonFile("appsettings.json", false, true).AddEnvironmentVariables();

if (env.IsDevelopment())
{
    configuration.AddJsonFile($"appsettings.{Environments.Development}.json", true, true);
    configuration.AddUserSecrets(Assembly.GetExecutingAssembly(), true);
}

#endregion

#region Logging

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();


builder.Host.UseSerilog();

#endregion

#region CORS

builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", builder =>
    {
        builder
            .AllowAnyOrigin() // or WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });

});

#endregion

builder.Services.AddApplication(Assembly.Load("Cypherly.Keystore.Application"));
builder.Services.AddInfrastructure(configuration, Assembly.Load("Cypherly.Keystore.Infrastructure"));

builder.Services.AddEndpoints();
builder.Services.AddOpenApi();

var app = builder.Build();

app.RegisterMinimalEndpoints();

app.UseCors("Development");

#region Scalar

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.WithTitle("Cypherly.Keystore.API V1")
            .WithTheme(ScalarTheme.Purple)
            .WithDarkModeToggle(false)
            .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
    });
}

#endregion

if (env.IsProduction())
{
    app.Services.ApplyPendingMigrations();
}

try
{
    Log.Information("Starting Cypherly.Keystore.API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}