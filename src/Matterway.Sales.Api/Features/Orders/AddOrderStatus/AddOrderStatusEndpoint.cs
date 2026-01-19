using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Orders.AddOrderStatus;

public class AddOrderStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Orders/{orderId:guid}/Statuses", Handle)
            .WithName("AddOrderStatus").WithSummary("Add an Order status entry.")
            .WithTags(nameof(OrderStatus))
            .Produces<OrderStatusResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesValidationProblem()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderStatusResponse>, BadRequest<ProblemDetails>, NotFound>> Handle(
        Guid orderId,
        AddOrderStatusRequest request,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        AddOrderStatusService addOrderStatusService,
        CancellationToken cancellationToken)
    {
        var command = new AddOrderStatusCommand(orderId, request.Status, request.Note);

        if (!await addOrderStatusService.OrderExistsAsync(command.OrderId, cancellationToken))
            return TypedResults.NotFound();

        var status = addOrderStatusService.BuildStatus(command);

        var created = await addOrderStatusService.PersistAsync(status, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order status could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetOrderById",
            new { orderId });

        return TypedResults.Created(location, AddOrderStatusMapper.MapToResponse(created));
    }
}