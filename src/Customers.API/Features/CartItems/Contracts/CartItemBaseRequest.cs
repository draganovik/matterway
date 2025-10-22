using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Features.CartItems.Contracts;

public class CartItemBaseRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}