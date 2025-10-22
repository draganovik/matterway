using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Payments.API.Features.Payments.Contracts;
using Payments.API.Features.Payments.Data;
using Payments.API.Features.Payments.Domain;
using Payments.API.Features.Shared;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;
using Common.Infrastructure.ModelTemplates;

namespace Payments.API.Features.Payments.Endpoints;

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

        var baseUri = ResourceUrlHelper.CreateBaseUri(httpContext, "Payments");
        var paginationResponse = new PaginationResponse<PaymentBaseResponse>(total, pagingQuery.Page!.Value,
            pagingQuery.PageSize!.Value,
            mapper.Map<IEnumerable<PaymentBaseResponse>>(entities).ToList(), baseUri);

        return TypedResults.Ok(paginationResponse);
    }
}