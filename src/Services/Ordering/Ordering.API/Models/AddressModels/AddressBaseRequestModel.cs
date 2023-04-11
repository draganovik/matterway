using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Models.AddressModels;

public class AddressBaseRequestModel
{
    [Required]
    public string? ReceiverName { get; set; }
    [Required]
    public string? Residence { get; set; }
    [Required]
    public string? Street { get; set; }
    [Required]
    public string? City { get; set; }
    [Required]
    [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Invalid zip code. Zip code must be 5 digits")]
    public string? ZipCode { get; set; }
    [Required]
    public string? Note { get; set; }
}
