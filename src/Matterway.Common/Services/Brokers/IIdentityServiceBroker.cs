using System.Security.Claims;

namespace Matterway.Common.Services.Brokers;

public interface IIdentityServiceBroker
{
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
}