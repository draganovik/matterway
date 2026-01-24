using System.ComponentModel.DataAnnotations;
using System.Net;
using Asp.Versioning;
using Matterway.Sales.Api.Application;
using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Matterway.Sales.Api.Features.Self.Orders;

public class SelfCreateOrder : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("self/orders", Handle)
            .WithName("SelfCreateOrder").WithSummary("Create own Order from cart items.")
            .WithTags(nameof(Order))
            .Produces<OrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .RequireAuthorization(policy => policy.RequireAssertion(context =>
                RequestIdentity.IsCustomer(context.User)))
            .MapToApiVersion(new ApiVersion(1, 0));
    }

    private static async Task<Results<Created<OrderResponse>, BadRequest<ProblemDetails>, NotFound, ForbidHttpResult>>
        Handle(
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

        var orderId = Guid.CreateVersion7();
        var customersRequest = MapToCustomersRequest(orderId, request);
        var authorization = httpContext.Request.Headers.Authorization.ToString();
        var customersResult = await customersClient.CreateOrderAsync(
            effectiveCustomerId,
            customersRequest,
            authorization,
            cancellationToken);

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

        if (customersResult.Order is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Customer order could not be created.",
                Status = StatusCodes.Status400BadRequest
            });

        if (customersResult.Order.OrderId != orderId)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Customer order mismatch.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer order id does not match the Sales order id."
            });

        if (customersResult.Order.CustomerId != effectiveCustomerId)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Customer order mismatch.",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer id does not match the Sales order request."
            });

        if (customersResult.Order.Items.Count == 0)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Order items are missing."
            });

        if (customersResult.Order.Items.Any(item => item.UnitPrice is null))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Order items must include unit prices."
            });

        if (customersResult.Order.Items.Any(item => string.IsNullOrWhiteSpace(item.ArticleName)))
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Order items must include article titles."
            });

        var order = MapToEntity(request, customersResult.Order, effectiveCustomerId);

        var created = await orderRepository.Create(order, cancellationToken);
        if (created is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Order could not be created.",
                Status = StatusCodes.Status400BadRequest
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
        public Guid Id { get; init; }
        public Guid? CustomerId { get; init; }
        public EOrderType Type { get; init; }
        public decimal TotalAmount { get; init; }
        public DateTime PlacedAt { get; init; }
        public DeliveryInfoResponse? DeliveryInfo { get; init; }
        public IReadOnlyList<OrderItemResponse> Items { get; init; } = [];
        public IReadOnlyList<OrderStatusResponse> StatusHistory { get; init; } = [];
        public IReadOnlyList<PaymentSnapshotResponse> Payments { get; init; } = [];
    }

    public record DeliveryInfoResponse
    {
        public string? Country { get; init; }
        public string? City { get; init; }
        public string? ZipCode { get; init; }
        public string? AddressLine1 { get; init; }
        public string? AddressLine2 { get; init; }
        public string? ContactPhone { get; init; }
    }

    public record OrderItemResponse
    {
        public Guid Id { get; init; }
        public Guid ArticleId { get; init; }
        public string? ArticleTitle { get; init; }
        public decimal UnitPrice { get; init; }
        public int Quantity { get; init; }
    }

    public record OrderStatusResponse
    {
        public EOrderStatusType Status { get; init; }
        public DateTime ChangedAt { get; init; }
        public string? Note { get; init; }
    }

    public record PaymentSnapshotResponse
    {
        public Guid Id { get; init; }
        public string? Provider { get; init; }
        public string? ReferenceId { get; init; }
        public decimal Amount { get; init; }
        public EPaymentStatus Status { get; init; }
        public DateTime CreatedAt { get; init; }
    }

    private static CustomersCreateOrderRequest MapToCustomersRequest(Guid orderId, CreateOrderRequest request)
    {
        return new CustomersCreateOrderRequest
        {
            OrderId = orderId,
            DeliveryInfo = request.DeliveryInfo is null
                ? null
                : new CustomersDeliveryInfoRequest
                {
                    Country = string.IsNullOrWhiteSpace(request.DeliveryInfo.Country)
                        ? "Serbia"
                        : request.DeliveryInfo.Country.Trim(),
                    City = request.DeliveryInfo.City,
                    ZipCode = request.DeliveryInfo.ZipCode,
                    AddressLine1 = request.DeliveryInfo.AddressLine1,
                    AddressLine2 = request.DeliveryInfo.AddressLine2 ?? string.Empty,
                    ContactPhone = request.DeliveryInfo.ContactPhone
                }
        };
    }

    private static Order MapToEntity(CreateOrderRequest request, CustomersOrderResponse customerOrder, Guid customerId)
    {
        var order = new Order
        {
            Id = customerOrder.OrderId,
            CustomerId = customerId,
            Type = request.Type ?? EOrderType.Ecommerce,
            PlacedAt = customerOrder.PlacedAt
        };

        if (customerOrder.DeliveryInfo is not null)
            order.DeliveryInfo = MapToDeliveryInfo(order.Id, customerOrder.DeliveryInfo);

        order.Items.AddRange(customerOrder.Items.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ArticleId = item.ArticleId,
            ArticleTitle = item.ArticleName?.Trim() ?? string.Empty,
            UnitPrice = item.UnitPrice ?? 0m,
            Quantity = item.Quantity
        }));

        order.StatusHistory.Add(new OrderStatus
        {
            OrderId = order.Id,
            Status = EOrderStatusType.Processing,
            ChangedAt = DateTime.UtcNow
        });

        return order;
    }

    private static OrderDeliveryInfo MapToDeliveryInfo(Guid orderId, CustomersDeliveryInfoResponse request)
    {
        var resolvedCountry = string.IsNullOrWhiteSpace(request.Country)
            ? "Serbia"
            : request.Country.Trim();

        return new OrderDeliveryInfo
        {
            OrderId = orderId,
            Country = resolvedCountry,
            City = request.City ?? string.Empty,
            ZipCode = request.ZipCode ?? string.Empty,
            AddressLine1 = request.AddressLine1 ?? string.Empty,
            AddressLine2 = request.AddressLine2,
            ContactPhone = request.ContactPhone
        };
    }

    private static decimal CalculateTotal(IEnumerable<OrderItem> items)
    {
        var total = items.Sum(item => item.UnitPrice * item.Quantity);
        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    public static OrderResponse MapToResponse(Order entity)
    {
        return new OrderResponse
        {
            Id = entity.Id,
            CustomerId = entity.CustomerId,
            Type = entity.Type,
            TotalAmount = CalculateTotal(entity.Items),
            PlacedAt = entity.PlacedAt,
            DeliveryInfo = entity.DeliveryInfo is null
                ? null
                : new DeliveryInfoResponse
                {
                    Country = entity.DeliveryInfo.Country,
                    City = entity.DeliveryInfo.City,
                    ZipCode = entity.DeliveryInfo.ZipCode,
                    AddressLine1 = entity.DeliveryInfo.AddressLine1,
                    AddressLine2 = entity.DeliveryInfo.AddressLine2,
                    ContactPhone = entity.DeliveryInfo.ContactPhone
                },
            Items = entity.Items.Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ArticleId = item.ArticleId,
                ArticleTitle = item.ArticleTitle,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            }).ToList(),
            StatusHistory = entity.StatusHistory.Select(status => new OrderStatusResponse
            {
                Status = status.Status,
                ChangedAt = status.ChangedAt,
                Note = status.Note
            }).ToList(),
            Payments = entity.Payments.Select(payment => new PaymentSnapshotResponse
            {
                Id = payment.Id,
                Provider = payment.Provider,
                ReferenceId = payment.ReferenceId,
                Amount = payment.Amount,
                Status = payment.Status,
                CreatedAt = payment.CreatedAt
            }).ToList()
        };
    }
}