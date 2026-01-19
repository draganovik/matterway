using Matterway.Sales.Api.Application;

namespace Matterway.Sales.Api.Features.Orders.QueryOrders;

public sealed record QueryOrdersParameters : PaginationRequestParameters
{
    public Guid? CustomerId { get; init; }
}