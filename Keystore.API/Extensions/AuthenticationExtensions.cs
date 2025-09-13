using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Keystore.API.Extensions;

internal static class AuthenticationExtensions
{
    internal static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("Jwt:Issuer").Value ?? throw new NullReferenceException("Issuer cannot be null"),
                    ValidAudience = configuration.GetSection("Jwt:Audience").Value ?? throw new NullReferenceException("Audience cannot be null"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                        configuration.GetSection("Jwt:Secret").Value ??
                        throw new NullReferenceException("Secret cannot be null")))
                };
            });
        
        services.AddAuthorization();
    }
}