using System.Reflection;
using Cypherly.Message.Contracts.Abstractions;
using Cypherly.Message.Contracts.Messages.KeyBundle;
using Keystore.Application.Features.KeyBundle.Consumers;
using Keystore.Infrastructure.Messaging;
using Keystore.Infrastructure.Settings;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Keystore.Infrastructure.Extensions;

public static class MasstransitExtensions
{
    internal static void AddMassTransitWithRabbitMq(this IServiceCollection services)
    {
        services.ConfigureMasstransit(Assembly.Load("Keystore.Application"), (cfg, context) =>
        {
            cfg.ReceiveEndpoint("keystore.logout_queue", e =>
            {
                e.Consumer<UserLogoutConsumer>(context);
            });
        });

        services.AddProducer<KeyCountLowMessage>();
    }

    private static void ConfigureMasstransit(
            this IServiceCollection services,
            Assembly consumerAssembly,
            Action<IRabbitMqBusFactoryConfigurator, IBusRegistrationContext>? rabbitMqConfig = null)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumers(consumerAssembly);

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                cfg.Host(rabbitMqSettings.Host, "/", h =>
                        {
                            h.Username(rabbitMqSettings.Username ??
                                       throw new InvalidOperationException("Cannot initialize RabbitMQ without a username"));
                            h.Password(rabbitMqSettings.Password ??
                                       throw new InvalidOperationException("Cannot initialize RabbitMQ without a password"));
                        });

                cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));

                cfg.UseCircuitBreaker(cb =>
                {
                    cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                    cb.TripThreshold = 15;
                    cb.ActiveThreshold = 10;
                    cb.ResetInterval = TimeSpan.FromMinutes(5);
                });

                rabbitMqConfig?.Invoke(cfg, context);
                cfg.ConfigureEndpoints(context);
            });
        });
    }

    /// <summary>
    /// Add a <see cref="Producer{TMessage}"/> for a specific message type to the service collection
    /// </summary>
    /// <param name="services">the collection producer will be added to</param>
    /// <typeparam name="TMessage">the type the producer will handle. TMessage type should be of type <see cref="IBaseMessage"/></typeparam>
    /// <returns><see cref="IServiceCollection"/></returns>
    private static IServiceCollection AddProducer<TMessage>(this IServiceCollection services)
        where TMessage : IBaseMessage
    {
        services.AddScoped<IProducer<TMessage>, Producer<TMessage>>();
        return services;
    }
}
