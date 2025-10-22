using System.ComponentModel.DataAnnotations;

namespace Ordering.Api.Features.Addresses.Contracts;

public class AddressBaseResponse
{
    public Guid Id { get; set; }
    public string? ReceiverName { get; set; }
    public string? Residence { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }

    [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "Invalid zip code. Zip code must be 5 digits")]
    public string? ZipCode { get; set; }

    public string? Note { get; set; }
}