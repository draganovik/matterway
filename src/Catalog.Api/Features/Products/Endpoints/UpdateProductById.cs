using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using AutoMapper;
using Catalog.Api.Features.Products.Contracts;
using Catalog.Api.Features.Products.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Catalog.Api.Features.Products.Endpoints;

public class UpdateProductById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("Products/{id:guid}", Handler)
            .WithName("UpdateProductById").WithSummary("Update a Product by id.")
            .WithTags("Products")
            .Produces<ProductBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<ProductBaseResponse>, NotFound, BadRequest<ProblemDetails>>> Handler(
        Guid id,
        ProductBaseRequest request,
        IProductRepository productRepository,
        IMapper mapper)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        var isValid = Validator.TryValidateObject(request, context, results, true);

        if (!isValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred."
            };
            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var updatedProduct = await productRepository.Update(id, request);
        return updatedProduct is not null
            ? TypedResults.Ok(mapper.Map<ProductBaseResponse>(updatedProduct))
            : TypedResults.NotFound();
    }
}