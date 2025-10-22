using System.ComponentModel.DataAnnotations;

namespace Customers.Api.Features.CartItems.Contracts;

public class CartItemBaseResponse
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public string? ProductName { get; set; }

    [Required]
    public Guid ProductId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public double UnitPrice { get; set; }
}