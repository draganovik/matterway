using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Infrastructure.ImportExport;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.Admin.CatalogArchive;

public class AdminImportCatalogArchive : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "catalog/archive/import", Handle)
            .WithName("AdminImportCatalogArchive")
            .WithSummary("[admin] Import catalog data and image binaries from zip archive")
            .WithTags("CatalogArchive")
            .Produces<ImportCatalogArchiveResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<ImportCatalogArchiveRequest>("multipart/form-data")
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ImportCatalogArchiveResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromForm]
        ImportCatalogArchiveRequest request,
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
            return TypedResults.Ok(new ImportCatalogArchiveResponse
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
    }

    public record ImportCatalogArchiveRequest
    {
        [Required]
        public IFormFile? File { get; init; }
    }

    public record ImportCatalogArchiveResponse
    {
        public int DetailCount { get; init; }
        public int ArticleCount { get; init; }
        public int DiscountCount { get; init; }
        public int ArticleDetailTextCount { get; init; }
        public int ArticleDetailNumericCount { get; init; }
        public int ArticleImageCount { get; init; }
    }
}