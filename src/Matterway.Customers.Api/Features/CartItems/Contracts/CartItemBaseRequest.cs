using System.ComponentModel.DataAnnotations;

namespace Matterway.Customers.Api.Features.CartItems.Contracts;

public class CartItemBaseRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}