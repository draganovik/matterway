using Asp.Versioning;
using Matterway.Catalog.Api.Features.ProductImages.Mapping;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Features.ProductImages.Contracts;
using Matterway.Catalog.Api.Infrastructure.Abstractions;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Catalog.Api.Features.ProductImages.Endpoints;

public class CreateProductImage : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("ProductImages", Handler)
            .WithName("CreateProductImage").WithSummary("Create a new ProductImage.")
            .WithTags(nameof(ProductImage))
            .Produces<ProductImageBaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<ProductImageBaseResponse>, BadRequest<ProblemDetails>>> Handler(
        ProductImageBaseRequest request,
        HttpContext httpContext, IProductImageRepository productImageRepository)
    {
        var productImageModel = await ExecuteAsync(request, productImageRepository);
        if (productImageModel is null)
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
            $"ProductImages/{productImageModel.ProductId}/{productImageModel.Id}");

        return TypedResults.Created(location,
            productImageModel);
    }

    private static async Task<ProductImageBaseResponse?> ExecuteAsync(
        ProductImageBaseRequest request,
        IProductImageRepository productImageRepository)
    {
        var productImageModel = ProductImage.FromContract(request);
        var created = await productImageRepository.Create(productImageModel);
        return created?.ToContract<ProductImageBaseResponse>();
    }
}