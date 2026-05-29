namespace Matterway.Catalog.Api.Features.Admin.Articles.Contracts;

public record AdminDeleteArticleResponse
{
    public required ArticleCode Code { get; init; }
    public string Message { get; init; } = "Article deleted successfully.";
}