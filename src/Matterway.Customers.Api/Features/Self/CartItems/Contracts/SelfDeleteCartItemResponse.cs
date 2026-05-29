namespace Matterway.Customers.Api.Features.Self.CartItems.Contracts;

public record SelfDeleteCartItemResponse
{
    public Guid CustomerId { get; init; }
    public ArticleCode ArticleCode { get; init; }
    public string Message { get; init; } = "Cart item removed successfully.";
}
