using Matterway.Identity.Api.Domain;
using Matterway.Identity.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Matterway.Identity.Api.Infrastructure.Persistence;

public static class ModelDataLoader
{
    public static void Initialize(ModelBuilder modelBuilder)
    {
        var employeeRoleId = new Guid("6f055a46-6bf0-4fdb-8c27-1877a2b6f811");
        var customerRoleId = new Guid("c4c29ba9-3b22-416f-8a37-8a7c3d6ed1f9");

        var adminUserId = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b1");
        var managerUserId = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b2");
        var customer01UserId = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b3");
        var customer02UserId = new Guid("a9d64b64-93c1-41a8-a742-8a8ba81e20b4");

        #region IdentityRole data

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            new IdentityRole<Guid>
            {
                Id = employeeRoleId,
                Name = nameof(EIdentityRole.Employee),
                NormalizedName = nameof(EIdentityRole.Employee).ToUpperInvariant(),
                ConcurrencyStamp = "f9f0b98d-7d9e-48d8-86c5-9a13ebf1c81a"
            },
            new IdentityRole<Guid>
            {
                Id = customerRoleId,
                Name = nameof(EIdentityRole.Customer),
                NormalizedName = nameof(EIdentityRole.Customer).ToUpperInvariant(),
                ConcurrencyStamp = "a40b1c8b-7f9f-4cbb-9de8-8c79d9f0dd8e"
            });

        #endregion

        #region User data

        modelBuilder.Entity<SystemUser>().HasData(
            new SystemUser
            {
                Id = adminUserId,
                Email = "mladen@matterway.local",
                NormalizedEmail = "MLADEN@MATTERWAY.LOCAL",
                UserName = "mladen@matterway.local",
                NormalizedUserName = "MLADEN@MATTERWAY.LOCAL",
                Created = new DateTime(2021, 9, 9, 10, 10, 10, DateTimeKind.Utc),
                PasswordHash = "AQAAAAIAAYagAAAAEF0PUBp9R90/+4Ul8J4HdcOJNNv1Ol5W2vIK2ooqOFRTc1vcgJtxBVqWYlONUSjEJw==",
                SecurityStamp = "4a1f3895-9229-45c9-9fcb-7b8f2a9f46b3",
                ConcurrencyStamp = "3d5f0a44-e0f6-4b13-8a06-1dc8db48fe74"
            },
            new SystemUser
            {
                Id = managerUserId,
                Email = "jelena@matterway.local",
                NormalizedEmail = "JELENA@MATTERWAY.LOCAL",
                UserName = "jelena@matterway.local",
                NormalizedUserName = "JELENA@MATTERWAY.LOCAL",
                Created = new DateTime(2022, 10, 10, 11, 11, 11, DateTimeKind.Utc),
                PasswordHash = "AQAAAAIAAYagAAAAEJru7jHMkz3Cdn5rk9dTj3umC8lMLv0XfkY1k5PXeuh2VkwiazogHTe8IoEhYWr6rg==",
                SecurityStamp = "bf0cb1b8-d485-4fdd-8a89-fb70120dbd9f",
                ConcurrencyStamp = "cb6a7ad6-7c4c-4bb6-bce5-8aa4d1bf7b58"
            },
            new SystemUser
            {
                Id = customer01UserId,
                Email = "stefan999@mail.local",
                NormalizedEmail = "STEFAN999@MAIL.LOCAL",
                UserName = "stefan999@mail.local",
                NormalizedUserName = "STEFAN999@MAIL.LOCAL",
                Created = new DateTime(2023, 4, 12, 12, 10, 0, DateTimeKind.Utc),
                PasswordHash = "AQAAAAIAAYagAAAAEFSn2wwtXMTkX5P1GglolnIwSI7iauwvIgZXlrTih7oTJ/FWJkY/CXmdNCR4ZJKAug==",
                SecurityStamp = "019bada3-b4dc-7155-baca-576dd9914d57",
                ConcurrencyStamp = "371c6716-8af4-48b7-ac6b-61120c592477"
            },
            new SystemUser
            {
                Id = customer02UserId,
                Email = "marag@mail.local",
                NormalizedEmail = "MARAG@MAIL.LOCAL",
                UserName = "marag@mail.local",
                NormalizedUserName = "MARAG@MAIL.LOCAL",
                Created = new DateTime(2024, 2, 20, 9, 30, 0, DateTimeKind.Utc),
                PasswordHash = "AQAAAAIAAYagAAAAEEPDW3c2GRf473Cn5iK6pW8vBqQuPvei43fnIK+WbTaixjrzkd/mMyXGIp2+IoDI8Q==",
                SecurityStamp = "8ca7016d-0e4d-4b92-96cb-c964d7d8f9c1",
                ConcurrencyStamp = "3b9f3e46-3c3c-4f04-9b6f-4fe90f0a8c80"
            });

        #endregion

        #region User IdentityRole data

        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                UserId = adminUserId,
                RoleId = employeeRoleId
            },
            new IdentityUserRole<Guid>
            {
                UserId = managerUserId,
                RoleId = employeeRoleId
            },
            new IdentityUserRole<Guid>
            {
                UserId = customer01UserId,
                RoleId = customerRoleId
            },
            new IdentityUserRole<Guid>
            {
                UserId = customer02UserId,
                RoleId = customerRoleId
            });

        #endregion

        #region User permission data

        modelBuilder.Entity<IdentityUserClaim<Guid>>().HasData(
            new IdentityUserClaim<Guid>
            {
                Id = 1,
                UserId = adminUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("identity", PermissionLevel.Manager)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 2,
                UserId = adminUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("catalog", PermissionLevel.Manager)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 3,
                UserId = adminUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("customers", PermissionLevel.Manager)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 4,
                UserId = adminUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("sales", PermissionLevel.Manager)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 5,
                UserId = managerUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("identity", PermissionLevel.Operator)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 6,
                UserId = managerUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("catalog", PermissionLevel.Operator)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 7,
                UserId = managerUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("customers", PermissionLevel.Operator)
            },
            new IdentityUserClaim<Guid>
            {
                Id = 8,
                UserId = managerUserId,
                ClaimType = PermissionClaims.ClaimType,
                ClaimValue = PermissionClaims.Format("sales", PermissionLevel.Operator)
            });

        #endregion
    }
}