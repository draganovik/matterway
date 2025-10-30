using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductDetails.Add;

public class AddProductDetailEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductDetails", Handler)
            .WithName("AddProductDetail").WithSummary("Add a new ProductDetail.")
            .WithTags(nameof(ProductDetail))
            .Produces<AddProductDetailResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductDetailResponse>, BadRequest<ProblemDetails>>> Handler(
        AddProductDetailRequest request,
        HttpContext httpContext,
        IProductDetailRepository productDetailRepository)
    {
        var productDetailModel = ProductDetail.FromRequest(request);
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
            created.ToResponse());
    }
}