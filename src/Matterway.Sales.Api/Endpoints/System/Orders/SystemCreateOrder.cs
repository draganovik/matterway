using System.ComponentModel.DataAnnotations;
using System.Net;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Endpoints.System.Orders;

public class SystemCreateOrder : IEndpoint
{
    private const string DefaultCountry = "Serbia";

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "orders", Handler)
            .WithName("SystemCreateOrder")
            .WithSummary("[system] Create customer Order from cart items.")
            .WithTags(nameof(Order))
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesValidationProblem()
            .RequireSystemAccessKey()
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<
            Results<Created<OrderResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult,
                ProblemHttpResult>>
        Handler(
            CreateOrderRequest request,
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
        var customersRequest = MapToCustomersRequest(effectiveCustomerId, orderId, request);
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
                _ => TypedResults.Problem(new ProblemDetails
                {
                    Title = "Customer order could not be created.",
                    Status = (int)statusCode,
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

        var order = MapToEntity(request, customersOrder, effectiveCustomerId, pendingOrder.PlacedAt);

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

        return TypedResults.Created(location, MapToResponse(created));
    }

    public record CreateOrderRequest
    {
        public EOrderType? Type { get; init; }

        public DeliveryInfoRequest? DeliveryInfo { get; init; }
    }

    public record DeliveryInfoRequest
    {
        public string? Country { get; init; }

        [Required]
        public required string City { get; init; }

        [Required]
        public required string ZipCode { get; init; }

        [Required]
        public required string AddressLine1 { get; init; }

        public string? AddressLine2 { get; init; }

        public string? ContactPhone { get; init; }
    }

    public record OrderResponse
    {
        public OrderId Id { get; init; }
        public Guid? CustomerId { get; init; }
        public EOrderType Type { get; init; }
        public decimal TotalAmount { get; init; }
        public DateTime PlacedAt { get; init; }
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

    private static CustomersCreateOrderRequest MapToCustomersRequest(
        Guid customerId,
        OrderId orderId,
        CreateOrderRequest request)
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

    private static Order MapToEntity(
        CreateOrderRequest request,
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
            order.DeliveryInfo = MapToDeliveryInfo(order.Id, customerOrder.DeliveryInfo);

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

    private static OrderDeliveryInfo MapToDeliveryInfo(OrderId orderId, CustomersDeliveryInfoResponse request)
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

    private static OrderResponse MapToResponse(Order entity)
    {
        return new OrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt
        };
    }
}