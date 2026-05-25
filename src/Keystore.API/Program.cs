using System.Reflection;
using Keystore.API.Common;
using Keystore.API.Extensions;
using Keystore.Application.Extensions;
using Keystore.Infrastructure.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.SetupConfiguration();

builder.AddLogging();
builder.Services.AddObservability();

builder.Services.AddCorsPolicy();

builder.Services.AddApplication(Assembly.Load("Keystore.Application"));

builder.Services.AddInfrastructure(configuration, Assembly.Load("Keystore.Infrastructure"));

builder.Services.AddAuthentication(configuration);

builder.Services.AddEndpoints();

builder.Services.AddOpenApi();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.RegisterMinimalEndpoints();

app.UseCors("Development");

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options.WithTitle("Keystore.API V1")
        .WithTheme(ScalarTheme.Purple)
        .HideDarkModeToggle()
        .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Axios);
});

if (builder.Environment.IsProduction())
{
    app.Services.ApplyPendingMigrations();
}

try
{
    logger.LogInformation("Keystore.API is starting up");
    app.Run();
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Application start-up failed");
    throw;
}

// Required for integration tests
public partial class Program;
