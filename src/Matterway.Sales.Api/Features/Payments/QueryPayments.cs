using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Providers.Persistence.PaymentEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Payments;

public class QueryPayments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Payments", Handler)
            .WithName("QueryPayments").WithSummary("Query Payments.")
            .WithTags(nameof(Payment))
            .Produces<PaginationResponse<RegisterPayment.PaymentResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async
        Task<Results<Ok<PaginationResponse<RegisterPayment.PaymentResponse>>, NoContent, ValidationProblem>>
        Handler([AsParameters] QueryPaymentsParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IPaymentRepository paymentRepository,
            CancellationToken cancellationToken)
    {
        var total = await paymentRepository.Count(queryParameters.OrderId, cancellationToken);
        if (total == 0)
            return TypedResults.NoContent();

        var entities = await paymentRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.OrderId,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            "QueryPayments",
            null);

        var results = entities.Select(RegisterPayment.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<RegisterPayment.PaymentResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryPaymentsParameters : PaginationRequestParameters
    {
        public Guid? OrderId { get; init; }
    }
}