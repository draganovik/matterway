using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Providers.Persistence;

public class IdentityDbComposer(DbContextOptions<IdentityDbComposer> options)
    : IdentityDbContext<SystemUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ModelDataLoader.InitializeDemo(modelBuilder);
    }
}