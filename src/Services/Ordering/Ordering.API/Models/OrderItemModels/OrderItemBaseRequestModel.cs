using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.OrderItemModels;

public class OrderItemBaseRequestModel
{
    [Required]
    public Guid OrderId { get; set; }
    [Required]
    public Guid ProductId { get; set; }
}
