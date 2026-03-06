using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

public class EfPgCustomerArticleRepository(CustomersDbComposer context) : ICustomerArticleRepository
{
    public async Task<ICollection<CustomerArticle>> QueryCart(Guid customerId, int pageIndex, int pageSize,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle
            .Where(article => article.CustomerId == customerId && article.OrderId == null)
            .AsNoTracking()
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerArticle?> UpsertCartItem(CustomerArticle requestModel,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.CustomerArticle
            .FirstOrDefaultAsync(article =>
                    article.CustomerId == requestModel.CustomerId &&
                    article.ArticleCode == requestModel.ArticleCode &&
                    article.OrderId == null,
                cancellationToken);

        if (existing is not null)
        {
            // Update only mutable fields to avoid touching the primary key.
            existing.Quantity = requestModel.Quantity;
            existing.UnitPrice = requestModel.UnitPrice;
            existing.ArticleName = requestModel.ArticleName;
            existing.ArticleCode = requestModel.ArticleCode;
            context.CustomerArticle.Update(existing);
        }
        else
        {
            context.CustomerArticle.Add(requestModel);
        }

        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0 ? existing ?? requestModel : null;
    }

    public async Task<bool> DeleteCartItem(Guid customerId, ArticleCode articleCode,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.CustomerArticle
            .Where(model => model.CustomerId == customerId)
            .Where(model => model.ArticleCode == articleCode.Value)
            .Where(model => model.OrderId == null)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<CustomerArticle?> GetCartItem(Guid customerId, ArticleCode articleCode,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle.FirstOrDefaultAsync(article =>
                article.CustomerId == customerId &&
                article.ArticleCode == articleCode.Value &&
                article.OrderId == null,
            cancellationToken);
    }

    public async Task<int> CountCart(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle
            .Where(article => article.CustomerId == customerId && article.OrderId == null)
            .CountAsync(cancellationToken);
    }
}