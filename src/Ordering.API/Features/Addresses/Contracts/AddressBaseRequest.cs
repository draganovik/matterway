using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Features.Addresses.Contracts;

public class AddressBaseRequest
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

    public string? Note { get; set; }
}