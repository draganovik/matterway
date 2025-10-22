using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;
using Payments.API.Features.Payments.Data;
using Payments.API.Features.Payments.Domain;
using Common.Infrastructure.Enums;
using Common.Infrastructure.Interfaces;

namespace Payments.API.Features.Payments.Endpoints;

public class DeletePayment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("Payments/{id:guid}", Handler)
            .WithName("DeletePayment").WithSummary("Delete Payment by id.")
            .WithTags(nameof(Payment))
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization(policy => policy.RequireRole(nameof(SystemUserRole.Admin)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<NoContent, NotFound>>
        Handler(Guid id, IPaymentRepository paymentRepository)
    {
        var isDeleted = await paymentRepository.Delete(id);
        return isDeleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}