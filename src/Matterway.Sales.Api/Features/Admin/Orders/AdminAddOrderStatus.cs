using System.ComponentModel.DataAnnotations;
using Asp.Versioning;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderStatusEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Admin.Orders;

public class AdminAddOrderStatus : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "orders/{orderId:guid}/statuses", Handle)
            .WithName("AdminAddOrderStatus").WithSummary("[admin] Add an Order status entry")
            .WithTags(nameof(OrderStatus))
            .Produces<OrderStatusResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context => RequestIdentity.AsOperator(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderStatusResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        Guid orderId,
        AddOrderStatusRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository,
        CancellationToken cancellationToken)
    {
        if (!await orderRepository.Exists(orderId, cancellationToken))
            return TypedResults.NotFound();

        var status = new OrderStatus
        {
            OrderId = orderId,
            Status = request.Status,
            Note = request.Note,
            ChangedAt = DateTime.UtcNow
        };

        var created = await orderStatusRepository.Create(status, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order status could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "AdminGetOrderById",
            new { orderId });

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record AddOrderStatusRequest
    {
        [Required]
        public EOrderStatusType Status { get; init; }

        public string? Note { get; init; }
    }

    public record OrderStatusResponse
    {
        public EOrderStatusType Status { get; init; }
        public DateTime ChangedAt { get; init; }
        public string? Note { get; init; }
    }

    private static OrderStatusResponse MapToResponse(OrderStatus entity)
    {
        return new OrderStatusResponse
        {
            Status = entity.Status,
            ChangedAt = entity.ChangedAt,
            Note = entity.Note
        };
    }
}