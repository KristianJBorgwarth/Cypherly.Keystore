using System.Reflection;
using Cypherly.Keystore.Application.Abstractions;
using Cypherly.Keystore.Application.Contracts;
using Cypherly.Keystore.Infrastructure.Persistence.Context;
using Cypherly.Keystore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Keystore.Infrastructure.Extensions;

internal static class PersistenceExtensions
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

        services.AddRepositories();

        return services;
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IKeyBundleRepository, KeyBundleRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
    }
}