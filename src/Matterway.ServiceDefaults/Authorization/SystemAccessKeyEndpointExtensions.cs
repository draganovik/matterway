using Microsoft.AspNetCore.Builder;

namespace Matterway.ServiceDefaults.Authorization;

public static class SystemAccessKeyEndpointExtensions
{
    private static readonly object Requirement = new();

    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder RequireSystemAccessKey()
        {
            ArgumentNullException.ThrowIfNull(builder);

            builder.WithMetadata(Requirement);
            return builder;
        }
    }

    internal static bool IsSystemAccessKeyRequired(IEnumerable<object>? metadata)
    {
        return metadata?.Any(static item => ReferenceEquals(item, Requirement)) is true;
    }
}