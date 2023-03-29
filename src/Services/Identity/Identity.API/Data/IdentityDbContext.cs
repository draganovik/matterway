using Identity.API.Entities;
using Identity.API.Enums;
using Identity.API.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data
{
    public class IdentityDbContext : DbContext
    {
        IConfiguration configuration;
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IConfiguration config)
            : base(options)
        {
            configuration = config;
        }

        public DbSet<SystemUser> SystemUser { get; set; } = default!;

        public DbSet<Session> Session { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SystemUser>()
                .HasOne(s => s.Session)
                .WithOne(u => u.SystemUser)
                .HasForeignKey<Session>(s => s.SystemUserId)
                .IsRequired();

            var initUser = new SystemUser
            {
                Id = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1"),
                Email = "user@example.com",
                PasswordHash = new PasswordHasher<SystemUser>().HashPassword(null, "password1"),
                Role = SystemUserRole.Customer
            };

            modelBuilder.Entity<SystemUser>().HasData(initUser);

            var (token1, desc) = JwtOperations.Generate(initUser, configuration);
            var (token2, _) = JwtOperations.Generate(initUser, configuration, true);

            modelBuilder.Entity<Session>().HasData(new Session
            {
                Id = new Guid("4e54e945-90e7-4f75-88f7-9d9b84d7c81c"),
                SystemUserId = initUser.Id,
                Token = token1,
                RefreshToken = token2,
                Created = desc.IssuedAt.GetValueOrDefault(),
                Expires = desc.Expires.GetValueOrDefault()
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
