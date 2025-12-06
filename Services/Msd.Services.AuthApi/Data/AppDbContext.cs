using Msd.Services.AuthApi.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Msd.Services.AuthApi.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Admin use default seeding
            const string AdminUserName = "admin@admin.com";
            const string AdminPassword = "Admin123";

            // Seed Admin Role
            var adminRoleId = "1a2b3c4d-5e6f-7g8h-9i0j-1k2l3m4n5o6p";
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = Role.Admin,
                NormalizedName = Role.Admin.ToUpperInvariant(),
                ConcurrencyStamp = adminRoleId
            });

            // Seed User Role
            var userRoleId = "3c4d5e6f-7g8h-9i0j-1k2l-3m4n5o6p7q8r";
            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = userRoleId,
                Name = Role.User,
                NormalizedName = Role.User.ToUpperInvariant(),
                ConcurrencyStamp = userRoleId
            });

            // Seed Admin User
            var adminUserId = "2b3c4d5e-6f7g-8h9i-0j1k-2l3m4n5o6p7q";
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = AdminUserName,
                NormalizedUserName = AdminUserName.ToUpper(),
                Email = AdminUserName,
                NormalizedEmail = AdminUserName.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            // Hash the password
            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, AdminPassword);

            builder.Entity<IdentityUser>().HasData(adminUser);

            // Assign Admin Role to Admin User
            builder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
        }
    }
}
