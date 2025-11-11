using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductDetail;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails;

public class AddProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products/{productId:Guid}/Details", Handle)
            .WithName("AddProductDetail").WithSummary("Add a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<AddProductDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductDetailResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid productId,
        AddProductDetailRequest request,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository,
        CancellationToken cancellationToken)
    {
        var productDetailModel = MapToEntity(productId, request);
        var created = await productDetailRepository.Create(productDetailModel, cancellationToken);
        if (created is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create entity"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = ApiResourceUriBuilder.BuildRelativePath(httpContext,
            $"Products/{created.ProductId}/Details/{created.TypeId}");

        return TypedResults.Created(location,
            MapToResponse(created));
    }

    public record AddProductDetailRequest
    {
        [Required]
        public int TypeId { get; init; }

        [Required]
        public required string Value { get; init; }
    }

    public record AddProductDetailResponse
    {
        public Guid ProductId { get; init; }
        public string? ProductTitle { get; init; }
        public int TypeId { get; init; }
        public string? Title { get; init; }
        public string? Value { get; init; }
        public string? Unit { get; init; }
    }

    public static ProductDetail MapToEntity(Guid productId, AddProductDetailRequest request)
    {
        return new ProductDetail
        {
            ProductId = productId,
            TypeId = request.TypeId,
            Value = request.Value,
        };
    }

    public static AddProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new AddProductDetailResponse
        {
            ProductId = entity.ProductId,
            ProductTitle = entity.Product?.Title,
            TypeId = entity.TypeId,
            Title = entity.Type?.Title,
            Value = entity.Value,
            Unit = entity.Type?.Unit
        };
    }
}