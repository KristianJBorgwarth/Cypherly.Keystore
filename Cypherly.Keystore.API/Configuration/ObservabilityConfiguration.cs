using MassTransit.Monitoring;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;            // <-- critical
using OpenTelemetry.Trace;

namespace Cypherly.Keystore.API.Configuration;

public static class ObservabilityConfiguration
{
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(r => r
                .AddService(
                    serviceName: "cypherly.keystore.svc",
                    serviceVersion: typeof(Program).Assembly.GetName().Version?.ToString(),
                    serviceInstanceId: Environment.MachineName))

            .WithTracing(b => b
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                .AddOtlpExporter())

            .WithMetrics(b => b
                .AddRuntimeInstrumentation()
                .AddMeter(InstrumentationOptions.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter());

        return services;
    }
}