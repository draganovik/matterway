using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Features.Shared;

public class PagingQueryParams
{
    [FromQuery(Name = "page")]
    [Required]
    [Range(1, int.MaxValue)]
    public int? Page { get; init; }

    [FromQuery(Name = "pageSize")]
    [Required]
    [Range(1, int.MaxValue)]
    public int? PageSize { get; init; }
}