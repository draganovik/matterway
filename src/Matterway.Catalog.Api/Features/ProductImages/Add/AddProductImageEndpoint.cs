using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages.Add;

public class AddProductImageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductImages", Handler)
            .WithName("AddProductImage").WithSummary("Add a new ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<AddProductImageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<AddProductImageRequest>("multipart/form-data")
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .DisableAntiforgery()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductImageResponse>, BadRequest<ProblemDetails>>> Handler(
        [FromForm] AddProductImageRequest request,
        HttpContext httpContext,
        IProductImageRepository productImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Image file is required."
            });
        }

        ImageStorageUploadResult uploadResult;
        try
        {
            uploadResult = await imageStorageService.UploadAsync(request.ProductId, request.File, cancellationToken);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Upload failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = ex.Message
            });
        }

        var entity = ProductImage.FromRequest(request, uploadResult);
        ProductImage? created;

        try
        {
            created = await productImageRepository.Create(entity);
        }
        catch
        {
            await imageStorageService.DeleteAsync(uploadResult.ImageRef, cancellationToken);
            throw;
        }

        var productImageModel = created?.ToResponse();

        if (productImageModel is null)
        {
            await imageStorageService.DeleteAsync(uploadResult.ImageRef, cancellationToken);
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = ApiResourceUriBuilder.BuildRelativePath(httpContext,
            $"ProductImages/{productImageModel.ProductId}/{productImageModel.Id}");

        return TypedResults.Created(location,
            productImageModel);
    }
}
