using Cypherly.Message.Contracts.Messages.User;
using Keystore.Application.Abstractions;
using Keystore.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Consumers;

public sealed class UserLogoutConsumer(
        IKeyBundleRepository keyBundleRepository,
        IUnitOfWork unitOfWork,
        ILogger<UserLogoutConsumer> logger) 
        : IConsumer<UserLogoutMessage>
{
    public async Task Consume(ConsumeContext<UserLogoutMessage> context)
    {
        try
        {
            var keyBundle = await keyBundleRepository.GetByIdAsync(context.Message.DeviceId, context.CancellationToken);
            if (keyBundle is null)
            {
                logger.LogWarning("No key bundle found for device id: {DeviceId}", context.Message.DeviceId);
                return;
            }

            await keyBundleRepository.DeleteAsync(keyBundle, context.CancellationToken);
            await unitOfWork.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while trying to process user logout event");
            throw;
        }
    }
}
