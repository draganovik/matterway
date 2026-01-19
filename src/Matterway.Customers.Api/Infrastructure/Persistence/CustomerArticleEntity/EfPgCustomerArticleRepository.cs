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

    public async Task<ICollection<CustomerArticle>> GetCartItems(Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle
            .Where(article => article.CustomerId == customerId && article.OrderId == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<CustomerArticle?> UpsertCartItem(CustomerArticle requestModel,
        CancellationToken cancellationToken = default)
    {
        var existing = await context.CustomerArticle
            .FirstOrDefaultAsync(article =>
                    article.CustomerId == requestModel.CustomerId &&
                    article.ProductId == requestModel.ProductId &&
                    article.OrderId == null,
                cancellationToken);

        if (existing is not null)
            context.CustomerArticle.Entry(existing).CurrentValues.SetValues(requestModel);
        else
            context.CustomerArticle.Add(requestModel);

        var affected = await context.SaveChangesAsync(cancellationToken);
        return affected > 0 ? existing ?? requestModel : null;
    }

    public async Task<bool> DeleteCartItem(Guid customerId, Guid productId,
        CancellationToken cancellationToken = default)
    {
        var affected = await context.CustomerArticle
            .Where(model => model.CustomerId == customerId)
            .Where(model => model.ProductId == productId)
            .Where(model => model.OrderId == null)
            .ExecuteDeleteAsync(cancellationToken);
        return affected == 1;
    }

    public async Task<CustomerArticle?> GetCartItem(Guid customerId, Guid productId,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle.FirstOrDefaultAsync(article =>
                article.CustomerId == customerId &&
                article.ProductId == productId &&
                article.OrderId == null,
            cancellationToken);
    }

    public async Task<int> CountCart(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle
            .Where(article => article.CustomerId == customerId && article.OrderId == null)
            .CountAsync(cancellationToken);
    }

    public async Task<ICollection<CustomerArticle>> QueryByOrderId(Guid orderId,
        CancellationToken cancellationToken = default)
    {
        return await context.CustomerArticle
            .Where(article => article.OrderId == orderId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}