using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Catalog.Api.Application;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Domain.Entities;
using Matterway.Catalog.Api.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductSpecifications;

public class AddProductSpecification : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products/{productId:Guid}/Specifications", Handle)
            .WithName("AddProductSpecification").WithSummary("Add a new ProductSpecification.")
            .WithTags(nameof(ProductSpecification))
            .Produces<AddProductSpecificationResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(ERequestClaimsRole.Admin),
                nameof(ERequestClaimsRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductSpecificationResponse>, BadRequest<ProblemDetails>>> Handle(
        [FromRoute]
        Guid productId,
        AddProductSpecificationRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IProductSpecificationRepository repository,
        CancellationToken cancellationToken)
    {
        var model = MapToEntity(productId, request);
        var created = await repository.Create(model, cancellationToken);
        if (created is null)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Cannot create specification"
            };
            return TypedResults.BadRequest(problemDetails);
        }

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetProductById",
            new
            {
                id = created.ProductId
            });

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record AddProductSpecificationRequest
    {
        [Required]
        public required string SpecificationSlug { get; init; }

        [Required]
        public required decimal Value { get; init; }
    }

    public record AddProductSpecificationResponse
    {
        public Guid ProductId { get; init; }
        public string? ProductTitle { get; init; }
        public string? SpecificationSlug { get; init; }
        public string? Title { get; init; }
        public decimal Value { get; init; }
        public string? Unit { get; init; }
    }

    public static ProductSpecification MapToEntity(Guid productId, AddProductSpecificationRequest request)
    {
        return new ProductSpecification
        {
            ProductId = productId,
            SpecificationSlug = request.SpecificationSlug,
            Value = request.Value
        };
    }

    public static AddProductSpecificationResponse MapToResponse(ProductSpecification entity)
    {
        return new AddProductSpecificationResponse
        {
            ProductId = entity.ProductId,
            ProductTitle = entity.Product?.Title,
            SpecificationSlug = entity.SpecificationSlug,
            Title = entity.Specification?.Title,
            Value = entity.Value,
            Unit = entity.Specification?.Unit
        };
    }
}