using System.Reflection;
using Cypherly.Keystore.Application.Behavior;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Cypherly.Keystore.Application.Configuration;

public static class ApplicationConfiguration
{
    public static void AddApplication(this IServiceCollection services, Assembly assembly)
    {
        services.AddMediatR(assembly);
    }

    private static void AddMediatR(this IServiceCollection services, Assembly assembly)
    {
        services.AddValidatorsFromAssembly(assembly);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    }
}