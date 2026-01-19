using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Sales.Api.Features.Payments;

public class GetPaymentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Payments/{paymentId:guid}", Handler)
            .WithName("GetPaymentById").WithSummary("Get Payment by id.")
            .WithTags(nameof(Payment))
            .Produces<RegisterPayment.PaymentResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<RegisterPayment.PaymentResponse>, NotFound>> Handler(
        Guid paymentId,
        IPaymentRepository paymentRepository,
        CancellationToken cancellationToken)
    {
        var entity = await paymentRepository.GetById(paymentId, cancellationToken);
        if (entity is null) return TypedResults.NotFound();
        return TypedResults.Ok(RegisterPayment.MapToResponse(entity));
    }
}