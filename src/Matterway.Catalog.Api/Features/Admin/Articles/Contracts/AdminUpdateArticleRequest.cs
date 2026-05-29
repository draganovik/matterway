using System.ComponentModel.DataAnnotations;
using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Features.Admin.Articles.Contracts;

public record AdminUpdateArticleRequest
{
    public ArticleCode? Code { get; init; }

    [MinLength(1, ErrorMessage = "Title cannot be empty if provided.")]
    public string? Title { get; init; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Base price must be greater than zero.")]
    public decimal? BasePrice { get; init; }

    [MinLength(1, ErrorMessage = "Description cannot be empty if provided.")]
    public string? Description { get; init; }

    public bool? IsAvailable { get; init; }
}
