using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.System.Payments;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Admin.Payments;

public class AdminGetPaymentById : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapGet(EndpointKind.Admin, "payments/{paymentId:guid}", Handler)
            .WithName("AdminGetPaymentById").WithSummary("[admin] Get Payment by id")
            .WithTags(nameof(Payment))
            .Produces<SystemRegisterPayment.PaymentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsObserver(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
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