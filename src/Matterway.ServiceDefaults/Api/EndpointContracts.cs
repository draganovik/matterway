using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Matterway.ServiceDefaults.Api;

public enum EndpointKind
{
    Self,
    Admin,
    Public,
    System
}

public readonly record struct EndpointKindMetadata(EndpointKind Kind);

public interface IEndpoint
{
    void MapEndpoint(EndpointRouter endpoints);
}

public sealed class EndpointRouter(IReadOnlyDictionary<EndpointKind, RouteGroupBuilder> groups)
{
    private RouteGroupBuilder ResolveGroup(EndpointKind endpointKind)
    {
        if (groups.TryGetValue(endpointKind, out var group)) return group;

        throw new ArgumentOutOfRangeException(nameof(endpointKind), endpointKind, null);
    }

    private RouteHandlerBuilder Map(
        EndpointKind endpointKind,
        string pattern,
        Delegate handler,
        Func<RouteGroupBuilder, string, Delegate, RouteHandlerBuilder> map)
    {
        var builder = map(ResolveGroup(endpointKind), pattern, handler);
        builder.WithMetadata(new EndpointKindMetadata(endpointKind));
        return builder;
    }

    public RouteHandlerBuilder MapGet(EndpointKind endpointKind, string pattern, Delegate handler)
    {
        return Map(endpointKind, pattern, handler, static (group, route, action) => group.MapGet(route, action));
    }

    public RouteHandlerBuilder MapPost(EndpointKind endpointKind, string pattern, Delegate handler)
    {
        return Map(endpointKind, pattern, handler, static (group, route, action) => group.MapPost(route, action));
    }

    public RouteHandlerBuilder MapPut(EndpointKind endpointKind, string pattern, Delegate handler)
    {
        return Map(endpointKind, pattern, handler, static (group, route, action) => group.MapPut(route, action));
    }

    public RouteHandlerBuilder MapPatch(EndpointKind endpointKind, string pattern, Delegate handler)
    {
        return Map(endpointKind, pattern, handler, static (group, route, action) => group.MapPatch(route, action));
    }

    public RouteHandlerBuilder MapDelete(EndpointKind endpointKind, string pattern, Delegate handler)
    {
        return Map(endpointKind, pattern, handler, static (group, route, action) => group.MapDelete(route, action));
    }
}