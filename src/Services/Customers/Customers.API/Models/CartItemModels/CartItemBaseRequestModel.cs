using System.ComponentModel.DataAnnotations;

namespace Customers.API.Models.CartItemModels;

public class CartItemBaseRequestModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}