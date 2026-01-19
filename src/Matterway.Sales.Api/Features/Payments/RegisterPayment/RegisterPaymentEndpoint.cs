using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Payments.RegisterPayment;

public class RegisterPaymentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Payments", Handle)
            .WithName("RegisterPayment").WithSummary("Register a Payment.")
            .WithTags(nameof(Payment))
            .Produces<PaymentResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<PaymentResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        RegisterPaymentRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        RegisterPaymentService registerPaymentService,
        CancellationToken cancellationToken)
    {
        var command = new RegisterPaymentCommand(
            request.OrderId,
            request.Provider,
            request.ReferenceId,
            request.Amount,
            request.Status,
            request.CreatedAt);

        if (!await registerPaymentService.OrderExistsAsync(command.OrderId, cancellationToken))
            return TypedResults.NotFound();

        var payment = registerPaymentService.BuildPayment(command);

        var created = await registerPaymentService.PersistAsync(payment, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Payment could not be registered.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetPaymentById",
            new { paymentId = created.Id });

        return TypedResults.Created(location, RegisterPaymentMapper.MapToResponse(created));
    }
}