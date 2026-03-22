using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Catalog.Api.Endpoints.Public.Images;

public class PublicGetImageById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Public, "images/{imageId:guid}", Handle)
            .WithName("PublicGetImageById")
            .WithSummary("[public] Get an article image")
            .WithTags("Images")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<FileContentHttpResult, NotFound>> Handle(
        Guid imageId,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        var image = await imageStorageService.DownloadAsync(imageId, cancellationToken);
        if (image is null) return TypedResults.NotFound();

        return TypedResults.File(image.Content, image.ContentType);
    }
}