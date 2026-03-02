using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Matterway.Customers.Api.Infrastructure.Persistence.CustomerArticleEntity;

internal sealed class CustomerArticleEntityTypeConfiguration : IEntityTypeConfiguration<CustomerArticle>
{
    public void Configure(EntityTypeBuilder<CustomerArticle> builder)
    {
        builder.ToTable(nameof(CustomerArticle),
            tb => { tb.HasCheckConstraint("CK_CustomerArticle_Quantity", "\"Quantity\" >= 1"); });

        builder.HasKey(article => article.Id);

        builder.Property(article => article.CustomerId)
            .IsRequired();

        builder.Property(article => article.ArticleId)
            .IsRequired();

        builder.Property(article => article.OrderId)
            .IsRequired(false);

        builder.Property(article => article.Quantity)
            .IsRequired();

        builder.Property(article => article.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(article => article.ArticleName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(article => article.ArticleCode)
            .IsRequired(false)
            .HasMaxLength(10);

        builder.HasOne(article => article.Customer)
            .WithMany()
            .HasForeignKey(article => article.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(article => article.Order)
            .WithMany(order => order.Items)
            .HasForeignKey(article => article.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(article => article.CustomerId);

        builder.HasIndex(article => new { article.CustomerId, article.ArticleId, article.OrderId })
            .IsUnique()
            .HasFilter("\"OrderId\" IS NOT NULL");

        builder.HasIndex(article => new { article.CustomerId, article.ArticleId })
            .IsUnique()
            .HasFilter("\"OrderId\" IS NULL");
    }
}