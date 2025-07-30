using System.Reflection;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using Keystore.Infrastructure.Persistence.Context;
using Keystore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Keystore.Infrastructure.Extensions;

internal static class PersistenceExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration,
        Assembly assembly)
    {
        services.AddDbContext<KeystoreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("KeystoreDbConnectionString"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(assembly.FullName);
                        sqlOptions.EnableRetryOnFailure();
                    })
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, [DbLoggerCategory.Database.Command.Name],
                    LogLevel.Information);
        });

        services.AddRepositories();
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IKeyBundleRepository, KeyBundleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
    }
}