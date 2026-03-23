using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Endpoints.System.Payments;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Endpoints.Admin.Payments;

public class AdminGetPaymentById : IEndpoint
{
    private const string RouteName = nameof(AdminGetPaymentById);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "payments/{paymentId:guid}", Handler)
            .WithName(RouteName).WithSummary("[admin] Get Payment by id")
            .WithTags(nameof(Payment))
            .Produces<SystemRegisterPayment.PaymentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsObserver(context.User) || RequestIdentity.AsOperator(context.User) ||
                    RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Ok<SystemRegisterPayment.PaymentResponse>, NotFound>> Handler(
        Guid paymentId,
        IPaymentRepository paymentRepository,
        CancellationToken cancellationToken)
    {
        var entity = await paymentRepository.GetById(paymentId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(SystemRegisterPayment.MapToResponse(entity));
    }
}