namespace Matterway.Identity.Api.Application;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}