using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;
using Matterway.Sales.Api.Infrastructure.Persistence.OrderEntity;

namespace Matterway.Sales.Api.Features.Orders.CreateOrder;

public sealed class CreateOrderService(ICustomersClient customersClient, IOrderRepository orderRepository)
{
    public CreateOrderFailure? ValidateCommand(CreateOrderCommand command)
    {
        return command.CustomerId == Guid.Empty ? CreateOrderFailure.BadRequest("CustomerId is required.") : null;
    }

    public Task<CustomersOrderResult> RequestCustomerOrderAsync(
        Guid orderId,
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var customersRequest = CreateOrderMapper.MapToCustomersRequest(orderId, command);

        return customersClient.CreateOrderAsync(
            command.CustomerId,
            customersRequest,
            command.AuthorizationHeader,
            cancellationToken);
    }

    public CreateOrderFailure? ValidateCustomerOrder(
        Guid expectedOrderId,
        CreateOrderCommand command,
        CustomersOrderResponse? customerOrder)
    {
        if (customerOrder is null)
            return CreateOrderFailure.CustomerOrderCreationFailed();

        if (customerOrder.OrderId != expectedOrderId)
            return CreateOrderFailure.CustomerOrderMismatch("Customer order id does not match the Sales order id.");

        if (customerOrder.CustomerId != command.CustomerId)
            return CreateOrderFailure.CustomerOrderMismatch("Customer id does not match the Sales order request.");

        if (customerOrder.Items.Count == 0)
            return CreateOrderFailure.BadRequest("Order items are missing.");

        if (customerOrder.Items.Any(item => item.UnitPrice is null))
            return CreateOrderFailure.BadRequest("Order items must include unit prices.");

        if (customerOrder.Items.Any(item => string.IsNullOrWhiteSpace(item.ProductName)))
            return CreateOrderFailure.BadRequest("Order items must include product titles.");

        return null;
    }

    public Order BuildOrder(CreateOrderCommand command, CustomersOrderResponse customerOrder)
    {
        return CreateOrderMapper.MapToEntity(command, customerOrder);
    }

    public Task<Order?> PersistAsync(Order order, CancellationToken cancellationToken)
    {
        return orderRepository.Create(order, cancellationToken);
    }
}

public sealed record CreateOrderCommand(
    Guid CustomerId,
    EOrderType? Type,
    CreateOrderDeliveryInfo? DeliveryInfo,
    string? AuthorizationHeader);

public sealed record CreateOrderDeliveryInfo(
    string? Country,
    string City,
    string ZipCode,
    string AddressLine1,
    string? AddressLine2,
    string? ContactPhone);

public sealed record CreateOrderFailure(string Title, string? Detail)
{
    public static CreateOrderFailure BadRequest(string detail)
    {
        return new CreateOrderFailure("Bad Request", detail);
    }

    public static CreateOrderFailure CustomerOrderCreationFailed()
    {
        return new CreateOrderFailure("Customer order could not be created.", null);
    }

    public static CreateOrderFailure CustomerOrderMismatch(string detail)
    {
        return new CreateOrderFailure("Customer order mismatch.", detail);
    }
}