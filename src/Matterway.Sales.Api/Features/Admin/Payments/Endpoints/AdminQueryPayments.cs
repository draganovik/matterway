using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Admin.Payments.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.Admin.Payments.Endpoints;

public class AdminQueryPayments : IEndpoint
{
    private const string RouteName = nameof(AdminQueryPayments);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "payments", Handler)
            .WithName(RouteName).WithSummary("[admin] Query Payments")
            .WithTags(nameof(Payment))
            .Produces<PaginationResponse<AdminBasePaymentResponse>>()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async
        Task<Ok<PaginationResponse<AdminBasePaymentResponse>>>
        Handler([AsParameters] AdminQueryPaymentParameters queryParameters,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            IPaymentRepository paymentRepository,
            CancellationToken cancellationToken)
    {
        var total = await paymentRepository.Count(queryParameters.OrderId, cancellationToken);
        var entities = await paymentRepository.Query(
            queryParameters.Page,
            queryParameters.PageSize,
            queryParameters.OrderId,
            cancellationToken);

        var location = linkGenerator.GetUriByName(
            httpContext,
            RouteName,
            null);

        var results = entities.Select(ToResponse).ToList();

        var paginationResponse = PaginationResponse<AdminBasePaymentResponse>.Create(
            results,
            total,
            queryParameters.Page,
            queryParameters.PageSize,
            location);

        return TypedResults.Ok(paginationResponse);
    }

    private static AdminBasePaymentResponse ToResponse(Payment entity)
    {
        return new AdminBasePaymentResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Provider = entity.Provider,
            Amount = entity.Amount,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };
    }
}
