using GameDock.Server.Infrastructure.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameDock.Server.Infrastructure.Database.Configurations;

public class UserRolesSeeds : IEntityTypeConfiguration<AppRole>, IEntityTypeConfiguration<AppUser>,
    IEntityTypeConfiguration<IdentityUserRole<string>>
{
    private static readonly AppUser AdminUser = new()
    {
        Id = "20ceda78-8778-488a-9ae1-321a1ce43bfb",
        TwoFactorEnabled = false,
        Email = "admin@admin.org",
        NormalizedEmail = "ADMIN@ADMIN.ORG",
        UserName = "admin",
        NormalizedUserName = "ADMIN",
    };

    private static readonly AppRole AdminRole = new()
    {
        Id = "4459f5a8-08f4-4303-b506-534fa2bed5dd",
        Name = "Admin",
        NormalizedName = "ADMIN",
    };

    private static readonly AppRole UserRole = new()
    {
        Id = "313f6f44-4aa3-4720-beed-23fdf2a31b01",
        Name = "User",
        NormalizedName = "USER",
    };

    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        var passwordHash = new PasswordHasher<AppUser>().HashPassword(AdminUser, "admin");
        AdminUser.PasswordHash = passwordHash;
        
        builder.HasData(AdminUser);
    }

    public void Configure(EntityTypeBuilder<AppRole> builder) => builder.HasData(UserRole, AdminRole);

    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder) =>
        builder.HasData(new IdentityUserRole<string> { RoleId = AdminRole.Id, UserId = AdminUser.Id });
}