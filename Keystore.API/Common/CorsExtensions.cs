using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Keystore.API.Common;

internal static class CorsExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("Development", builder =>
            {
                builder
                    .AllowAnyOrigin() // or WithOrigins("*")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}