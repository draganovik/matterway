using System.ComponentModel.DataAnnotations;

namespace Identity.Api.Features.Shared;

public class PagingQueryParams
{
    [Required(ErrorMessage = "The field Page is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int? Page { get; init; }

    [Required(ErrorMessage = "The field PageSize is required and must be a valid number.")]
    [Range(1, int.MaxValue)]
    public int? PageSize { get; init; }
}