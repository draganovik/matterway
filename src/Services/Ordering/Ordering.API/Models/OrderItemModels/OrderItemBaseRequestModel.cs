using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.OrderItemModels;

public class OrderItemBaseRequestModel
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Units must be greater than 0")]
    public int Units { get; set; }
}
