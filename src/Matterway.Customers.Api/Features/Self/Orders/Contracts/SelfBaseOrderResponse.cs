using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Self.Orders.Contracts;

public record SelfBaseOrderResponse
{
    public OrderId OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public DateTime PlacedAt { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyList<Item> Items { get; init; } = [];

    public record Item
    {
        [Required]
        public ArticleCode ArticleCode { get; init; }

        [Required]
        public string? ArticleName { get; init; }

        [Required]
        public decimal? UnitPrice { get; init; }

        [Required]
        public int Quantity { get; init; }
    }
}