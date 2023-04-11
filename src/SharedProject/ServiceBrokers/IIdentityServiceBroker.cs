using System.Security.Claims;

namespace Shared.ServiceBrokers;

public interface IIdentityServiceBroker
{
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}
