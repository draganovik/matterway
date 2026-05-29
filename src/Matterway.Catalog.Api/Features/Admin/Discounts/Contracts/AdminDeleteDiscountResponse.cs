namespace Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;

public record AdminDeleteDiscountResponse
{
    public string Code { get; init; } = string.Empty;
    public int RemovedCount { get; init; }
    public string Message { get; init; } = "Discount(s) removed successfully.";
}