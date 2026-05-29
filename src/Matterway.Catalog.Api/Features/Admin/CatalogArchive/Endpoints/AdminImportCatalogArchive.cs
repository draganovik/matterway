using System.Text.Json;
using Matterway.Catalog.Api.Features.Admin.CatalogArchive.Contracts;
using Matterway.Catalog.Api.Infrastructure.ImportExport;

namespace Matterway.Catalog.Api.Features.Admin.CatalogArchive.Endpoints;

public class AdminImportCatalogArchive : IEndpoint
{
    private const string RouteName = nameof(AdminImportCatalogArchive);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "catalog/archive/import", Handle)
            .WithName(RouteName)
            .WithSummary("[admin] Import catalog data and image binaries from zip archive")
            .WithTags("CatalogArchive")
            .Produces<AdminImportCatalogArchiveResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<AdminImportCatalogArchiveRequest>("multipart/form-data")
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminImportCatalogArchiveResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromForm]
        AdminImportCatalogArchiveRequest request,
        ICatalogArchiveService archiveService,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Archive file is required."
            });

        if (!request.File.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid archive",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Only .zip archives are supported."
            });

        try
        {
            await using var archiveStream = request.File.OpenReadStream();
            var result = await archiveService.ImportAsync(archiveStream, cancellationToken);
            return TypedResults.Ok(new AdminImportCatalogArchiveResponse
            {
                DetailCount = result.DetailCount,
                ArticleCount = result.ArticleCount,
                DiscountCount = result.DiscountCount,
                ArticleDetailTextCount = result.ArticleDetailTextCount,
                ArticleDetailNumericCount = result.ArticleDetailNumericCount,
                ArticleImageCount = result.ArticleImageCount
            });
        }
        catch (InvalidDataException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid archive content",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
        catch (JsonException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid archive content",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }
    }
}