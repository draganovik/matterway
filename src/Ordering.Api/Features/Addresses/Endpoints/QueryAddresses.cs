using Asp.Versioning;
using AutoMapper;
using Common.Infrastructure.Abstractions;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Http;
using Common.Infrastructure.Pagination;
using Microsoft.AspNetCore.Http.HttpResults;
using Ordering.Api.Features.Addresses.Contracts;
using Ordering.Api.Features.Addresses.Data;
using Ordering.Api.Features.Addresses.Domain;

namespace Ordering.Api.Features.Addresses.Endpoints;

public class QueryAddresses : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Addresses", Handler)
            .WithName("QueryAddresses").WithSummary("Query Addresses.")
            .WithTags(nameof(Address))
            .Produces<PaginationResponse<AddressBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<AddressBaseResponse>>, NoContent, ValidationProblem>>
        Handler(
            [AsParameters]
            PagingQueryParams pagingQuery,
            HttpContext httpContext,
            IAddressRepository addressRepository,
            IMapper mapper)
    {
        var total = await addressRepository.GetTotalEntities();
        var entities =
            await addressRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);

        if (!entities.Any()) return TypedResults.NoContent();

        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "Addresses");
        var paginationResponse = new PaginationResponse<AddressBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<AddressBaseResponse>>(entities).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}