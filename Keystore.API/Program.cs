using System.Reflection;
using Keystore.API.Common;
using Keystore.API.Extensions;
using Keystore.Application.Extensions;
using Keystore.Infrastructure.Extensions;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.SetupConfiguration();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddObservability(configuration);

builder.Services.AddCorsPolicy();

builder.Services.AddApplication(Assembly.Load("Keystore.Application"));

builder.Services.AddInfrastructure(configuration, Assembly.Load("Keystore.Infrastructure"));

builder.Services.AddSecurity(configuration);

builder.Services.AddEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();

app.RegisterMinimalEndpoints();

app.UseCors("Development");

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Keystore.API V1")
        .WithTheme(ScalarTheme.Purple)
        .WithDarkModeToggle(false)
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
});

if (builder.Environment.IsProduction())
{
    app.Services.ApplyPendingMigrations();
    app.MapPrometheusScrapingEndpoint();
}

app.UseSerilogRequestLogging();

try
{
    Log.Information("Keystore.API is starting up");
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

// Required for integration tests
public partial class Program;