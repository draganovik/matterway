using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.PaymentEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Payments;

public class RegisterPayment : IEndpoint
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
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<PaymentResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        RegisterPaymentRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        CancellationToken cancellationToken)
    {
        if (!await orderRepository.Exists(request.OrderId, cancellationToken))
            return TypedResults.NotFound();

        var payment = MapToEntity(request);

        var created = await paymentRepository.Create(payment, cancellationToken);
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

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record RegisterPaymentRequest
    {
        [Required]
        public Guid OrderId { get; init; }

        [Required]
        public required string Provider { get; init; }

        [Required]
        [RegularExpression(@"^[0-9]{4}-[0-9]{4}-[0-9]{4}-[0-9]{4}$",
            ErrorMessage = "Invalid ReferenceId. ReferenceId format must be: 0000-0000-0000-0000")]
        public required string ReferenceId { get; init; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; init; }

        public EPaymentStatus Status { get; init; } = EPaymentStatus.Reserved;

        public DateTime? CreatedAt { get; init; }
    }

    public record PaymentResponse
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public string? Provider { get; init; }
        public string? ReferenceId { get; init; }
        public decimal Amount { get; init; }
        public EPaymentStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    private static Payment MapToEntity(RegisterPaymentRequest request)
    {
        return new Payment
        {
            OrderId = request.OrderId,
            Provider = request.Provider,
            ReferenceId = request.ReferenceId,
            Amount = request.Amount,
            Status = request.Status,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow
        };
    }

    public static PaymentResponse MapToResponse(Payment entity)
    {
        return new PaymentResponse
        {
            Id = entity.Id,
            OrderId = entity.OrderId,
            Provider = entity.Provider,
            ReferenceId = entity.ReferenceId,
            Amount = entity.Amount,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt
        };
    }
}