using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Payments.Api.Features.Payments.Contracts;
using Payments.Api.Features.Payments.Data;
using Payments.Api.Features.Payments.Domain;
using Common.Infrastructure.Abstractions;

namespace Payments.Api.Features.Payments.Endpoints;

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