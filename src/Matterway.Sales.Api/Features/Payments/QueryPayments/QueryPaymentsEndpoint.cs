using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Payments.RegisterPayment;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Payments.QueryPayments;

public class QueryPaymentsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Payments", Handler)
            .WithName("QueryPayments").WithSummary("Query Payments.")
            .WithTags(nameof(Payment))
            .Produces<PaginationResponse<PaymentResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaginationResponse<PaymentResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] QueryPaymentsParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            QueryPaymentsService queryPaymentsService,
            CancellationToken cancellationToken)
    {
        var command = new QueryPaymentsCommand(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.OrderId);

        var total = await queryPaymentsService.CountAsync(command, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await queryPaymentsService.QueryAsync(command, cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "QueryPayments",
            null);

        var results = entities.Select(RegisterPaymentMapper.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<PaymentResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }
}