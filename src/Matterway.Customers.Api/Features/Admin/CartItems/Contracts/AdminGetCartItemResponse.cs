using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Admin.CartItems.Contracts;

public record AdminGetCartItemResponse
{
    [Required]
    public Guid CustomerId { get; init; }

    [Required]
    public string? ArticleName { get; init; }

    [Required]
    public ArticleCode ArticleCode { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }

    [Required]
    [Range(typeof(decimal), "0.01", "2147483647")]
    public decimal? UnitPrice { get; init; }
}
