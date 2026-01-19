using Matterway.Sales.Api.Application;

namespace Matterway.Sales.Api.Features.Payments.QueryPayments;

public sealed record QueryPaymentsParameters : PaginationRequestParameters
{
    public Guid? OrderId { get; init; }
}