using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Features.OrderItems.Contracts;

public class OrderItemBaseRequest
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}