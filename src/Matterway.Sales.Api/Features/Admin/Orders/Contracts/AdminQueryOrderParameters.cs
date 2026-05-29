namespace Matterway.Sales.Api.Features.Admin.Orders.Contracts;

public sealed record AdminQueryOrderParameters : PaginationRequestParameters
{
    public Guid? CustomerId { get; init; }
}
