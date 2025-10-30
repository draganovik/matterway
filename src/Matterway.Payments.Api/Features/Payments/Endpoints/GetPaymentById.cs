using Asp.Versioning;
using AutoMapper;
using Matterway.Common.Abstractions;
using Matterway.Payments.Api.Features.Payments.Contracts;
using Matterway.Payments.Api.Features.Payments.Data;
using Matterway.Payments.Api.Features.Payments.Domain;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Matterway.Payments.Api.Features.Payments.Endpoints;

public class GetPaymentById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("Payments/{id:guid}", Handler)
            .WithName("GetPaymentById").WithSummary("Get Payment by id.")
            .WithTags(nameof(Payment))
            .Produces<PaymentBaseResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Ok<PaymentBaseResponse>, NotFound>>
        Handler(Guid id, IPaymentRepository paymentRepository, IMapper mapper)
    {
        var entity = await paymentRepository.GetById(id);
        return entity is not null
            ? TypedResults.Ok(mapper.Map<PaymentBaseResponse>(entity))
            : TypedResults.NotFound();
    }
}