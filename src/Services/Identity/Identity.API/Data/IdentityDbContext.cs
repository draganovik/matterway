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

        var adminUser = new SystemUser
        {
            Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"),
            Email = "mladen@matterway.com",
            Role = SystemUserRole.Admin
        };

        adminUser.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(adminUser, "sifr45");

        var managerUser = new SystemUser
        {
            Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2"),
            Email = "jelena@matterway.com",
            Role = SystemUserRole.Manager
        };

        managerUser.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(managerUser, "sifr56");

        var customer1User = new SystemUser
        {
            Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3"),
            Email = "stefan999@gmail.com",
            Role = SystemUserRole.Customer
        };

        customer1User.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(customer1User, "sifr67");

        var customer2User = new SystemUser
        {
            Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4"),
            Email = "marag2@gmail.com",
            Role = SystemUserRole.Customer
        };

        customer2User.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(customer2User, "sifr78");

        modelBuilder.Entity<SystemUser>().HasData(adminUser, managerUser, customer1User, customer2User);

        base.OnModelCreating(modelBuilder);
    }
}