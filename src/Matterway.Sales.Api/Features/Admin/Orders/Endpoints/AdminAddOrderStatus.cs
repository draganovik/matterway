using System.ComponentModel.DataAnnotations;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.Admin.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderStatusEntity;

namespace Matterway.Sales.Api.Features.Admin.Orders.Endpoints;

public class AdminAddOrderStatus : IEndpoint
{
    private const string RouteName = nameof(AdminAddOrderStatus);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.Admin, "orders/{orderId:OrderId}/statuses", Handle)
            .WithName(RouteName).WithSummary("[admin] Add an Order status entry")
            .WithTags(nameof(OrderStatus))
            .Produces<AdminAddOrderStatusResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .RequireAuthorization(policy =>
                policy.RequireAssertion(context =>
                    RequestIdentity.AsOperator(context.User) || RequestIdentity.AsManager(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<AdminAddOrderStatusResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        OrderId orderId,
        AdminAddOrderStatusRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        IOrderRepository orderRepository,
        IOrderStatusRepository orderStatusRepository,
        CancellationToken cancellationToken)
    {
        if (!await orderRepository.Exists(orderId, cancellationToken))
            return TypedResults.NotFound();

        if (request.Status is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order status could not be created.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Status is required."
            });

        var status = new OrderStatus
        {
            OrderId = orderId,
            Status = request.Status.Value,
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

        return TypedResults.Created(location, ToResponse(created));
    }

    private static AdminAddOrderStatusResponse ToResponse(OrderStatus entity)
    {
        return new AdminAddOrderStatusResponse
        {
            Status = entity.Status,
            ChangedAt = entity.ChangedAt,
            Note = entity.Note
        };
    }
}
