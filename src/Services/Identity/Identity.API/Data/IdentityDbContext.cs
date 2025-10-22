using Identity.API.Features.Sessions.Domain;
using Identity.API.Features.SystemUsers.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Identity.API.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
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
                Created = DateTime.Parse("2021-09-09T10:10:10Z"),
                //sifr45
                PasswordHash = "AQAAAAIAAYagAAAAEF0PUBp9R90/+4Ul8J4HdcOJNNv1Ol5W2vIK2ooqOFRTc1vcgJtxBVqWYlONUSjEJw=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"),
                Email = "jelena@matterway.com",
                Role = SystemUserRole.Manager,
                Created = DateTime.Parse("2022-10-10T11:11:11Z"),
                //sifr56
                PasswordHash = "AQAAAAIAAYagAAAAEJru7jHMkz3Cdn5rk9dTj3umC8lMLv0XfkY1k5PXeuh2VkwiazogHTe8IoEhYWr6rg=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
                Email = "stefan999@gmail.com",
                Role = SystemUserRole.Customer,
                Created = DateTime.Parse("2023-04-12T12:10:00Z"),
                //sifr67
                PasswordHash = "AQAAAAIAAYagAAAAEFSn2wwtXMTkX5P1GglolnIwSI7iauwvIgZXlrTih7oTJ/FWJkY/CXmdNCR4ZJKAug=="
            },
            new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
                Email = "marag2@gmail.com",
                Role = SystemUserRole.Customer,
                Created = DateTime.Parse("2024-02-20T09:30:00Z"),
                //sifr78
                PasswordHash = "AQAAAAIAAYagAAAAEEPDW3c2GRf473Cn5iK6pW8vBqQuPvei43fnIK+WbTaixjrzkd/mMyXGIp2+IoDI8Q=="
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}