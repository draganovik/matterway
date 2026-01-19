using Matterway.Sales.Api.Domain;
using Matterway.Sales.Api.Domain.Entities;
using Matterway.Sales.Api.Infrastructure.Brokers.Customers;

namespace Matterway.Sales.Api.Features.Orders.CreateOrder;

public static class CreateOrderMapper
{
    public static CustomersCreateOrderRequest MapToCustomersRequest(Guid orderId, CreateOrderCommand command)
    {
        return new CustomersCreateOrderRequest
        {
            OrderId = orderId,
            DeliveryInfo = command.DeliveryInfo is null
                ? null
                : new CustomersDeliveryInfoRequest
                {
                    Country = string.IsNullOrWhiteSpace(command.DeliveryInfo.Country)
                        ? "Serbia"
                        : command.DeliveryInfo.Country.Trim(),
                    City = command.DeliveryInfo.City,
                    ZipCode = command.DeliveryInfo.ZipCode,
                    AddressLine1 = command.DeliveryInfo.AddressLine1,
                    AddressLine2 = command.DeliveryInfo.AddressLine2 ?? string.Empty,
                    ContactPhone = command.DeliveryInfo.ContactPhone
                }
        };
    }

    public static Order MapToEntity(CreateOrderCommand command, CustomersOrderResponse customerOrder)
    {
        var order = new Order
        {
            Id = customerOrder.OrderId,
            CustomerId = command.CustomerId,
            Type = command.Type ?? EOrderType.Ecommerce,
            PlacedAt = customerOrder.PlacedAt
        };

        if (customerOrder.DeliveryInfo is not null)
            order.DeliveryInfo = MapToDeliveryInfo(order.Id, customerOrder.DeliveryInfo);

        order.Items.AddRange(customerOrder.Items.Select(item => new OrderItem
        {
            OrderId = order.Id,
            ProductId = item.ProductId,
            ProductTitle = item.ProductName?.Trim() ?? string.Empty,
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
                ProductId = item.ProductId,
                ProductTitle = item.ProductTitle,
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