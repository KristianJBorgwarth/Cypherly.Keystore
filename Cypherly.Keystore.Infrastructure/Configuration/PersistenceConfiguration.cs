using System.Reflection;
using Cypherly.Keystore.Application.Abstractions;
using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Infrastructure.Persistence.Context;
using Cypherly.Keystore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Keystore.Infrastructure.Configuration;

internal static class PersistenceConfiguration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddDbContext<KeystoreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("KeystoreDbConnectionString"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(assembly.FullName);
                    sqlOptions.EnableRetryOnFailure();
                });
        });

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IKeyBundleRepository, KeyBundleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
    }
}