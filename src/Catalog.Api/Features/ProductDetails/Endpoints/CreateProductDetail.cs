using Asp.Versioning;
using Catalog.Api.Domain;
using Catalog.Api.Features.ProductDetails.Contracts;
using Catalog.Api.Features.ProductDetails.Mapping;
using Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Features.ProductDetails.Endpoints;

public class CreateProductDetail : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductDetails", Handler)
            .WithName("CreateProductDetail").WithSummary("Create a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<ProductDetailBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<ProductDetailBaseResponse>, BadRequest<ProblemDetails>>> Handler(
        ProductDetailBaseRequest request,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository)
    {
        var productDetailModel = ProductDetail.FromContract(request);
        var created = await productDetailRepository.Create(productDetailModel);
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
            created.ToContract<ProductDetailBaseResponse>());
    }
}