using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.Self.CartItems.Contracts;

public record SelfUpsertCartItemRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}
