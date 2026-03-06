using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Endpoints.System.Payments;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Endpoints.Admin.Payments;

public class AdminQueryPayments : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "payments", Handler)
            .WithName("AdminQueryPayments").WithSummary("[admin] Query Payments")
            .WithTags(nameof(Payment))
            .Produces<PaginationResponse<SystemRegisterPayment.PaymentResponse>>()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Results<Ok<PaginationResponse<SystemRegisterPayment.PaymentResponse>>, NoContent, ValidationProblem>>
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
            "AdminQueryPayments",
            null);

        var results = entities.Select(SystemRegisterPayment.MapToResponse).ToList();

        var paginationResponse = PaginationResponse<SystemRegisterPayment.PaymentResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    public sealed record QueryPaymentsParameters : PaginationRequestParameters
    {
        public OrderId? OrderId { get; init; }
    }
}