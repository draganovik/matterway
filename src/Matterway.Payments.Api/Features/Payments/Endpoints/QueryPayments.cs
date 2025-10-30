using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Common.Enums;
using Matterway.Common.Http;
using Matterway.Common.Pagination;
using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Data;
using Matterway.Payments.Api.Features.Payments.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Payments.Api.Features.Payments.Endpoints;

public class QueryPayments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Payments", Handler)
            .WithName("QueryPayments").WithSummary("Query Payments.")
            .WithTags(nameof(Payment))
            .Produces<PaginationResponse<PaymentBaseResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireRole(
                nameof(SystemUserRole.Admin),
                nameof(SystemUserRole.Manager)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<PaymentBaseResponse>>, NoContent, ValidationProblem>>
        Handler(
            [AsParameters]
            PagingQueryParams pagingQuery,
            HttpContext httpContext,
            IPaymentRepository paymentRepository,
            IMapper mapper)
    {
        var total = await paymentRepository.GetTotalEntities();
        var entities = await paymentRepository.Query(pagingQuery.Page!.Value, pagingQuery.PageSize!.Value);
        if (!entities.Any()) return TypedResults.NoContent();

        var baseUri = ApiResourceUriBuilder.BuildAbsoluteUri(httpContext, "Payments");
        var paginationResponse = new PaginationResponse<PaymentBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<PaymentBaseResponse>>(entities).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}