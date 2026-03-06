using Matterway.Catalog.Api.Domain.Entities;

namespace Matterway.Catalog.Api.Infrastructure.ImportExport.Rows;

public sealed record DiscountRow(
    string Code,
    string ArticleCode,
    decimal Percentage,
    DateTime ValidFrom,
    DateTime? ValidTo)
{
    public static DiscountRow FromEntity(Discount entity)
    {
        return new DiscountRow(
            entity.Code,
            entity.ArticleCode,
            entity.Percentage,
            entity.ValidFrom,
            entity.ValidTo);
    }

    public Discount ToEntity()
    {
        return new Discount
        {
            Code = Code,
            ArticleCode = ArticleCode,
            Percentage = Percentage,
            ValidFrom = ValidFrom,
            ValidTo = ValidTo
        };
    }
}