using System.Reflection;
using Keystore.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Keystore.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.ConfigureSettings(configuration);
        services.AddPersistence(configuration, assembly);
        services.AddMassTransitWithRabbitMq(Assembly.GetExecutingAssembly());
        return services;
    }

    private static void ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(configuration.GetSection("RabbitMq"));
    }
}