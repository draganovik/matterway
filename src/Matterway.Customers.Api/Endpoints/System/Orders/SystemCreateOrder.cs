using System.ComponentModel.DataAnnotations;
using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

namespace Matterway.Customers.Api.Endpoints.System.Orders;

public class SystemCreateOrder : IEndpoint
{
    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "orders", Handler)
            .WithName("SystemCreateOrder")
            .WithSummary("[system] Create customer order from open cart items.")
            .WithTags(nameof(CustomerOrder))
            .Produces<CustomerOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<CustomerOrderResponse>, BadRequest<ProblemDetails>, NotFound>> Handler(
        CreateCustomerOrderRequest request,
        HttpContext httpContext,
        ICustomerRepository customerRepository,
        IAddressRepository addressRepository,
        ICustomerOrderRepository customerOrderRepository,
        CancellationToken cancellationToken)
    {
        if (request.CustomerId == Guid.Empty)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "CustomerId is required."
            });

        if (request.OrderId == default)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "OrderId is required."
            });

        var customer = await customerRepository.GetBy(request.CustomerId, cancellationToken);
        if (customer is null) return TypedResults.NotFound();

        var deliveryInfo = request.DeliveryInfo is null
            ? await ResolveDeliveryInfo(customer, addressRepository, cancellationToken)
            : new DeliveryInfoResponse
            {
                Country = request.DeliveryInfo.Country,
                City = request.DeliveryInfo.City,
                ZipCode = request.DeliveryInfo.ZipCode,
                AddressLine1 = request.DeliveryInfo.AddressLine1,
                AddressLine2 = request.DeliveryInfo.AddressLine2,
                ContactPhone = request.DeliveryInfo.ContactPhone
            };

        if (deliveryInfo is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Customer address is unavailable."
            });

        var createdOrder = await customerOrderRepository.CreateFromCart(request.CustomerId, request.OrderId,
            cancellationToken);
        if (createdOrder is null)
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Unable to create order from cart items."
            });

        var location = $"{httpContext.Request.Path}/{createdOrder.OrderId}";
        return TypedResults.Created(location, MapToResponse(createdOrder, deliveryInfo));
    }

    public record CreateCustomerOrderRequest
    {
        [Required]
        public Guid CustomerId { get; init; }

        [Required]
        public OrderId OrderId { get; init; }

        public DeliveryInfoRequest? DeliveryInfo { get; init; }
    }

    public record DeliveryInfoRequest
    {
        [Required]
        public string Country { get; init; } = string.Empty;

        [Required]
        public string City { get; init; } = string.Empty;

        [Required]
        public string ZipCode { get; init; } = string.Empty;

        [Required]
        public string AddressLine1 { get; init; } = string.Empty;

        [Required]
        public string AddressLine2 { get; init; } = string.Empty;

        public string? ContactPhone { get; init; }
    }

    public record CustomerOrderResponse
    {
        public OrderId OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public DateTime PlacedAt { get; init; }
        public decimal TotalAmount { get; init; }
        public DeliveryInfoResponse? DeliveryInfo { get; init; }
        public IReadOnlyList<CustomerArticleResponse> Items { get; init; } = [];
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

    public record CustomerArticleResponse
    {
        public ArticleCode ArticleCode { get; init; }
        public string? ArticleName { get; init; }
        public decimal? UnitPrice { get; init; }
        public int Quantity { get; init; }
    }

    private static async Task<DeliveryInfoResponse?> ResolveDeliveryInfo(
        Customer customer,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        Address? address = null;

        if (customer.DefaultAddressId.HasValue)
            address = await addressRepository.GetById(customer.DefaultAddressId.Value, cancellationToken);

        address ??= await addressRepository.GetByCustomerId(customer.Id, cancellationToken);

        if (address is null) return null;

        return new DeliveryInfoResponse
        {
            Country = address.Country,
            City = address.City,
            ZipCode = address.ZipCode,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            ContactPhone = address.ContactPhone
        };
    }

    private static CustomerOrderResponse MapToResponse(CustomerOrder order, DeliveryInfoResponse deliveryInfo)
    {
        var items = order.Items.Select(item => new CustomerArticleResponse
        {
            ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
            ArticleName = item.ArticleName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();

        var totalAmount = items.Sum(item => (item.UnitPrice ?? 0m) * item.Quantity);

        return new CustomerOrderResponse
        {
            OrderId = order.OrderId,
            CustomerId = order.CustomerId,
            PlacedAt = order.PlacedAt,
            TotalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero),
            DeliveryInfo = deliveryInfo,
            Items = items
        };
    }
}