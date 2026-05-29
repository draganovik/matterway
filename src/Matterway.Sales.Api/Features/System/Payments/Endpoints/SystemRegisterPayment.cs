using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.System.Payments.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;

namespace Matterway.Sales.Api.Features.System.Payments.Endpoints;

public class SystemRegisterPayment : IEndpoint
{
    private const string RouteName = nameof(SystemRegisterPayment);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "payments", Handle)
            .WithName(RouteName).WithSummary("[system] Register a Payment")
            .WithTags(nameof(Payment))
            .Produces<SystemBasePaymentResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<SystemBasePaymentResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        SystemRegisterPaymentRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        CancellationToken cancellationToken)
    {
        if (!await orderRepository.Exists(request.OrderId, cancellationToken))
            return TypedResults.NotFound();

        var payment = ToEntity(request);

        var created = await paymentRepository.Create(payment, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Payment could not be registered.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "AdminGetPaymentById",
            new { paymentId = created.Id });

        return TypedResults.Created(location, ToResponse(created));
    }

    private static Payment ToEntity(SystemRegisterPaymentRequest request)
    {
        return new Payment
        {
            OrderId = request.OrderId,
            Provider = request.Provider,
            Amount = request.Amount,
            Status = request.Status,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };
    }

    private static SystemBasePaymentResponse ToResponse(Payment entity)
    {
        return new SystemBasePaymentResponse
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