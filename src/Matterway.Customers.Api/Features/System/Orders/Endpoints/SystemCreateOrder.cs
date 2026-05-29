using Matterway.Customers.Api.Domain.Entities;
using Matterway.Customers.Api.Features.System.Orders.Contracts;
using Matterway.Customers.Api.Infrastructure.Persistence.AddressEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerEntity;
using Matterway.Customers.Api.Infrastructure.Persistence.CustomerOrderEntity;

namespace Matterway.Customers.Api.Features.System.Orders.Endpoints;

public class SystemCreateOrder : IEndpoint
{
    private const string RouteName = nameof(SystemCreateOrder);

    public void MapEndpoint(EndpointRouter endpoints)
    {
        endpoints.MapPost(EndpointKind.System, "orders", Handler)
            .WithName(RouteName)
            .WithSummary("[system] Create customer order from open cart items.")
            .WithTags(nameof(CustomerOrder))
            .Produces<SystemCreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .RequireSystemAccessKey()
            .MapToApiVersion(new ApiVersion(1));
    }

    private static async Task<Results<Created<SystemCreateOrderResponse>, BadRequest<ProblemDetails>, NotFound>>
        Handler(
            SystemCreateOrderRequest request,
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
            : new SystemCreateOrderResponse.DeliveryInfoResponse
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
        return TypedResults.Created(location, ToResponse(createdOrder, deliveryInfo));
    }

    private static async Task<SystemCreateOrderResponse.DeliveryInfoResponse?> ResolveDeliveryInfo(
        Customer customer,
        IAddressRepository addressRepository,
        CancellationToken cancellationToken)
    {
        Address? address = null;

        if (customer.DefaultAddressId.HasValue)
            address = await addressRepository.GetById(customer.DefaultAddressId.Value, cancellationToken);

        address ??= await addressRepository.GetByCustomerId(customer.Id, cancellationToken);

        if (address is null) return null;

        return new SystemCreateOrderResponse.DeliveryInfoResponse
        {
            Country = address.Country,
            City = address.City,
            ZipCode = address.ZipCode,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            ContactPhone = address.ContactPhone
        };
    }

    private static SystemCreateOrderResponse ToResponse(
        CustomerOrder order,
        SystemCreateOrderResponse.DeliveryInfoResponse deliveryInfo)
    {
        var items = order.Items.Select(item => new SystemCreateOrderResponse.Item
        {
            ArticleCode = ArticleCode.Parse(item.ArticleCode, null),
            ArticleName = item.ArticleName,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity
        }).ToList();

        var totalAmount = items.Sum(item => (item.UnitPrice ?? 0m) * item.Quantity);

        return new SystemCreateOrderResponse
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