﻿using Cypherly.Keystore.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable ConvertToUsingDeclaration

namespace Cypherly.Keystore.Infrastructure.Extensions;

public static class ApplyMigrations
{
    public static void ApplyPendingMigrations(this IServiceProvider provider)
    {
        using (var scope = provider.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<KeystoreDbContext>>();

            try
            {
                var dbContext = services.GetRequiredService<KeystoreDbContext>();

                if (dbContext.Database.GetPendingMigrations().Any())
                {
                    logger.LogInformation("Applying migrations...");
                    dbContext.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully");
                }
                else
                {
                    logger.LogInformation("No pending migrations found");
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured while attempting to apply migration to the database");
                throw;
            }
        }
    }
}