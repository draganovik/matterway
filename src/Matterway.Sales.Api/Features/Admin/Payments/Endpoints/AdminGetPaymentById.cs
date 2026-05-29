using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Admin.Payments.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.Admin.Payments.Endpoints;

public class AdminGetPaymentById : IEndpoint
{
    private const string RouteName = nameof(AdminGetPaymentById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "payments/{paymentId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Payment by id")
            .WithTags(nameof(Payment))
            .Produces<AdminBasePaymentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<AdminBasePaymentResponse>, NotFound>> Handler(
        Guid paymentId,
        IPaymentRepository paymentRepository,
        CancellationToken cancellationToken)
    {
        var entity = await paymentRepository.GetById(paymentId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(ToResponse(entity));
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