using Matterway.Customers.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Customers.Api.Infrastructure.Persistence;

public static class ModelDataLoader
{
    private const string PhilipsHueArticleCode = "PHUE0002";
    private const string RingArticleCode = "RING0001";

    public static void Initialize(ModelBuilder modelBuilder)
    {
        #region Customers data

        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                FirstName = "Stefan",
                LastName = "Stefanov",
                BirthDate = new DateOnly(1980, 1, 1),
                DefaultAddressId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5")
            },
            new Customer
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                FirstName = "Mara",
                LastName = "Jakov",
                BirthDate = new DateOnly(2000, 5, 5),
                DefaultAddressId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b6")
            }
        );

        #endregion

        #region CustomerArticle

        modelBuilder.Entity<CustomerArticle>().HasData(
            new CustomerArticle
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b7"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                ArticleCode = PhilipsHueArticleCode,
                ArticleName = "Philips Hue White and Color Ambiance A19 Smart LED Bulb",
                UnitPrice = 4999m,
                Quantity = 3
            },
            new CustomerArticle
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b8"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                ArticleCode = RingArticleCode,
                ArticleName = "Ring Spotlight Cam",
                UnitPrice = 19999m,
                Quantity = 1
            }
        );

        #endregion

        #region Addresses data

        modelBuilder.Entity<Address>().HasData(
            new Address
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b5"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                Country = "Serbia",
                City = "Novi Sad",
                ZipCode = "21000",
                AddressLine1 = "Futog",
                AddressLine2 = "23b",
                ContactPhone = "+381601234567"
            },
            new Address
            {
                Id = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b6"),
                CustomerId = Guid.Parse("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                Country = "Serbia",
                City = "Beograd",
                ZipCode = "11000",
                AddressLine1 = "Kralja Milana",
                AddressLine2 = "34/10",
                ContactPhone = "+381676543210"
            }
        );

        #endregion
    }
}