using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using Catalog.API.Entities;
using Catalog.API.Models.ProductImageModels;
using Catalog.API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models;
using SharedProject.ModelTemplates;
using ProductImage = Catalog.API.Entities.ProductImage;

namespace Catalog.API.Endpoints.ProductImages;

public class QueryProductImages : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/ProductImages", Handler)
            .WithName("QueryProductImages").WithSummary("Query ProductImages.")
            .WithTags(nameof(ProductImage))
            .Produces<PaginationResponse<ProductImageBaseResponseModel>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)));
    }

    private static async
        Task<Results<Ok<PaginationResponse<ProductImageBaseResponseModel>>, NoContent, BadRequest<ProblemDetails>>>
        Handler([FromQuery] [Range(1, int.MaxValue)] int page,
            [FromQuery] [Range(1, int.MaxValue)]
            int pageSize,
            HttpContext httpContext,
            IProductImageRepository productImageRepository,
            IMapper mapper)
    {
        if (page < 1 || pageSize < 1)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid page or pageSize.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Page and pageSize must be greater than zero."
            };
            var results = new List<ValidationResult>();
            if (page < 1) results.Add(new ValidationResult("Page must be greater than zero.", new[] { nameof(page) }));
            if (pageSize < 1)
                results.Add(new ValidationResult("PageSize must be greater than zero.", new[] { nameof(pageSize) }));

            problemDetails.Extensions.Add("errors", mapper.Map<Dictionary<string, string>>(results));
            return TypedResults.BadRequest(problemDetails);
        }

        var total = await productImageRepository.GetTotalEntities();
        var entities = await productImageRepository.Query(page, pageSize);
        var baseUri =
            new Uri(
                $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.PathBase}/api/ProductImages");

        var paginationResponse = new PaginationResponse<ProductImageBaseResponseModel>(total, page, pageSize,
            mapper.Map<IEnumerable<ProductImageBaseResponseModel>>(entities).ToList(), baseUri);

        return entities is IEnumerable<ProductImage> value && value.Any()
            ? TypedResults.Ok(paginationResponse)
            : TypedResults.NoContent();
    }
}
