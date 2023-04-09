using Identity.API.Entities;
using Identity.API.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace Identity.API.Data;

public class IdentityDbContext : DbContext
{
    readonly IConfiguration configuration;
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IConfiguration config)
        : base(options)
    {
        configuration = config;
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

        var initUser = new SystemUser
        {
            Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"),
            Email = "user@example.com",
            Role = SystemUserRole.Admin
        };

        initUser.PasswordHash = new PasswordHasher<SystemUser>().HashPassword(initUser, "string");

        modelBuilder.Entity<SystemUser>().HasData(initUser);

        var (token1, desc) = JwtOperations.Generate(initUser, configuration);
        var (token2, rdesc) = JwtOperations.Generate(initUser, configuration, true);

        modelBuilder.Entity<Session>().HasData(new Session
        {
            Id = new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"),
            SystemUserId = initUser.Id,
            Token = token1,
            RefreshToken = token2,
            Created = desc.IssuedAt.GetValueOrDefault(),
            Expires = desc.Expires.GetValueOrDefault(),
            RefreshExpires = rdesc.Expires.GetValueOrDefault()
        });

        base.OnModelCreating(modelBuilder);
    }
}
