using Asp.Versioning;
using Matterway.Catalog.Api.Domain;
using Matterway.Catalog.Api.Infrastructure.Persistence.ProductImage;
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
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<AddProductImageResponse>, BadRequest<ProblemDetails>>> Handler(
        AddProductImageRequest request,
        HttpContext httpContext, IProductImageRepository productImageRepository)
    {
        var entity = ProductImage.FromRequest(request);
        var created = await productImageRepository.Create(entity);
        var productImageModel = created?.ToResponse();

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
}