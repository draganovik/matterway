using System.ComponentModel.DataAnnotations;

namespace Ordering.API.Features.Addresses.Domain;

public class Address
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string? ReceiverName { get; set; }

    [Required]
    public string? Residence { get; set; }

    [Required]
    public string? Street { get; set; }

    [Required]
    public string? City { get; set; }

    [Required]
    public string Country { get; set; } = "Serbia";

    [Required]
    [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Invalid zip code. Zip code must be 5 digits")]
    public string? ZipCode { get; set; }

    public string? Note { get; set; }
}