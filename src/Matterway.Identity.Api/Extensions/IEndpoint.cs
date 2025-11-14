namespace Matterway.Catalog.Api.Application;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}