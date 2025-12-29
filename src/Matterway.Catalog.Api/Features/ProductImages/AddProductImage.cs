using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImageEntity;
using Matterway.Catalog.Api.Infrastructure.Storage;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages;

public class AddProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products/{productId:guid}/Images", Handle)
            .WithName("AddProductImage").WithSummary("Add a new ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<AddProductImageResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Accepts<AddProductImageRequest>("multipart/form-data")
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductImageResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid productId,
        [FromForm]
        AddProductImageRequest request,
        HttpContext httpContext,
        IProductImageRepository productImageRepository,
        IImageStorageService imageStorageService,
        CancellationToken cancellationToken)
    {
        if (request.File is null || request.File.Length == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Invalid request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Image file is required."
            });

        var imageId = Guid.CreateVersion7();
        ImageStorageUploadResult uploadResult;
        try
        {
            uploadResult =
                await imageStorageService.UploadAsync(productId, imageId, request.File, cancellationToken);
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

        var entity = MapToEntity(productId, request, uploadResult);
        ProductImage? created;

        try
        {
            created = await productImageRepository.Create(entity, cancellationToken);
        }
        catch
        {
            await imageStorageService.DeleteAsync(productId, imageId, cancellationToken);
            throw;
        }

        var productImageModel = MapToResponse(created);

        if (productImageModel is null)
        {
            await imageStorageService.DeleteAsync(productId, imageId, cancellationToken);
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        return TypedResults.Created(productImageModel.ImageUrl,
            productImageModel);
    }

    public record AddProductImageRequest
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int OrderIndex { get; init; }

        [Required]
        public IFormFile? File { get; init; }

        public string? ImageAlt { get; init; }
    }

    public record AddProductImageResponse
    {
        public Guid Id { get; init; }
        public int OrderIndex { get; init; }
        public Guid ProductId { get; init; }
        public string? ProductName { get; init; }
        public string? ImageUrl { get; init; }
        public string? ImageAlt { get; init; }
    }

    public static ProductImage MapToEntity(Guid productId, AddProductImageRequest request,
        ImageStorageUploadResult uploadResult)
    {
        return new ProductImage
        {
            Id = uploadResult.ImageId,
            ProductId = productId,
            OrderIndex = request.OrderIndex,
            ImageUrl = uploadResult.ImageUrl,
            ImageAlt = request.ImageAlt ?? string.Empty
        };
    }

    public static AddProductImageResponse? MapToResponse(ProductImage? entity)
    {
        return entity is null
            ? null
            : new AddProductImageResponse
            {
                Id = entity.Id,
                OrderIndex = entity.OrderIndex,
                ProductId = entity.ProductId,
                ProductName = entity.Product?.Title,
                ImageUrl = entity.ImageUrl,
                ImageAlt = entity.ImageAlt
            };
    }
}