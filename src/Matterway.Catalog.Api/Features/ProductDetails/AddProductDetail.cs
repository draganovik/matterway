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
        app.MapPost("ProductDetails", Handle)
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
        AddProductDetailRequest request,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository,
        CancellationToken cancellationToken)
    {
        var productDetailModel = MapToEntity(request);
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
            $"ProductDetails/{created.Id}");

        return TypedResults.Created(location,
            MapToResponse(created));
    }

    public record AddProductDetailRequest
    {
        [Required]
        public Guid ProductId { get; init; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DetailType Type { get; init; }

        [Required]
        public string? Title { get; init; }

        [Required]
        public string? Value { get; init; }

        public string? Unit { get; init; }
    }

    public record AddProductDetailResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public string? ProductTitle { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DetailType Type { get; init; }

        public string? Title { get; init; }
        public string? Value { get; init; }
        public string? Unit { get; init; }
    }

    public static ProductDetail MapToEntity(AddProductDetailRequest request)
    {
        return new ProductDetail
        {
            ProductId = request.ProductId,
            Type = request.Type,
            Title = request.Title ?? string.Empty,
            Value = request.Value ?? string.Empty,
            Unit = request.Unit
        };
    }

    public static AddProductDetailResponse MapToResponse(ProductDetail entity)
    {
        return new AddProductDetailResponse
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductTitle = entity.Product?.Title,
            Type = entity.Type,
            Title = entity.Title,
            Value = entity.Value,
            Unit = entity.Unit
        };
    }
}