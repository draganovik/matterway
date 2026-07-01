using System.ComponentModel.DataAnnotations;

namespace Matterway.Catalog.Api.Features.Admin.Discounts.Contracts;

public record AdminUpdateDiscountRequest
{
    [Range(0.01, 1, ErrorMessage = "Percentage must be between 0.01 and 1.")]
    public required decimal Percentage { get; init; }

    [Required]
    public required DateTime ValidFrom { get; init; }

    public DateTime? ValidTo { get; init; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one articleCode is required.")]
    [ArticleCodeCollection]
    public ICollection<string>? ArticleCodes { get; init; }
}