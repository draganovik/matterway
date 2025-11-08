using Microsoft.EntityFrameworkCore;
using Matterway.Common.Enums;
using Matterway.Identity.Api.Features.Sessions.Domain;
using Matterway.Identity.Api.Features.SystemUsers.Domain;

namespace Matterway.Identity.Api.Data;

public class IdentityDb : DbContext
{
    public IdentityDb(DbContextOptions<IdentityDb> options)
        : base(options)
    {
    }

    public DbSet<SystemUser> SystemUser { get; set; } = default!;

    public DbSet<Session> Session { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>()
            .HasOne(su => su.SystemUser)
            .WithMany(se => se.Sessions)
            .HasForeignKey(s => s.SystemUserId)
            .IsRequired();

        modelBuilder.Entity<SystemUser>().HasData(
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"),
                Email = "mladen@matterway.com",
                Role = SystemUserRole.Admin,
                Created = new DateTime(2021, 9, 9, 10, 10, 10, DateTimeKind.Utc),
                //sifr45
                PasswordHash = "AQAAAAIAAYagAAAAEF0PUBp9R90/+4Ul8J4HdcOJNNv1Ol5W2vIK2ooqOFRTc1vcgJtxBVqWYlONUSjEJw=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"),
                Email = "jelena@matterway.com",
                Role = SystemUserRole.Manager,
                Created = new DateTime(2022, 10, 10, 11, 11, 11, DateTimeKind.Utc),
                //sifr56
                PasswordHash = "AQAAAAIAAYagAAAAEJru7jHMkz3Cdn5rk9dTj3umC8lMLv0XfkY1k5PXeuh2VkwiazogHTe8IoEhYWr6rg=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                Email = "stefan999@gmail.com",
                Role = SystemUserRole.Customer,
                Created = new DateTime(2023, 4, 12, 12, 10, 0, DateTimeKind.Utc),
                //sifr67
                PasswordHash = "AQAAAAIAAYagAAAAEFSn2wwtXMTkX5P1GglolnIwSI7iauwvIgZXlrTih7oTJ/FWJkY/CXmdNCR4ZJKAug=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                Email = "marag2@gmail.com",
                Role = SystemUserRole.Customer,
                Created = new DateTime(2024, 2, 20, 9, 30, 0, DateTimeKind.Utc),
                //sifr78
                PasswordHash = "AQAAAAIAAYagAAAAEEPDW3c2GRf473Cn5iK6pW8vBqQuPvei43fnIK+WbTaixjrzkd/mMyXGIp2+IoDI8Q=="
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}