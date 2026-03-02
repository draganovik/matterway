using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record DiscountRow(
    string Code,
    Guid ArticleId,
    decimal Percentage,
    DateTime ValidFrom,
    DateTime? ValidTo)
{
    public static DiscountRow FromEntity(Discount entity)
    {
        return new DiscountRow(
            entity.Code,
            entity.ArticleId,
            entity.Percentage,
            entity.ValidFrom,
            entity.ValidTo);
    }

    public Discount ToEntity()
    {
        return new Discount
        {
            Code = Code,
            ArticleId = ArticleId,
            Percentage = Percentage,
            ValidFrom = ValidFrom,
            ValidTo = ValidTo
        };
    }
}