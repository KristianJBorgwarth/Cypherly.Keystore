using Cypherly.Message.Contracts.Messages.User;
using Keystore.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Keystore.Application.Features.KeyBundle.Consumers;

public sealed class UserLogoutConsumer(
        IKeyBundleRepository keyBundleRepository,
        ILogger<UserLogoutConsumer> logger) 
        : IConsumer<UserLogoutMessage>
{
    public async Task Consume(ConsumeContext<UserLogoutMessage> context)
    {
        try
        {

        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception occured while trying to process user logout event");
            throw;
        }
    }
}
