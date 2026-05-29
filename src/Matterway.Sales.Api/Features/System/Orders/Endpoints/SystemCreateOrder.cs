using System.Net;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Features.System.Orders.Contracts;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.System.Orders.Endpoints;

public class SystemCreateOrder : IEndpoint
{
    private const string RouteName = nameof(SystemCreateOrder);
    private const string DefaultCountry = "Serbia";

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "orders", Handler)
            .WithName(RouteName)
            .WithSummary("[system] Create customer Order from cart items.")
            .WithTags(nameof(Order))
            .Produces<SystemCreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status502BadGateway)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireSystemAccessKey()
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<
            Results<Created<SystemCreateOrderResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult,
                ProblemHttpResult>>
        Handler(
            SystemCreateOrderRequest request,
            HttpContext httpContext,
            LinkGenerator linkGenerator,
            ICustomersClient customersClient,
            IOrderRepository orderRepository,
            CancellationToken cancellationToken)
    {
        var customerId = RequestIdentity.GetIdentifier(httpContext.User);
        if (customerId is null) return TypedResults.Forbid();
        var effectiveCustomerId = customerId.Value;

        var pendingOrder = await orderRepository.GetPendingByCustomerId(effectiveCustomerId, cancellationToken);
        if (pendingOrder is null)
        {
            var pending = new Order
            {
                Id = OrderId.New(),
                CustomerId = effectiveCustomerId,
                Type = request.Type.GetValueOrDefault(EOrderType.Ecommerce),
                PlacedAt = DateTime.UtcNow
            };
            pending.StatusHistory.Add(new OrderStatus
            {
                OrderId = pending.Id,
                Status = EOrderStatusType.Processing,
                ChangedAt = pending.PlacedAt
            });

            pendingOrder = await orderRepository.Create(pending, cancellationToken);
            if (pendingOrder is null)
                return TypedResults.Problem(new ProblemDetails
                {
                    Title = "Order could not be created.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "Unable to reserve an order before checkout."
                });
        }

        var orderId = pendingOrder.Id;
        var customersRequest = ToCustomersRequest(effectiveCustomerId, orderId, request);
        var customersResult = await customersClient.CreateOrderAsync(
            customersRequest,
            cancellationToken);

        if (!customersResult.IsSuccess || customersResult.Data is null)
        {
            var statusCode = customersResult.StatusCode ?? HttpStatusCode.BadGateway;
            var detail = customersResult.ErrorMessage ?? "Customer order could not be created.";

            return statusCode switch
            {
                HttpStatusCode.Forbidden => TypedResults.Forbid(),
                HttpStatusCode.NotFound => TypedResults.NotFound(),
                HttpStatusCode.BadRequest => TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Customer order could not be created.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = detail
                }),
                HttpStatusCode.ServiceUnavailable => TypedResults.Problem(new ProblemDetails
                {
                    Title = "Customer order could not be created.",
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Detail = detail
                }),
                _ => TypedResults.Problem(new ProblemDetails
                {
                    Title = "Customer order could not be created.",
                    Status = StatusCodes.Status502BadGateway,
                    Detail = detail
                })
            };
        }

        var customersOrder = customersResult.Data;
        if (customersOrder.OrderId != orderId)
            return BadRequestProblem(
                "Customer order id does not match the Sales order id.",
                "Customer order mismatch.");

        if (customersOrder.CustomerId != effectiveCustomerId)
            return BadRequestProblem(
                "Customer id does not match the Sales order request.",
                "Customer order mismatch.");

        if (customersOrder.Items.Count == 0)
            return BadRequestProblem("Order items are missing.");

        if (customersOrder.Items.Any(item => item.UnitPrice is null))
            return BadRequestProblem("Order items must include unit prices.");

        if (customersOrder.Items.Any(item => string.IsNullOrWhiteSpace(item.ArticleName)))
            return BadRequestProblem("Order items must include article titles.");

        var order = ToEntity(request, customersOrder, effectiveCustomerId, pendingOrder.PlacedAt);

        var created = await orderRepository.Update(order, cancellationToken);
        if (created is null)
            return TypedResults.Problem(new ProblemDetails
            {
                Title = "Order could not be updated.",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The local order could not be finalized."
            });

        var location = linkGenerator.GetUriByName(
            httpContext,
            "SelfGetOrderById",
            new { orderId = created.Id });

        return TypedResults.Created(location, ToResponse(created));
    }

    private static BadRequest<ProblemDetails> BadRequestProblem(
        string detail,
        string title = "Bad Request")
    {
        return TypedResults.BadRequest(new ProblemDetails
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Detail = detail
        });
    }

    private static CustomersCreateOrderRequest ToCustomersRequest(
        Guid customerId,
        OrderId orderId,
        SystemCreateOrderRequest request)
    {
        return new CustomersCreateOrderRequest
        {
            CustomerId = customerId,
            OrderId = orderId,
            DeliveryInfo = request.DeliveryInfo is null
                ? null
                : new CustomersDeliveryInfoRequest
                {
                    Country = DefaultCountry,
                    City = request.DeliveryInfo.City,
                    ZipCode = request.DeliveryInfo.ZipCode,
                    AddressLine1 = request.DeliveryInfo.AddressLine1,
                    AddressLine2 = request.DeliveryInfo.AddressLine2,
                    ContactPhone = request.DeliveryInfo.ContactPhone
                }
        };
    }

    private static Order ToEntity(
        SystemCreateOrderRequest request,
        CustomersOrderResponse customerOrder,
        Guid customerId,
        DateTime placedAt)
    {
        var order = new Order
        {
            Id = customerOrder.OrderId,
            CustomerId = customerId,
            Type = request.Type.GetValueOrDefault(EOrderType.Ecommerce),
            PlacedAt = placedAt
        };

        if (customerOrder.DeliveryInfo is not null)
            order.DeliveryInfo = ToDeliveryInfo(order.Id, customerOrder.DeliveryInfo);

        order.Items.AddRange(customerOrder.Items.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ArticleCode = item.ArticleCode.ToString(),
            ArticleTitle = item.ArticleName!.Trim(),
            UnitPrice = item.UnitPrice!.Value,
            Quantity = item.Quantity
        }));

        return order;
    }

    private static OrderDeliveryInfo ToDeliveryInfo(OrderId orderId, CustomersDeliveryInfoResponse request)
    {
        return new OrderDeliveryInfo
        {
            OrderId = orderId,
            Country = DefaultCountry,
            City = request.City ?? string.Empty,
            ZipCode = request.ZipCode ?? string.Empty,
            AddressLine1 = request.AddressLine1 ?? string.Empty,
            AddressLine2 = request.AddressLine2,
            ContactPhone = request.ContactPhone
        };
    }

    private static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        return Math.Round(items.Sum(item => item.UnitPrice * item.Quantity), 2, MidpointRounding.AwayFromZero);
    }

    private static SystemCreateOrderResponse ToResponse(Order entity)
    {
        return new SystemCreateOrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt
        };
    }
}
