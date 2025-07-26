using System.Reflection;
using FluentValidation;
using Keystore.Application.Behavior;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Keystore.Application.Extensions;

public static class ApplicationExtensions
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