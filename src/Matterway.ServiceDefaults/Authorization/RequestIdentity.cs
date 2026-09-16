using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Matterway.ServiceDefaults.Authorization;

public sealed record RequestIdentityOptions
{
    public required string ServiceName { get; init; }
}

public static class RequestIdentity
{
    private static readonly AsyncLocal<RequestIdentityOptions?> CurrentOptions = new();

    internal static IDisposable PushOptions(RequestIdentityOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(options.ServiceName);

        return new RequestIdentityOptionsScope(options);
    }

    public static Guid? GetIdentifier(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;

        var subject = GetSubjectValue(principal);
        return Guid.TryParse(subject, out var identifier) ? identifier : null;
    }

    public static bool IsCustomer(ClaimsPrincipal? principal)
    {
        var roleValue = principal?.FindFirstValue(ClaimTypes.Role);
        return string.Equals(roleValue, "Customer", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsEmployee(ClaimsPrincipal? principal)
    {
        return principal is not null && PermissionClaims.IsEmployee(principal);
    }

    public static bool AsObserver(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Observer);
    }

    public static bool AsOperator(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Operator);
    }

    public static bool AsManager(ClaimsPrincipal? principal)
    {
        return HasPermission(principal, PermissionLevel.Manager);
    }

    public static bool CanManageOwnedResource(ClaimsPrincipal? principal, Guid ownerId)
    {
        var requesterId = GetIdentifier(principal);
        if (requesterId is null) return false;

        return !IsCustomer(principal) || requesterId.Value == ownerId;
    }

    private static bool HasPermission(ClaimsPrincipal? principal, PermissionLevel level)
    {
        return principal is not null &&
               PermissionClaims.HasPermission(principal, GetOptions().ServiceName, level);
    }

    private static RequestIdentityOptions GetOptions()
    {
        return CurrentOptions.Value
               ?? throw new InvalidOperationException(
                   "RequestIdentity options are unavailable for this request. Ensure UseApiFoundation() is configured.");
    }

    private static string? GetSubjectValue(ClaimsPrincipal principal)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(subject)) return subject;

        return principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
    }

    private sealed class RequestIdentityOptionsScope : IDisposable
    {
        private readonly RequestIdentityOptions? _previous;

        public RequestIdentityOptionsScope(RequestIdentityOptions current)
        {
            _previous = CurrentOptions.Value;
            CurrentOptions.Value = current;
        }

        public void Dispose()
        {
            CurrentOptions.Value = _previous;
        }
    }
}