using Microsoft.AspNetCore.Routing;

namespace Shared.Models;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}