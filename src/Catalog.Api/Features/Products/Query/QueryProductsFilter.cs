using Catalog.Api.Domain;
using Matterway.Common.Enums;

namespace Catalog.Api.Features.Products.Query;

public record QueryProductFilter
{
    public string? TitleLike { get; set; }
    public double? PriceMin { get; set; }
    public double? PriceMax { get; set; }
    public string? CategoryLike { get; set; }
    public string[]? ProductDetailsLike { get; set; }
    public bool? IsAvailable { get; set; }

    public IQueryable<Product> GenerateQuery(IQueryable<Product> query)
    {
        if (TitleLike != null)
            query = query.Where(p => p.Title != null && p.Title.ToLower().Contains(TitleLike.ToLower()));
        if (PriceMin.HasValue) query = query.Where(p => p.Price >= PriceMin);
        if (PriceMax.HasValue) query = query.Where(p => p.Price <= PriceMax);
        if (CategoryLike != null)
            query = query.Where(p => p.ProductDetails != null && p.ProductDetails.Any(pd =>
                pd.Type == DetailType.Category && pd.Value != null &&
                pd.Value.Contains(CategoryLike, StringComparison.CurrentCultureIgnoreCase)));
        if (ProductDetailsLike?.Length > 0)
        {
            query = ProductDetailsLike.Aggregate(query,
                (current, productDetailLike) => current.Where(p =>
                    p.ProductDetails != null && p.ProductDetails.Any(pd =>
                        pd.Value != null && pd.Value.ToLower().Contains(productDetailLike.ToLower()))));
        }

        if (IsAvailable.HasValue) query = query.Where(p => p.IsAvailable == IsAvailable);

        return query;
    }
}