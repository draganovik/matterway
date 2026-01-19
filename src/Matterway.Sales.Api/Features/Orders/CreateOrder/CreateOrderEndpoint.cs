using System.Net;
using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Orders.CreateOrder;

public class CreateOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("Orders", Handle)
            .WithName("CreateOrder").WithSummary("Create a new Order from Customers cart items.")
            .WithTags(nameof(Order))
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .RequireAuthorization()
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult>>
        Handle(
            CreateOrderRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            CreateOrderService createOrderService,
            CancellationToken cancellationToken)
    {
        var command = BuildCommand(request, httpContext);
        var requestFailure = createOrderService.ValidateCommand(command);
        if (requestFailure is not null)
            return ToBadRequest(requestFailure);

        var orderId = Guid.CreateVersion7();
        var customersResult = await createOrderService.RequestCustomerOrderAsync(orderId, command, cancellationToken);

        if (!customersResult.IsSuccess)
            return customersResult.StatusCode switch
            {
                HttpStatusCode.Forbidden => TypedResults.Forbid(),
                HttpStatusCode.NotFound => TypedResults.NotFound(),
                _ => TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Customer order could not be created.",
                    Status = StatusCodes.Status400BadRequest
                })
            };

        var customersOrderFailure = createOrderService.ValidateCustomerOrder(orderId, command, customersResult.Order);
        if (customersOrderFailure is not null)
            return ToBadRequest(customersOrderFailure);

        var order = createOrderService.BuildOrder(command, customersResult.Order!);

        var created = await createOrderService.PersistAsync(order, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "GetOrderById",
            new { orderId = created.Id });

        return TypedResults.Created(location, CreateOrderMapper.MapToResponse(created));
    }

    private static CreateOrderCommand BuildCommand(CreateOrderRequest request, HttpContext httpContext)
    {
        return new CreateOrderCommand(
            request.CustomerId,
            request.Type,
            request.DeliveryInfo is null
                ? null
                : new CreateOrderDeliveryInfo(
                    request.DeliveryInfo.Country,
                    request.DeliveryInfo.City,
                    request.DeliveryInfo.ZipCode,
                    request.DeliveryInfo.AddressLine1,
                    request.DeliveryInfo.AddressLine2,
                    request.DeliveryInfo.ContactPhone),
            httpContext.Request.Headers.Authorization.ToString());
    }

    private static BadRequest<ProblemDetails> ToBadRequest(CreateOrderFailure failure)
    {
        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = failure.Title,
            Detail = failure.Detail,
            Status = StatusCodes.Status400BadRequest
        });
    }
}