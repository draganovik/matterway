using Catalog.API.Features.Products.Domain;
using Shared.Enums;

namespace Catalog.API.Features.Products.Shared;

public class ProductFilter
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
                pd.Value.ToLower().Contains(CategoryLike.ToLower())));
        if (ProductDetailsLike?.Length > 0)
        {
            foreach (var productDetailLike in ProductDetailsLike)
            {
                query = query.Where(p => p.ProductDetails != null && p.ProductDetails.Any(pd =>
                    pd.Value != null && pd.Value.ToLower().Contains(productDetailLike.ToLower())));
            }
        }

        if (IsAvailable.HasValue) query = query.Where(p => p.IsAvailable == IsAvailable);

        return query;
    }
}