using System.Security.Claims;

namespace Common.Infrastructure.Services.Brokers;

public interface IIdentityServiceBroker
{
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}
