using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.Products.Create;

public class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Products", Handler)
            .WithName("CreateProduct").WithSummary("Create a new Product.")
            .WithTags("Products")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<CreateProductResponse>, BadRequest<ProblemDetails>>> Handler(
        CreateProductRequest request,
        IProductRepository productRepository,
        HttpContext httpContext)
    {
        var product = Product.FromRequest(request);

        var created = await productRepository.Create(product);

        if (created is null)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Product could not be created.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var location = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/Products/{created.Id}";

        return TypedResults.Created(location, created.ToResponse());
    }
}