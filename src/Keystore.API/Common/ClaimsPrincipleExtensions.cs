using System.Security.Claims;

namespace Keystore.API.Common;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId?.Value, out var parsedGuid)
            ? parsedGuid
            : throw new UnauthorizedAccessException("User ID not found or invalid.");
    }

    public static Guid GetDeviceId(this ClaimsPrincipal principal)
    {
        var deviceId = principal.FindFirst("device_id");
        return Guid.TryParse(deviceId?.Value, out var parsedGuid)
            ? parsedGuid
            : throw new UnauthorizedAccessException("Device ID not found or invalid.");
    }

    public static Guid GetConnectionId(this ClaimsPrincipal principal)
    {
        var connectionId = principal.FindFirst("connection_id");
        return Guid.TryParse(connectionId?.Value, out var parsedGuid)
            ? parsedGuid
            : throw new UnauthorizedAccessException("Connection ID not found or invalid.");
    }
}
