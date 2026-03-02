using Matterway.Catalog.Api.Infrastructure.ImportExport;

namespace Matterway.Catalog.Api.Endpoints.Admin.CatalogArchive;

public class AdminExportCatalogArchive : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "catalog/archive/export", Handle)
            .WithName("AdminExportCatalogArchive")
            .WithSummary("[admin] Export catalog data and image binaries as zip archive")
            .WithTags("CatalogArchive")
            .Produces(StatusCodes.Status200OK)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<FileContentHttpResult> Handle(
        ICatalogArchiveService archiveService,
        CancellationToken cancellationToken)
    {
        var result = await archiveService.ExportAsync(cancellationToken);
        return TypedResults.File(result.Content, "application/zip", result.FileName);
    }
}