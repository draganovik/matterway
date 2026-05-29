namespace Matterway.Sales.Api.Features.Admin.Payments.Contracts;

public sealed record AdminQueryPaymentParameters : PaginationRequestParameters
{
    public OrderId? OrderId { get; init; }
}