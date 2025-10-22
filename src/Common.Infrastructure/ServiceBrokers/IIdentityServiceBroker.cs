using System.Security.Claims;

namespace Common.Infrastructure.ServiceBrokers;

public interface IIdentityServiceBroker
{
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}
