using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.Details.Contracts;

public record AdminPutDetailRequest
{
    [Required]
    [StringLength(120, MinimumLength = 1)]
    public required string Title { get; init; }

    [StringLength(40)]
    public string? Unit { get; init; }
}
